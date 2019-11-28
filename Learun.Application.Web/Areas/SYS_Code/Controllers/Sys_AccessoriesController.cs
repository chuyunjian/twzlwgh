using Learun.Application.TwoDevelopment.SYS_Code;
using Learun.Util;
using System.Web.Mvc;

namespace Learun.Application.Web.Areas.SYS_Code.Controllers
{
    /// <summary>
    /// 版 本 Learun-ADMS V7.0.0 力软敏捷开发框架
    /// Copyright (c) 2013-2018 上海力软信息技术有限公司
    /// 创 建：超级管理员
    /// 日 期：2018-12-11 23:06
    /// 描 述：附件管理
    /// </summary>
    public class Sys_AccessoriesController : MvcControllerBase
    {
        private Sys_AccessoriesIBLL sys_AccessoriesIBLL = new Sys_AccessoriesBLL();

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
            var mod = sys_AccessoriesIBLL.GetEntity(keyValue);
            return View(mod);
        }
        /// <summary>
        /// 上传列表页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult UploadForm()
        {
            return View();
        }
        /// <summary>
        /// 下载列表页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult DownForm()
        {
            return View();
        }
        /// <summary>
        /// 上传头像页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult UploadAvatar()
        {
            return View();
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
            var data = sys_AccessoriesIBLL.GetList(queryJson);
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
            var data = sys_AccessoriesIBLL.GetPageList(paginationobj, queryJson);

         
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
            var data = sys_AccessoriesIBLL.GetEntity(keyValue);
            return Success(data);
        }

        /// <summary>
        /// 获取附件列表
        /// </summary>
        /// <param name="OperationCode">附件编码</param>
        /// <param name="OperationID">附件业务主键</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetFileList(string OperationCode, string OperationID)
        {
            var data = sys_AccessoriesIBLL.GetList(new { OperationCode, OperationID }.ToJson());
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
            sys_AccessoriesIBLL.DeleteEntity(keyValue);
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
        public ActionResult SaveForm(string keyValue, Sys_AccessoriesEntity entity)
        {
            sys_AccessoriesIBLL.SaveEntity(keyValue, entity);
            return Success("保存成功！");
        }
        #endregion

    }
}
