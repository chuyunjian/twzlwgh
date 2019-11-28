using Learun.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Script.Serialization;

namespace Learun.Application.Web
{
    /// <summary>
    /// 执行接口数据验证，防止非法调用与数据被篡改
    /// </summary>
    /// <remarks>
    /// 此类有特殊处理，对于测试环境，可以不用开启数据验证，生产则必须验证
    /// 为了生产环境生效，需要先配置Properties\AssemblyInfo.cs的发布文件
    /// 将[assembly: AssemblyConfiguration("")]配置为下列代码
    /// //这里设置生成的运行环境
    /// #if DEBUG 
    ///[assembly: AssemblyConfiguration("Debug")]//测试环境
    ///#else
    ///[assembly: AssemblyConfiguration("Release")] //生产环境
    ///#endif
    /// </remarks>
    public class ApiDataSecurityAttribute : AuthorizeAttribute
    {

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            ResParameter response = new ResParameter { code = ResponseCode.fail, info = "非法签名", data = new List<object>() };
            try
            {
                //获取Asp.Net对应的Request  
                var request = ((HttpContextWrapper)actionContext.Request.Properties["MS_HttpContext"]).Request;
                //DataValidate 为1则需要进行数据防篡改验证
                string DataValidate = Config.GetValue("DataValidate");
                if (DataValidate.IsEmpty()) { response.code = ResponseCode.fail; throw new Exception("缺少DataValidate配置"); }
                if (!CommonHelper.isDeBug && DataValidate == "0")
                {
                    //如果是生产环境，并且没开启数据防篡改，则抛出错误，生产必须开启
                    response.code = ResponseCode.fail; throw new Exception("请开启数据验证");
                }
                if (DataValidate.ToLower() == "0")
                {
                    response.code = ResponseCode.success;
                }
                else
                {
                    //如果标注了忽略属性则不做数据验证
                    if (actionContext.ActionDescriptor.GetCustomAttributes<IgnoreDataValidateAttribute>().Count > 0)
                    {
                        response.code = ResponseCode.success;
                    }
                    #region 时间戳验证
                    //DateTime time = DateTime.Now;
                    //if (!DateTime.TryParse(getCollection["Timestamp"], out time))
                    //{
                    //    actionContext.Response = new HttpResponseMessage(HttpStatusCode.RequestTimeout);
                    //    return;
                    //}
                    //if ((DateTime.Now - time).TotalMilliseconds > 1000 * 60)
                    //{
                    //    actionContext.Response = new HttpResponseMessage(HttpStatusCode.RequestTimeout);
                    //    return;
                    //}
                    #endregion
                    #region 数据签名验证
                    string SecurityKey = Config.GetValue("SecurityKey");
                    if (SecurityKey.IsEmpty()) { response.code = ResponseCode.fail; throw new Exception("缺少SecurityKey配置"); }
                    Dictionary<string, string> valuePairs = new Dictionary<string, string>();
                    //只是为了同时显示restful四种方式才有这部分无意义代码  
                    //实际该以哪种方式进行请求应遵循restful标准  
                    switch (request.RequestType.ToUpper())
                    {
                        case "GET":
                        case "DELETE":
                            var temp = WebHelper.UrlDecode(request.QueryString.ToString()).Split(new char[1] { '&' }, StringSplitOptions.RemoveEmptyEntries);
                            if (temp != null)
                            {
                                foreach (var item in temp)
                                {
                                    valuePairs.Add(item.Split('=')[0], item.Split('=')[1]);
                                }
                            }
                            break;
                        case "POST":
                        case "PUT":
                            //post的数据必须通过application/x-www-form-urlencoded或multipart/form-data方式传递，不然取值错误
                            var contentType = request.ContentType;
                            if (!contentType.ToLower().Contains("application/x-www-form-urlencoded") && !contentType.ToLower().Contains("multipart/form-data"))
                            {
                                response.code = ResponseCode.fail;
                                throw new Exception("post的数据必须通过application/x-www-form-urlencoded或multipart/form-data方式传递 ");
                            }
                            NameValueCollection postCollection = request.Form;
                            valuePairs = GetDictionary(postCollection);
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    //签名支持两种模式，请求头或者参数，名称都是MD5。
                    string MD5 = "";
                    if (valuePairs.Any(a => a.Key.ToUpper() == "MD5"))
                    {
                        MD5 = valuePairs.First(a => a.Key.ToUpper() == "MD5").Value;
                    }
                    else
                    {
                        var sign = from h in actionContext.Request.Headers where h.Key.ToUpper() == "MD5" select h.Value.FirstOrDefault();
                        MD5 = sign != null && sign.Count() > 0 ? sign.First() : "";
                    }
                    if (MD5.IsEmpty())
                    {
                        throw new Exception("请进行MD5签名");
                    }
                    //合法判断
                    string value = string.Join("", valuePairs.Where(a => a.Key.ToUpper() != "MD5" && a.Key.ToUpper() != "TOKEN").Select(a => a.Value));
                    value += SecurityKey;
                    string IMD5 = Md5Helper.getMd5(value);
                    if (MD5.ToUpper() != IMD5.ToUpper())
                    {
                        throw new Exception("非法访问");
                    }
                    else
                    {
                        //验证通过
                        response.code = ResponseCode.success;
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                response.code = ResponseCode.unauthorized;
                response.info = ex.Message;
            }
            if (response.code == ResponseCode.success)
            {
                //授权成功
                IsAuthorized(actionContext);
            }
            else
            {
                if (actionContext.Response == null) { actionContext.Response = new HttpResponseMessage(); }
                actionContext.Response.Content = new StringContent(response.ToJson(), Encoding.UTF8, "application/json");
                //授权失败
                HandleUnauthorizedRequest(actionContext);
            }
        }
        protected override void HandleUnauthorizedRequest(HttpActionContext filterContext)
        {
            //base.HandleUnauthorizedRequest(filterContext);//使用了自定义消息与状态码，不执行自带的未授权方法
            var response = filterContext.Response;
            if (response == null)
            {
                response = new HttpResponseMessage();
                ResParameter res2 = new ResParameter { code = ResponseCode.nologin, info = "授权错误", data = new List<object>() };
                response.Content = new StringContent(res2.ToJson(), Encoding.UTF8, "application/json");
            }
            response.StatusCode = HttpStatusCode.OK;
        }
        private static Dictionary<string, string> GetDictionary(NameValueCollection collection, Func<string, bool> filter = null)
        {//获取客户端排序的键值对  
            Dictionary<string, string> dic = new Dictionary<string, string>();
            if (collection != null && collection.Count > 0)
            {
                foreach (var k in collection.AllKeys)
                {
                    if (k == null) continue;
                    if (filter == null || !filter(k))
                    {//如果没设置过滤条件或者无需过滤  
                        dic.Add(k, collection[k]);
                    }
                }
            }
            return dic;
        }
    }
    /// <summary>
    /// 不做数据防篡改签名验证
    /// </summary>
    public class IgnoreDataValidateAttribute : System.Attribute
    {
    }
}