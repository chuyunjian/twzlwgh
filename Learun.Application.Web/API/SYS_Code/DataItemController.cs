using Learun.Application.Base.SystemModule;
using Learun.Application.TwoDevelopment.SYS_Code;
using Learun.Util;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Learun.Application.Web.API.Anonymous
{
    /// <summary>
    /// 数据字典
    /// </summary>
    [RoutePrefix("rest/dataItem")]
     [ApiAuthorize(FilterMode.Ignore)]
    public class DataItemController : BaseApi
    {
        private DataItemIBLL dataItemIBLL = new DataItemBLL();

        /// <summary>
        /// 通过数据字典code返回集合
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [Route("list"), HttpGet]
        public ResParameter GetDataItemByName(string code)
        {
            try
            {
                var datas = dataItemIBLL.GetDetailList(code);
                var result = from item in datas
                             select new
                             {
                                 item.F_ItemValue,
                                 item.F_ItemName
                             };
                return Success(result);
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        /// <summary>
        /// 通过数据词典的Value得到对应的Text
        /// </summary>
        /// <param name="code">数据词典的编码</param>
        /// <param name="value">数据词典值</param>
        /// <returns>返回数据词典键</returns>
        [Route("text"), HttpGet]
        public ResParameter GetDataItemTextByCodeAndValue(string code, string value)
        {
            try
            {
                var datas = dataItemIBLL.GetDataItemTextByCodeAndValue(code, value);
                var result = new
                {
                    F_ItemName = datas
                };
                return Success(result);
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }
        /// <summary>
        /// 通过数据词典的Text得到对应的Value
        /// </summary>
        /// <param name="code">数据词典的编码</param>
        /// <param name="text">数据词典名称</param>
        /// <returns>返回数据词典值</returns>
        [Route("value"), HttpGet]
        public ResParameter GetDataItemValueByCodeAndText(string code, string text)
        {
            try
            {
                var datas = dataItemIBLL.GetDataItemValueByCodeAndText(code, text);
                var result = new
                {
                    F_ItemValue = datas
                };
                return Success(result);
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }
    }
}