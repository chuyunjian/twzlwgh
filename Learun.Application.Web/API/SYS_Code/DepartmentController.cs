using Learun.Application.Organization;
using Learun.Util;
using Newtonsoft.Json;
using System;
using System.Web.Http;
using System.Linq;
using System.Collections.Generic;

namespace Learun.Application.Web.SYS_Code
{
    /// <summary>
    /// 部门相关接口
    /// </summary>
    [RoutePrefix("rest/department")]
    public class DepartmentController : BaseApi
    {
        private DepartmentIBLL departmentIBLL = new DepartmentBLL();
        [Route("list"), HttpGet]
        public ResParameter GetList(string companyId = "", string parentId = "0", bool filter = true)
        {
            try
            {
                var user = LoginUser;
                List<TreeModel> data = new List<TreeModel>();
                if (string.IsNullOrEmpty(companyId) && user.isSystem)
                {
                    CompanyIBLL companyIBLL = new CompanyBLL();
                    var companylist = companyIBLL.GetList();
                    data = departmentIBLL.GetTree(companylist);
                }
                else if (companyId.IsEmpty())
                {
                    data = departmentIBLL.GetTree(user.companyId, parentId, filter);
                }
                else
                {
                    data = departmentIBLL.GetTree(companyId, parentId, filter);
                }
                var jsonData = from m in data
                               select new
                               {
                                   m.id,
                                   m.text,
                                   m.parentId,
                                   m.hasChildren
                               };
                return Success(jsonData);
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }
    }
}
