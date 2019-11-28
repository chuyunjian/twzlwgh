using Learun.Application.AppMagager;
using Learun.Util;
using Newtonsoft.Json;
using System;
using System.Web.Http;
using System.Linq;
namespace Learun.Application.Web.SYS_Code
{
    /// <summary>
    /// 移动功能相关
    /// </summary>
    [RoutePrefix("rest/function")]
    public class FunctionController : BaseApi
    {
        private FunctionIBLL functionIBLL = new FunctionBLL();

        /// <summary>
        /// 获取全部移动功能数据
        /// </summary>
        /// <returns></returns>
        [Route("list")]
        [HttpGet]
        public ResParameter GetList()
        {
            try
            {
                var list = functionIBLL.GetList(LoginUser).ToList();
                var josnData = from m in list
                               select new
                               {
                                   m.F_Name,//名字
                                   m.F_Code,//编码
                                   F_Img = m.HTTP_F_Img,//图片
                                   m.F_Url,//地址
                               };
                return Success(josnData);
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }
    }
}
