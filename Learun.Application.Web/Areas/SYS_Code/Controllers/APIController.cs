using Learun.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
namespace Learun.Application.Web.Areas.SYS_Code.Controllers
{
    /// <summary>
    /// API文档
    /// </summary>
    /// <remarks>
    /// 使用了HelpPage接口文档包
    /// 需要 项目属性-》生成-》XML 文档文件
    /// </remarks>
    [HandlerLogin(FilterMode.Ignore)]
    public class APIController : MvcControllerBase
    {
        #region 构造
        public System.Web.Http.HttpConfiguration Configuration { get; private set; }
        public APIController()
            : this(System.Web.Http.GlobalConfiguration.Configuration)
        {
        }
        public APIController(System.Web.Http.HttpConfiguration config)
        {
            Configuration = config;
        }
        #endregion

        #region 视图功能

        /// <summary>
        /// 主页面
        /// <summary>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        [HandlerLogin(FilterMode.Enforce)]
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 调试
        /// <summary>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        [HandlerLogin(FilterMode.Enforce)]
        public ActionResult Debug(string keyValue)
        {
            keyValue = HttpUtility.UrlDecode(keyValue);
            var apiList = Configuration.Services.GetApiExplorer().ApiDescriptions.Where(a => a.ID == keyValue);
            ILookup<System.Web.Http.Controllers.HttpControllerDescriptor, System.Web.Http.Description.ApiDescription> apiGroups =
                apiList.ToLookup(api => api.ActionDescriptor.ControllerDescriptor);
            foreach (var item in apiList)
            {
                ViewBag.Method = item.HttpMethod.Method;
            }
            return View(apiGroups);
        }
        /// <summary>
        /// 接口使用说明
        /// </summary>
        /// <returns></returns>
        public ActionResult Explain()
        {
            return View();
        }
        #endregion

        #region 获取数据
        /// <summary>
        /// 获取列表分页数据
        /// <param name="pagination">分页参数</param>
        /// <summary>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        [AjaxOnly]
        [HandlerLogin(FilterMode.Enforce)]
        public ActionResult GetPageList(string pagination, string queryJson)
        {
            Pagination paginationobj = pagination.ToObject<Pagination>();
            var apiList = Configuration.Services.GetApiExplorer().ApiDescriptions;
            ILookup<System.Web.Http.Controllers.HttpControllerDescriptor, System.Web.Http.Description.ApiDescription> apiGroups =
                apiList.ToLookup(api => api.ActionDescriptor.ControllerDescriptor);
            List<dynamic> rows = new List<dynamic>();
            foreach (var item in apiGroups)
            {
                foreach (var api in item)
                {
                    dynamic obj = new System.Dynamic.ExpandoObject();
                    obj.ID = api.ID;
                    obj.Controller = item.Key.ControllerName;
                    obj.Name = api.Documentation;
                    obj.Method = api.HttpMethod.Method;
                    if (api.RelativePath.IndexOf("?") > 0)
                    {
                        obj.Route = api.RelativePath.Substring(0, api.RelativePath.IndexOf("?"));
                    }
                    else
                    {
                        obj.Route = api.RelativePath;
                    }
                    var ParamDesc = " ";
                    foreach (var param in api.ParameterDescriptions)
                    {
                         ParamDesc += param.Name+":"+ param.Documentation + ";";
                    }
                    obj.ParamDesc = ParamDesc.Substring(0,ParamDesc.Length - 1);
                    rows.Add(obj);
                }
            }

            var queryParam = queryJson.ToJObject();
            if (!queryParam["Controller"].IsEmpty())
            {
                rows = rows.Where(a => a.Controller.ToString().ToLower().Contains(queryParam["Controller"].ToString().ToLower())).ToList();
            }
            if (!queryParam["Route"].IsEmpty())
            {
                rows = rows.Where(a => a.Route.ToString().ToLower().Contains(queryParam["Route"].ToString().ToLower())).ToList();
            }
            rows = rows.Skip(paginationobj.rows * (paginationobj.page - 1)).Take(paginationobj.rows).ToList();
            var jsonData = new
            {
                rows = rows,
                total = paginationobj.total,
                page = paginationobj.page,
                records = paginationobj.records
            };
            return Success(jsonData);
        }
        #endregion
    }
}