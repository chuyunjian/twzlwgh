using Dapper;
using Learun.Application.Organization;
using Learun.Application.WorkFlow;
using Learun.DataBase.Repository;
using Learun.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Learun.Application.TwoDevelopment.SYS_Code
{
    /// <summary>
    /// 创 建：严笛
    /// 日 期：2019-05-05 10:07
    /// 描 述：流程增强类
    /// </summary>
    public class WorkFlowService : RepositoryFactory
    {
        private WfTaskService wfTaskService = new WfTaskService();
        private UserService userService = new UserService();
        private Base.AuthorizeModule.UserRelationService userRelationService = new Base.AuthorizeModule.UserRelationService();
        #region 构造函数和属性
        private string schemeInfoFieldSql;
        private string taskFieldSql;
        public WorkFlowService()
        {
            schemeInfoFieldSql = @" 
                        t.F_Id,
                        t.F_Code,
                        t.F_Name,
                        t.F_Category,
                        t.F_Kind,
                        t.F_SchemeId,
                        t.F_DeleteMark,
                        t.F_EnabledMark,
                        t.F_Description
                        ";
            taskFieldSql = @" 
                t.F_Id, 
                t.F_ProcessId, 
                t.F_NodeId, 
                t.F_NodeName, 
                t.F_TaskType, 
                t.F_IsFinished, 
                t.F_AuditorId, 
                t.F_AuditorName, 
                t.F_CompanyId, 
                t.F_DepartmentId, 
                t.F_TimeoutAction, 
                t.F_TimeoutNotice, 
                t.F_PreviousId, 
                t.F_PreviousName, 
                t.F_CreateDate, 
                t.F_CreateUserId, 
                t.F_CreateUserName, 
                t.F_ModifyDate, 
                t.F_ModifyUserId, 
                t.F_ModifyUserName 
            ";
        }
        #endregion

        #region 获取数据
        /// <summary>
        /// 获取自定义流程列表
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <returns></returns>
        public IEnumerable<WfSchemeInfoEntity> GetCustmerSchemeInfoList(UserInfo userInfo, string F_Category)
        {
            try
            {
                string userId = userInfo.userId;
                string postIds = userInfo.postIds;
                string roleIds = userInfo.roleIds;
                List<WfSchemeAuthorizeEntity> list = (List<WfSchemeAuthorizeEntity>)this.BaseRepository().FindList<WfSchemeAuthorizeEntity>(t => t.F_ObjectId == null
                    || userId.Contains(t.F_ObjectId)
                    || postIds.Contains(t.F_ObjectId)
                    || roleIds.Contains(t.F_ObjectId)
                    );
                string schemeinfoIds = "";
                foreach (var item in list)
                {
                    schemeinfoIds += "'" + item.F_SchemeInfoId + "',";
                }
                schemeinfoIds = "(" + schemeinfoIds.Remove(schemeinfoIds.Length - 1, 1) + ")";

                var strSql = new StringBuilder();
                strSql.Append("SELECT ");
                strSql.Append(schemeInfoFieldSql);
                strSql.Append(" FROM LR_WF_SchemeInfo t WHERE 1=1 AND t.F_DeleteMark = 0 AND t.F_EnabledMark = 1  AND t.F_Kind = 1 AND t.F_Id in " + schemeinfoIds);
                if (!F_Category.IsEmpty())
                {
                    strSql.Append(" AND F_Category='" + F_Category + "'");
                }
                return this.BaseRepository().FindList<WfSchemeInfoEntity>(strSql.ToString());
            }
            catch (Exception ex)
            {
                if (ex is ExceptionEx)
                {
                    throw;
                }
                else
                {
                    throw ExceptionEx.ThrowServiceException(ex);
                }
            }
        }
        /// <summary>
        /// 获取委托人信息列表
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private List<UserInfo> GetDelegateTask(string userId)
        {
            try
            {
                List<UserInfo> list = wfTaskService.GetDelegateTask(userId);
                List<Base.AuthorizeModule.UserRelationEntity> rlist;
                foreach (var item in list)
                {
                    UserEntity userEntity = userService.GetEntity(item.userId);
                    item.companyId = userEntity.F_CompanyId;
                    item.departmentId = userEntity.F_DepartmentId;
                    rlist = (List<Base.AuthorizeModule.UserRelationEntity>)userRelationService.GetObjectIdList(userEntity.F_UserId, 1);
                    item.roleIds = rlist != null ? string.Join(",", rlist.Select(x => x.F_ObjectId)) : "";
                    rlist = (List<Base.AuthorizeModule.UserRelationEntity>)userRelationService.GetObjectIdList(userEntity.F_UserId, 2);
                    item.postIds = rlist != null ? string.Join(",", rlist.Select(x => x.F_ObjectId)) : "";
                }
                return list;
            }
            catch (Exception ex)
            {
                if (ex is ExceptionEx)
                {
                    throw;
                }
                else
                {
                    throw ExceptionEx.ThrowServiceException(ex);
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
                /**
                 *此业务参考 Learun.Application.WorkFlow =》WfTask =》 WfTaskBLL =》 GetActiveList方法
                 */

                //委托人
                List<UserInfo> delegateList = GetDelegateTask(userInfo.userId);
                string userId = userInfo.userId;
                string postIds = "'" + userInfo.postIds.Replace(",", "','") + "'";
                string roleIds = "'" + userInfo.roleIds.Replace(",", "','") + "'";
                string companyId = userInfo.companyId;
                string departmentId = userInfo.departmentId;


                var strSql = new StringBuilder();
                // 虚拟参数
                var dp = new DynamicParameters(new { });

                strSql.Append(@"SELECT " + taskFieldSql + @"
                                    FROM
	                                    LR_WF_Task t WHERE t.F_IsFinished = 0 ");
                strSql.Append(" AND ( t.F_AuditorId = '1' OR ");
                strSql.Append("  t.F_AuditorId = @t_userId ");
                if (!string.IsNullOrEmpty(userInfo.postIds))
                {
                    strSql.Append("  OR (t.F_AuditorId in (" + postIds + ") AND t.F_CompanyId = '1' AND t.F_DepartmentId = '1' ) ");
                    strSql.Append("  OR (t.F_AuditorId in (" + postIds + ") AND t.F_CompanyId = @t_companyId ) ");
                    strSql.Append("  OR (t.F_AuditorId in (" + postIds + ") AND t.F_DepartmentId = @t_departmentId ) ");
                }
                if (!string.IsNullOrEmpty(userInfo.roleIds))
                {
                    strSql.Append("  OR (t.F_AuditorId in (" + roleIds + ") AND t.F_CompanyId = '1' AND t.F_DepartmentId = '1' ) ");
                    strSql.Append("  OR (t.F_AuditorId in (" + roleIds + ") AND t.F_CompanyId = @t_companyId ) ");
                    strSql.Append("  OR (t.F_AuditorId in (" + roleIds + ") AND t.F_DepartmentId = @t_departmentId) ");
                }
                // 添加委托信息
                foreach (var item in delegateList)
                {
                    string processId = "'" + item.wfProcessId.Replace(",", "','") + "'";
                    string postIds2 = "'" + item.postIds.Replace(",", "','") + "'";
                    string roleIds2 = "'" + item.roleIds.Replace(",", "','") + "'";
                    string userI2 = "'" + item.userId + "'";
                    string companyId2 = "'" + item.companyId + "'";
                    string departmentId2 = "'" + item.departmentId + "'";

                    strSql.Append("  OR (t.F_AuditorId =" + userI2 + " AND t.F_ProcessId in (" + processId + ") )");

                    if (!string.IsNullOrEmpty(item.postIds))
                    {
                        strSql.Append("  OR (t.F_AuditorId in (" + postIds2 + ") AND t.F_CompanyId = '1' AND t.F_DepartmentId = '1' AND t.F_ProcessId in (" + processId + ")  ) ");
                        strSql.Append("  OR (t.F_AuditorId in (" + postIds2 + ") AND t.F_CompanyId = " + companyId2 + " AND t.F_ProcessId in (" + processId + ") ) ");
                        strSql.Append("  OR (t.F_AuditorId in (" + postIds2 + ") AND t.F_DepartmentId = " + departmentId2 + " AND t.F_ProcessId in (" + processId + ") ) ");
                    }

                    if (!string.IsNullOrEmpty(item.roleIds))
                    {
                        strSql.Append("  OR (t.F_AuditorId in (" + roleIds2 + ") AND t.F_CompanyId = '1' AND t.F_DepartmentId = '1' AND t.F_ProcessId in (" + processId + ")  ) ");
                        strSql.Append("  OR (t.F_AuditorId in (" + roleIds2 + ") AND t.F_CompanyId = " + companyId2 + " AND t.F_ProcessId in (" + processId + ") ) ");
                        strSql.Append("  OR (t.F_AuditorId in (" + roleIds2 + ") AND t.F_DepartmentId = " + departmentId2 + " AND t.F_ProcessId in (" + processId + ") ) ");
                    }
                }
                strSql.Append(")");
                //关联具体流程实例
                strSql.Append(" AND t.F_ProcessId=@F_ProcessId");
                strSql.Append(" ORDER BY t.F_CreateDate DESC");

                dp.Add("t_userId", userId, DbType.String);
                dp.Add("t_companyId", companyId, DbType.String);
                dp.Add("t_departmentId", departmentId, DbType.String);
                dp.Add("F_ProcessId", ProcessId, DbType.String);

                return this.BaseRepository().FindList<WfTaskEntity>(strSql.ToString(), dp);
            }
            catch (Exception ex)
            {
                if (ex is ExceptionEx)
                {
                    throw;
                }
                else
                {
                    throw ExceptionEx.ThrowServiceException(ex);
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
                var model = this.BaseRepository().FindEntity<WfProcessInstanceEntity>(t => t.F_Id == ProcessId);
                if (model == null) { throw new ExceptionEx("流程结束状态获取失败", null); }
                return model.F_IsFinished == 1;
            }
            catch (Exception ex)
            {
                if (ex is ExceptionEx)
                {
                    throw;
                }
                else
                {
                    throw ExceptionEx.ThrowServiceException(ex);
                }
            }
        }
        #endregion

        #region 帮助工具
        /// <summary>
        /// 追加待办任务的Sql语句
        /// </summary>
        /// <param name="strSql">具体业务的SQL语句</param>
        /// <param name="parameters">具体业务的参数化</param>
        /// <param name="userInfo">用户信息</param>
        /// <param name="pagination">翻页信息</param>
        /// <param name="queryJson">查询条件</param>
        /// <param name="F_Id">具体业务表对应的流程实例字段</param>
        /// <param name="M_Alias">主表的别名，默认t</param>
        /// <returns></returns>
        public void AppendActiveSql(StringBuilder strSql, DynamicParameters parameters, UserInfo userInfo, string queryJson, string F_Id = "F_ProcessId", string M_Alias = "t")
        {
            try
            {
                /**
                 *此业务参考 Learun.Application.WorkFlow =》WfTask =》 WfTaskBLL =》 GetActiveList方法
                 */

                //委托人
                List<UserInfo> delegateList = GetDelegateTask(userInfo.userId);
                string userId = userInfo.userId;
                string postIds = "'" + userInfo.postIds.Replace(",", "','") + "'";
                string roleIds = "'" + userInfo.roleIds.Replace(",", "','") + "'";
                string companyId = userInfo.companyId;
                string departmentId = userInfo.departmentId;

                // 获取委托信息
                strSql.Append(@" AND EXISTS ( SELECT
	                                p.F_Id
                                FROM
                                (
                                    SELECT
	                                   MAX(t.F_Id) AS F_TaskId,
			                           MAX(t.F_TaskType) AS F_TaskType,
			                           t.F_ProcessId,
			                           t.F_NodeName AS F_TaskName
                                    FROM
	                                    LR_WF_Task t WHERE t.F_IsFinished = 0 ");
                strSql.Append(" AND ( t.F_AuditorId = '1' OR ");
                strSql.Append("  t.F_AuditorId = @t_userId ");
                if (!string.IsNullOrEmpty(userInfo.postIds))
                {
                    strSql.Append("  OR (t.F_AuditorId in (" + postIds + ") AND t.F_CompanyId = '1' AND t.F_DepartmentId = '1' ) ");
                    strSql.Append("  OR (t.F_AuditorId in (" + postIds + ") AND t.F_CompanyId = @t_companyId ) ");
                    strSql.Append("  OR (t.F_AuditorId in (" + postIds + ") AND t.F_DepartmentId = @t_departmentId ) ");
                }
                if (!string.IsNullOrEmpty(userInfo.roleIds))
                {
                    strSql.Append("  OR (t.F_AuditorId in (" + roleIds + ") AND t.F_CompanyId = '1' AND t.F_DepartmentId = '1' ) ");
                    strSql.Append("  OR (t.F_AuditorId in (" + roleIds + ") AND t.F_CompanyId = @t_companyId ) ");
                    strSql.Append("  OR (t.F_AuditorId in (" + roleIds + ") AND t.F_DepartmentId = @t_departmentId) ");
                }
                // 添加委托信息
                foreach (var item in delegateList)
                {
                    string processId = "'" + item.wfProcessId.Replace(",", "','") + "'";
                    string postIds2 = "'" + item.postIds.Replace(",", "','") + "'";
                    string roleIds2 = "'" + item.roleIds.Replace(",", "','") + "'";
                    string userI2 = "'" + item.userId + "'";
                    string companyId2 = "'" + item.companyId + "'";
                    string departmentId2 = "'" + item.departmentId + "'";

                    strSql.Append("  OR (t.F_AuditorId =" + userI2 + " AND t.F_ProcessId in (" + processId + ") )");

                    if (!string.IsNullOrEmpty(item.postIds))
                    {
                        strSql.Append("  OR (t.F_AuditorId in (" + postIds2 + ") AND t.F_CompanyId = '1' AND t.F_DepartmentId = '1' AND t.F_ProcessId in (" + processId + ")  ) ");
                        strSql.Append("  OR (t.F_AuditorId in (" + postIds2 + ") AND t.F_CompanyId = " + companyId2 + " AND t.F_ProcessId in (" + processId + ") ) ");
                        strSql.Append("  OR (t.F_AuditorId in (" + postIds2 + ") AND t.F_DepartmentId = " + departmentId2 + " AND t.F_ProcessId in (" + processId + ") ) ");
                    }

                    if (!string.IsNullOrEmpty(item.roleIds))
                    {
                        strSql.Append("  OR (t.F_AuditorId in (" + roleIds2 + ") AND t.F_CompanyId = '1' AND t.F_DepartmentId = '1' AND t.F_ProcessId in (" + processId + ")  ) ");
                        strSql.Append("  OR (t.F_AuditorId in (" + roleIds2 + ") AND t.F_CompanyId = " + companyId2 + " AND t.F_ProcessId in (" + processId + ") ) ");
                        strSql.Append("  OR (t.F_AuditorId in (" + roleIds2 + ") AND t.F_DepartmentId = " + departmentId2 + " AND t.F_ProcessId in (" + processId + ") ) ");
                    }
                }


                strSql.Append(@" ) GROUP BY
	                                t.F_ProcessId,
	                                t.F_NodeId,
	                                t.F_NodeName )a LEFT JOIN LR_WF_ProcessInstance p ON p.F_Id = a.F_ProcessId WHERE 1=1 AND (p.F_IsFinished = 0 OR a.F_TaskType = 3) AND p.F_EnabledMark = 1 ");

                strSql.Append(" AND p.F_Id=" + M_Alias + ".F_ProcessId )");

                parameters.Add("t_userId", userId, DbType.String);
                parameters.Add("t_companyId", companyId, DbType.String);
                parameters.Add("t_departmentId", departmentId, DbType.String);
            }
            catch (Exception ex)
            {
                if (ex is ExceptionEx)
                {
                    throw;
                }
                else
                {
                    throw ExceptionEx.ThrowServiceException(ex);
                }
            }
        }
        #endregion
    }
}
