using Learun.Util;
using System;
using System.Collections.Generic;
using Learun.Application.WorkFlow;

namespace Learun.Application.TwoDevelopment.SYS_Code
{
    /// <summary>
    /// 版 本 Learun-ADMS V7.0.0 力软敏捷开发框架
    /// Copyright (c) 2013-2018 上海力软信息技术有限公司
    /// 创 建：超级管理员
    /// 日 期：2019-05-05 10:07
    /// 描 述：流程信息
    /// </summary>
    public class WorkFlowBLL : WorkFlowIBLL
    {
        private WorkFlowService workFlowService = new WorkFlowService();

        #region 获取数据
        /// <summary>
        /// 获取自定义流程列表
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="F_Category"></param>
        /// <returns></returns>
        public IEnumerable<WfSchemeInfoEntity> GetCustmerSchemeInfoList(UserInfo userInfo, string F_Category)
        {
            try
            {
                return workFlowService.GetCustmerSchemeInfoList(userInfo, F_Category);
            }
            catch (Exception ex)
            {
                if (ex is ExceptionEx)
                {
                    throw;
                }
                else
                {
                    throw ExceptionEx.ThrowBusinessException(ex);
                }
            }
        }
        /// <summary>
        /// 通过流程实例ID及登录用户，得到该用户的任务信息
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="ProcessId"></param>
        /// <returns></returns>
        public IEnumerable<WfTaskEntity> GetTaskByProcessId(UserInfo userInfo, string ProcessId)
        {
            try
            {
                return workFlowService.GetTaskByProcessId(userInfo, ProcessId);
            }
            catch (Exception ex)
            {
                if (ex is ExceptionEx)
                {
                    throw;
                }
                else
                {
                    throw ExceptionEx.ThrowBusinessException(ex);
                }
            }
        }
        /// <summary>
        /// 得到流程实例是否结束
        /// </summary>
        /// <returns></returns>
        public bool GetProcessFinished(string ProcessId)
        {
            try
            {
                return workFlowService.GetProcessFinished(ProcessId);
            }
            catch (Exception ex)
            {
                if (ex is ExceptionEx)
                {
                    throw;
                }
                else
                {
                    throw ExceptionEx.ThrowBusinessException(ex);
                }
            }
        }
        #endregion

    }
}
