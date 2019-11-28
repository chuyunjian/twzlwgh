using Learun.Util;
using Newtonsoft.Json;
using System;
using System.Web.Http;

namespace Learun.Application.Web.SYS_Code
{
    /// <summary>
    /// 百度地图相关接口
    /// </summary>
    [RoutePrefix("rest/bmap")]
    public class BMapController : BaseApi
    {
        #region 用户获取
        /// <summary>
        /// 通过经纬度，获取坐标信息
        /// </summary>
        /// <param name="Latitude">纬度</param>
        /// <param name="Longitude">经度</param>
        /// <returns></returns>
        [Route("pointDetail")]
        [HttpGet]
        public ResParameter GetPointDetail(string Latitude, string Longitude)
        {
            try
            {
                var url = "http://api.map.baidu.com/geocoder/v2/?output=json&ak={Key}&location={Latitude},{Longitude}&callback=showLocation";
                url = url.Replace("{Key}", Config.GetValue("BMapKey"));
                url = url.Replace("{Latitude}", Latitude);
                url = url.Replace("{Longitude}", Longitude);
                var jsondata = HttpMethods.Get(url);
                jsondata = jsondata.Replace("showLocation&&showLocation(", "");
                jsondata = jsondata.Substring(0, jsondata.Length - 1);
                var data = JsonConvert.DeserializeAnonymousType(jsondata, new { status = 0, result = new { formatted_address = "", sematic_description = "" } });
                if (data.status != 0) { throw new Exception("坐标信息获取失败"); }
                var model = new { Address = data.result.formatted_address + data.result.sematic_description };
                return Success(model);
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }
        #endregion
    }
}
