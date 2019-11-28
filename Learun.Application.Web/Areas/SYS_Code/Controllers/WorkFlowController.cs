using Learun.Application.TwoDevelopment.SYS_Code;
using Learun.Application.WorkFlow;
using Learun.Util;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Learun.Application.Web.Areas.SYS_Code.Controllers
{
    /// <summary>
    /// 版 本 Learun-ADMS V7.0.0 力软敏捷开发框架
    /// Copyright (c) 2013-2018 上海力软信息技术有限公司
    /// 创建人：力软-框架开发组
    /// 日 期：2017.04.17
    /// 描 述：工作流模板处理
    /// </summary>
    public class WorkFlowController : MvcControllerBase
    {
        private WorkFlowIBLL workFlowIBLL = new WorkFlowBLL();

        #region 获取数据
        /// <summary>
        /// 获取自定义流程列表
        /// </summary>
        /// <returns></returns>
        /// <param name="F_Category">流程分类</param>
        [HttpGet]
        [AjaxOnly]
        public ActionResult GetCustmerSchemeInfoList(string F_Category = "")
        {
            UserInfo userInfo = LoginUserInfo.Get();
            var data = workFlowIBLL.GetCustmerSchemeInfoList(userInfo, F_Category);
            return Success(data);
        }
        #endregion
    }
}