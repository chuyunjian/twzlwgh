using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Learun.Util;
namespace Learun.DataBase.Util
{
    /// <summary>
    /// 数据权限帮扶类
    /// </summary>
    public class DataPermission
    {
        /// <summary>
        /// 向当前SQL中追加权限语句
        /// </summary>
        /// <param name="strSql">SQL语句</param>
        /// <param name="dp">SQL参数</param>
        /// <param name="MainAlias">主表别名</param>
        /// <param name="CompanyField">单位ID的存储字段，默认F_CompanyId</param>
        /// <param name="DepartmentField">部门的存储字段，默认F_DepartmentId</param>
        /// <param name="AreaField">行政区存储字段，默认F_AreaID</param>
        /// <remarks>
        /// 实际的数据权限过滤规则应与需求对应
        /// </remarks>
        public static void AppendSql(StringBuilder strSql, DynamicParameters dp = null, string MainAlias = "t", string CompanyField = "F_CompanyId", string DepartmentField = "F_DepartmentId", string AreaField = "F_AreaId")
        {
            var user = LoginUserInfo.Get();
            if (user != null)
            {
                //排除超级管理员
                if (!user.isSystem)
                {
                    if (user.companyId.IsEmpty()) { throw new ExceptionEx("用户未设置所属单位", null); }
                    strSql.Append(@" AND " + MainAlias + ".F_CompanyId IN('" + user.companyId + "')");
                }
            }
            else
            {
                throw new ExceptionEx("数据权限过滤失败", null);
            }
        }
    }
}
