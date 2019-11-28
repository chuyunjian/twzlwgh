using Learun.Application.Base.SystemModule; 
using Learun.Application.TwoDevelopment.SYS_Code;
using Learun.Util;
using Newtonsoft.Json.Linq;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Learun.Application.Web.API.Anonymous
{
    /// <summary>
    /// APP管理
    /// </summary>
    [RoutePrefix("rest/app")]
    [ApiAuthorize(FilterMode.Ignore)]
    [SwaggerResponse(System.Net.HttpStatusCode.OK, Description = "日志信息", Type = typeof(APPEntity))]
    public class APPController : BaseApi
    {
        private APPIBLL appIBLL = new APPBLL();
        /// <summary>
        /// 获取最新版本
        /// </summary>
        /// <param name="Type">版本类型</param> 
        /// <returns></returns>
        [Route("getVersion"), HttpGet]
        public object UpdateVersion(string Type)
        {
            try
            { 
                JObject queryParam = new JObject();
                queryParam.Add("Type", Type);
                var list = appIBLL.GetList(1, queryParam.ToJson());
                if (list.Count() == 0)
                {
                    return Success(new { result = "0", message = "没有找到更新文件" });
                }
                Sys_AccessoriesIBLL accIBll = new Sys_AccessoriesBLL();
                List<object> relist = new List<object>();
                foreach (var item in list)
                {
                    JObject jqobj = new JObject();
                    jqobj["OperationCode"] = "Version";
                    jqobj["OperationID"] = item.AGuid;

                    var accInfo = accIBll.GetList(jqobj.ToString()).FirstOrDefault();
                    if (accInfo == null)
                    {
                        accInfo = new Sys_AccessoriesEntity();
                    }
                    relist.Add(new
                    {
                        DownLoadURL = accInfo.getHttpPath(),
                        VersionCode = item.Version,
                        VersionName = item.Name,
                        UpadatContent = item.ARemark
                    });
                }

                var result = new
                {
                    result = "1",
                    message = "成功",
                    list = relist
                };
                return SuccessData(result);
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }
        
    }
}