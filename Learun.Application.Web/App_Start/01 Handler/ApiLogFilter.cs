using Learun.Application.Base.SystemModule;
using Learun.Util;
using Learun.Util.Operat;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Learun.Application.Web
{
    /// <summary>
    /// API日志
    /// </summary>
    public class ApiLogFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {

        }
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            var log = Config.GetValue("APILog");
            if (!log.IsEmpty() && log == "1")
            {
                var request = ((HttpContextWrapper)actionExecutedContext.Request.Properties["MS_HttpContext"]).Request;
                var user = LoginUserInfo.Get();
                string url = actionExecutedContext.Request.RequestUri.AbsoluteUri;
                dynamic data = actionExecutedContext.Request.Content;
                dynamic content = actionExecutedContext.Response.Content;
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
                                string[] arry = item.Split('=');
                                if (arry.Length > 1)
                                {
                                    valuePairs.Add(arry[0], arry[1]);
                                }
                            }
                        }
                        break;
                    case "POST":
                    case "PUT":
                        NameValueCollection postCollection = request.Form;
                        valuePairs = GetDictionary(postCollection);
                        break;
                    default:
                        throw new NotImplementedException();
                }
                //使用异步记录日志
                Task.Run(() =>
                {
                    WriteLog(
                        url,
                        (data != null ? string.Join("</br>", valuePairs.Select(x => x.Key + ":" + x.Value)) : ""),
                        (content != null ? JsonConvert.SerializeObject(content.Value) : "")
                        , user);
                });
            }
        }

        #region 获取请求数据
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
        #endregion

        #region 写入日志
        /// <summary>
        /// 写操作日志
        /// </summary>
        public void WriteLog(string url, string data, string content, UserInfo user)
        {
            try
            {
                //记录日志
                LogEntity logEntity = new LogEntity();
                if (user == null)
                {
                    logEntity.F_OperateAccount = "sbdosft";
                    logEntity.F_OperateUserId = "sbdosft";
                }
                else
                {
                    logEntity.F_OperateAccount = user.account;
                    logEntity.F_OperateUserId = user.userId;
                }
                logEntity.F_CategoryId = 3;
                logEntity.F_OperateTypeId = ((int)OperationType.InterFace).ToString();
                logEntity.F_OperateType = EnumAttribute.GetDescription(OperationType.InterFace);
                logEntity.F_Module = "API";
                logEntity.F_SourceContentJson = url;
                logEntity.F_ExecuteResult = 1;
                logEntity.F_ExecuteResultJson = content;
                logEntity.F_Description = data;
                LogBLL.WriteLog(logEntity);
            }
            catch (Exception ex)
            {
            }
        }

        #endregion
    }
}