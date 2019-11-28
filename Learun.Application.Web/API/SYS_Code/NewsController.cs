using Learun.Application.OA;
using Learun.Util;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Learun.Application.Web.API.SYS_Code
{
     [RoutePrefix("rest/news")]
    public class NewsController : BaseApi
    {
         private NewsIBLL newsIBLL = new NewsBLL();
         /// <summary>
         /// 得到新闻列表
         /// </summary>
         /// <returns></returns>
         [Route("list"), HttpGet]
         public ResParameter GetMyPageList(int PageIndex = 1, int PageSize = 10)
         {
             try
             {
                 Pagination paginationobj = new Pagination();
                 paginationobj.page = PageIndex;
                 paginationobj.rows = PageSize;
                 paginationobj.sord = "DESC";
                 paginationobj.sidx = "F_ReleaseTime";
               
                 var data = newsIBLL.GetPageList(paginationobj, "");
                 var dataList = from m in data
                                select new
                                {
                                    m.F_FullHead,
                                    m.F_ReleaseTimeStr,
                                    m.F_Category,
                                    m.F_NewsId
                                };
                 var jsonData = new
                 {
                     rows = dataList,
                     total = paginationobj.total,
                     page = paginationobj.page,
                     records = paginationobj.records
                 };
                 return Success(jsonData);
             }
             catch (Exception ex)
             {
                 return Fail(ex.Message);
             }
         }

         /// <summary>
         /// 获取新闻信息
         /// </summary>
         /// <param name="FCode">农户主键</param>
         /// <returns></returns>
         [Route("detail"), HttpGet]
         public ResParameter GetDetail(string F_NewsId)
         {
             try
             {
                 if (F_NewsId.IsEmpty())
                 {
                     throw new Exception("请传公告主键编号！");
                 }
                 var data = newsIBLL.GetEntity(F_NewsId);
                 var result = new
                 {
                     data.F_NewsId,//编号
                     data.F_FullHead,//标题
                     data.F_Category,//所属类别
                     data.F_ReleaseTimeStr,//发布时间
                     F_NewsContent = WebHelper.HtmlDecode(data.F_NewsContent).AdditionalDomainName(Config.GetValue("UploadUrl")),//内容
                   
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
