using Learun.Application.Organization;
using Learun.Application.TwoDevelopment.SYS_Code;
using Learun.Application.WorkFlow;
using Learun.Util;
using Learun.Util.Login;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Learun.Application.Web.SYS_Code
{
    /// <summary>
    /// 流程相关接口
    /// </summary>
    [RoutePrefix("rest/workflow")]
    public class WorkFlowController : BaseApi
    {
        private WfEngineIBLL wfEngineIBLL = new WfEngineBLL();
        private WorkFlowIBLL workFlowIBLL = new WorkFlowBLL();

        #region 获取数据
        /// <summary>
        /// 获取自定义流程列表
        /// </summary>
        /// <returns></returns>
        /// <param name="F_Category">流程分类</param>
        [Route("list"), HttpGet]
        public ResParameter GetCustmerSchemeInfoList(string F_Category = "")
        {
            try
            {
                UserInfo userInfo = LoginUserInfo.Get();
                var datas = workFlowIBLL.GetCustmerSchemeInfoList(userInfo, F_Category);
                return Success(datas);
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }
        /// <summary>
        /// 获取流程实例信息
        /// </summary>
        /// <param name="processId"></param>
        /// <param name="taskId"></param>
        /// <returns></returns>
        [Route("detail"), HttpGet]
        public ResParameter ProcessinfoByMonitor(string processId = "", string taskId = "")
        {
            try
            {
                WfParameter wfParameter = new WfParameter();
                UserInfo userInfo = LoginUserInfo.Get();

                wfParameter.companyId = userInfo.companyId;
                wfParameter.departmentId = userInfo.departmentId;
                wfParameter.userId = userInfo.userId;
                wfParameter.userName = userInfo.realName;
                wfParameter.processId = processId;
                wfParameter.taskId = taskId;
                WfResult<WfContent> res = wfEngineIBLL.GetProcessInfoByMonitor(wfParameter);
                if (res.status != 1)
                {
                    throw new ExceptionEx(res.desc, null);
                }
                return Success(res.data);
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }
        #endregion
    }
}
