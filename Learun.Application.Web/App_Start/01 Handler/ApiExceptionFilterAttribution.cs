using Learun.Util;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http.Filters;

namespace Learun.Application.Web
{
    /// <summary>
    /// API接口异常处理器
    /// </summary>
    public class ApiExceptionFilterAttribution : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            ResParameter res = new ResParameter { code = ResponseCode.exception, info = actionExecutedContext.Exception.Message, data = new List<object>() };
            if (actionExecutedContext.Response == null) { actionExecutedContext.Response = new HttpResponseMessage(); }
            actionExecutedContext.Response.Content = new StringContent(res.ToJson(), Encoding.UTF8, "application/json");
            actionExecutedContext.Response.StatusCode = HttpStatusCode.OK;
        }
    }
}