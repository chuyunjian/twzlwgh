using Learun.Application.TwoDevelopment.SYS_Code;
using Learun.Util;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Data;
using System.Web.Mvc;

namespace Learun.Application.Web.Areas.SYS_Code.Controllers
{
    /// <summary>
    /// 版 本 Learun-ADMS V7.0.0 力软敏捷开发框架
    /// Copyright (c) 2013-2018 上海力软信息技术有限公司
    /// 创 建：超级管理员
    /// 日 期：2019-05-06 08:35
    /// 描 述：APP管理
    /// </summary>
    public class APPController : MvcControllerBase
    {
        private APPIBLL aPPIBLL = new APPBLL();

        #region 视图功能

        /// <summary>
        /// 主页面
        /// <summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 表单页
        /// <summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Form()
        {
            return View();
        }
        /// <summary>
        /// 详情页
        /// <summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Detail(string keyValue)
        {
            var mod = aPPIBLL.GetEntity(keyValue);
            return View(mod);
        }
        /// <summary>
        /// app下载页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HandlerLogin(FilterMode.Ignore)]
        public ActionResult APP()
        {
            APPIBLL bll = new APPBLL();
            //0安卓1苹果
            APPEntity AppModel = bll.GetLastVersion("0");
            Sys_AccessoriesIBLL accessoriesIBLL = new Sys_AccessoriesBLL();

            if (AppModel == null)
            {
                AppModel = new APPEntity();
                ViewData["HasAndroidApp"] = "false";
            }
            else
            {
                JObject jquery = new JObject();
                jquery.Add("OperationCode", "Version");
                jquery.Add("OperationID", AppModel.AGuid);
                var sys = (List<Sys_AccessoriesEntity>)accessoriesIBLL.GetList(jquery.ToJson());
                AppModel.FileUrl = sys[0].getHttpPath();
                ViewData["HasAndroidApp"] = "true";
            }
            return View(AppModel);
        }
        #endregion

        #region 获取数据

        /// <summary>
        /// 获取列表数据
        /// <summary>
        /// <returns></returns>
        [HttpGet]
        [AjaxOnly]
        public ActionResult GetList(string queryJson)
        {
            var data = aPPIBLL.GetList(queryJson);
            return Success(data);
        }
        /// <summary>
        /// 获取列表分页数据
        /// <param name="pagination">分页参数</param>
        /// <summary>
        /// <returns></returns>
        [HttpGet]
        [AjaxOnly]
        public ActionResult GetPageList(string pagination, string queryJson)
        {
            Pagination paginationobj = pagination.ToObject<Pagination>();
            var data = aPPIBLL.GetPageList(paginationobj, queryJson);
            var jsonData = new
            {
                rows = data,
                total = paginationobj.total,
                page = paginationobj.page,
                records = paginationobj.records
            };
            return Success(jsonData);
        }
        /// <summary>
        /// 获取表单数据
        /// <param name="keyValue">主键</param>
        /// <summary>
        /// <returns></returns>
        [HttpGet]
        [AjaxOnly]
        public ActionResult GetFormData(string keyValue)
        {
            var data = aPPIBLL.GetEntity(keyValue);
            return Success(data);
        }
        #endregion

        #region 提交数据

        /// <summary>
        /// 删除实体数据
        /// <param name="keyValue">主键</param>
        /// <summary>
        /// <returns></returns>
        [HttpPost]
        [AjaxOnly]
        public ActionResult DeleteForm(string keyValue)
        {
            aPPIBLL.DeleteEntity(keyValue);
            return Success("删除成功！");
        }
        /// <summary>
        /// 保存实体数据（新增、修改）
        /// <param name="keyValue">主键</param>
        /// <summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public ActionResult SaveForm(string keyValue, APPEntity entity, string Attachments)
        {
            aPPIBLL.SaveEntity(keyValue, entity, Attachments);
            return Success("保存成功！");
        }
        #endregion

    }
}
