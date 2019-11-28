using Learun.Util;
using Learun.Util.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Learun.Application.Web
{
    /// <summary>
    /// 接口权限过滤
    /// </summary>
    public class ApiAuthorizeAttribute : AuthorizeAttribute
    {
        private FilterMode _customMode;
        public ApiAuthorizeAttribute(FilterMode mode)
        {
            _customMode = mode;
        }
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            ResParameter response = new ResParameter { code = ResponseCode.unauthorized, info = "未经授权", data = new List<object>() };
            try
            {
                //控制器上如果标注了忽略属性则不做权限验证 以及 方法上单独加不做权限验证
                if (_customMode == FilterMode.Ignore ||
                   actionContext.ActionDescriptor.GetCustomAttributes<ApiAuthorizeAttribute>().Where(a => a._customMode == FilterMode.Ignore).Count() > 0)
                {
                    response.code = ResponseCode.success;
                }
                else
                {
                    string token = "";
                    //前端请求api时会将token存放在名为"authorization"的请求头中          
                    var request = ((HttpContextWrapper)actionContext.Request.Properties["MS_HttpContext"]).Request;
                    token = request.Headers.Get("Authorization");
                    if (token == null)
                    {
                        token = request.QueryString.Get("token");
                        if (token.IsEmpty())
                        {
                            token = request.Form.Get("token");
                        }
                    }
                    if (string.IsNullOrEmpty(token))
                    {
                        throw new Exception("token为空,请重新登录");
                    }
                    token = token.Replace("Bearer ", "");
                    //对token进行解密
                    var data = JwtHelp.GetJwtDecode(token);
                    if (data != null && data.Keys.Any(a => a == "guid"))
                    {
                        var utoken = data.First(a => a.Key.ToLower() == "guid").Value;
                        var account = data.First(a => a.Key.ToLower() == "account").Value;
                        var loginType = data.First(a => a.Key.ToLower() == "logintype").Value;
                        var loginIdentity = data.First(a => a.Key.ToLower() == "loginidentity").Value;
                        string loginMark = Md5Helper.Encrypt(Net.Ip + "_" + account + "_" + loginIdentity + "_" + LoginMode.APP.ToString(), 32);//登录设备唯一标识
                        LoginResult res = LoginHelper.Instance.IsOnLine(utoken.ToString(), loginMark);
                        if (res.stateCode == 1)
                        {
                            response.code = ResponseCode.success;
                        }
                        else
                        {
                            throw new Exception("请重新登录");
                        }
                    }
                    else
                    {
                        throw new Exception("token为空,请重新登录");
                    }
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
    }
}