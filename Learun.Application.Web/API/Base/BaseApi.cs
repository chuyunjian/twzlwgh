using Learun.Application.TwoDevelopment.SYS_Code;
using Learun.Loger;
using Learun.Util;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.Http;
using System.Web.Http.Description;

namespace Learun.Application.Web
{
    /// <summary>
    /// 接口父类
    /// 创建人：严笛
    /// 日 期：2019-03-19
    /// </summary>
    [ApiLogFilter]
    [ApiDataSecurity]
    [ApiAuthorize(FilterMode.Enforce)]
    [System.Web.Mvc.ValidateInput(false)]
    public class BaseApi : ApiController
    {
        protected System.Web.HttpContext httpContext;

        #region 参数转为JObject对象
        /// <summary>
        /// 获取请求参数，并转为JObject对象
        /// </summary>
        public JObject requestParam
        {
            get
            {
                var request = httpContext.Request;
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
                        break;
                }
                return valuePairs.ToJson().ToJObject();
            }
        }
        private Dictionary<string, string> GetDictionary(NameValueCollection collection, Func<string, bool> filter = null)
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

        #region 构造函数
        public BaseApi()
        {
            //统一跨域设置
            httpContext = System.Web.HttpContext.Current;
            httpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            httpContext.Response.Headers.Add("Access-Control-Allow-Methods", "*");
            httpContext.Response.Headers.Add("Access-Control-Allow-Headers", "*");
            //OPTIONS请求直接返回
            if (httpContext.Request.HttpMethod.ToUpper() == "OPTIONS")
            {
                httpContext.Response.StatusCode = 200;
                httpContext.Response.End();
            }
        }
        #endregion

        #region 响应接口
        /// <summary>
        /// 成功响应数据
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        public ResParameter Success(string info)
        {
            return new ResParameter { code = ResponseCode.success, info = info, data = new List<object>() };
        }
        /// <summary>
        /// 成功响应数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="res"></param>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        public ResParameter Success(object data)
        {
            return new ResParameter { code = ResponseCode.success, info = "响应成功", data = data };
        }
        /// <summary>
        /// 成功响应数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="res"></param>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        public ResParameter Success<T>(T data) where T : class
        {
            ResParameter res = new ResParameter { code = ResponseCode.success, info = "响应成功", data = data };
            return res;
        }
        /// <summary>
        /// 成功响应数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="res"></param>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        public object SuccessData<T>(T data) where T : class
        {
            ResParameter res = new ResParameter {  data = data };
            return res.data;
        }
        /// <summary>
        /// 成功响应数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="res"></param>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        public ResParameter SuccessString(string data)
        {
            ResParameter res = new ResParameter { code = ResponseCode.success, info = "响应成功", data = data };
            return res;
        }
        /// <summary>
        /// 接口响应失败
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        public ResParameter Fail(string info)
        {
            ResParameter res = new ResParameter { code = ResponseCode.fail, info = info, data = new List<object>() };
            return res;
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public ResParameter FailNoLogin(string info)
        {
            ResParameter res = new ResParameter { code = ResponseCode.nologin, info = info, data = new List<object>() };
            return res;
        }
        #endregion

        #region 日志操作
        /// <summary>
        /// 日志对象实体
        /// </summary>
        private Log _logger;
        /// <summary>
        /// 日志操作
        /// </summary>
        public Log Logger
        {
            get { return _logger ?? (_logger = LogFactory.GetLogger(this.GetType().ToString())); }
        }
        #endregion

        #region 登录用户
        private UserInfo _loginUser = null;
        /// <summary>
        /// 登录用户
        /// </summary>
        public UserInfo LoginUser
        {
            get
            {
                _loginUser = LoginUserInfo.Get();
                if (_loginUser == null)
                {
                    _loginUser = new UserInfo();
                }
                return _loginUser;
            }
        }
        #endregion

        #region 通过业务主键编号和附件编码得到第一张附件路径
        /// <summary>
        /// 通过业务主键编号和附件编码得到第一张附件路径
        /// </summary>
        /// <param name="OperationID">业务主键编号</param>
        /// <param name="OperationCode">业务附件编码</param>
        /// <returns>返回第一张附件路径</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        public string GetFileUrl(string OperationID, string OperationCode)
        {
            string url = string.Empty;
            Sys_AccessoriesIBLL bllA = new Sys_AccessoriesBLL();
            url = bllA.GetFileUrl(OperationID, OperationCode);
            if (url.IsEmpty())
            {
                url = Config.GetValue("UploadUrl") + "/Content/images/homene.png";
            }
            return url;
        }
        #endregion



    }
}