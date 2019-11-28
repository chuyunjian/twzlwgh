using Learun.Application.AppMagager;
using Learun.Application.Organization;
using Learun.Util;
using Learun.Util.Login;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Linq;

namespace Learun.Application.Web.SYS_Code
{
    /// <summary>
    /// 用户相关接口
    /// </summary>
    [RoutePrefix("rest/user")]
    public class UserController : BaseApi
    {
        private UserIBLL userIBLL = new UserBLL();
        private DepartmentIBLL departmentIBLL = new DepartmentBLL();

        /*
         查询使用GET请求，新增、编辑、删除请使用POST请求
        */

        #region 用户登录
        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="Account">账号</param>
        /// <param name="Password">密码</param>
        /// <param name="LoginType">登录身份（0：申请人 10：审批人 20：调度 30：司机）</param>
        /// <returns></returns>
        [Route("login")]
        [HttpPost]
        [ApiAuthorize(FilterMode.Ignore)]
        public ResParameter Login([FromBody]JObject data)
        {
            try
            {
                if (data["LoginType"].IsEmpty()) { throw new ExceptionEx("缺少登录类型", null); }
                var test = data.ToJson();
                string Account = data["Account"].ToString();
                string Password = data["Password"].ToString();
                LoginIdentity LoginType = (LoginIdentity)data["LoginType"].ToInt();
                string loginMsg = "";
                List<object> datas = new List<object>();
                UserInfo userInfo = LoginHelper.Instance.LoginUser(LoginType, LoginMode.APP, Account, Password, out loginMsg);


                List<FunctionEntity> functions = new List<FunctionEntity>();
                FunctionIBLL functionIBLL = new FunctionBLL();
                functions = functionIBLL.GetList(LoginUser).ToList();
                if (userInfo == null)
                {
                    throw new Exception(loginMsg);
                }
                else
                {
                    datas = new List<object>() {
                        new {
                            UserID=userInfo.userId,
                            UserName=userInfo.realName,
                            userInfo.headIcon,
                            LoginType=LoginType,
                            LoginTime=DateTime.Now,
                            Token=userInfo.token,
                            Menus=from m in functions
                                  select new {
                                       m.F_Name,//名字
                                       m.F_Code,//编码
                                       F_Img=m.HTTP_F_Img,//图片
                                       m.F_Url,//地址
                                  }
                        }
                    };
                }
                return Success(datas);
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }
        #endregion

        #region 用户密码修改
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="OldPassword">旧密码</param>
        /// <param name="NewPassword">新密码</param>
        /// <returns></returns>
        [Route("changePassword")]
        [HttpPost]
        public ResParameter ChangePassword([FromBody]JObject data)
        {
            try
            {
                var test = data.ToJson();
                string OldPassword = data["OldPassword"].ToString();
                string NewPassword = data["NewPassword"].ToString();
                if (OldPassword.IsEmpty()) { throw new Exception("请输入旧密码"); }
                if (NewPassword.IsEmpty()) { throw new Exception("请输入新密码"); }
                LoginIdentity LoginType = (LoginIdentity)LoginUser.LoginType;
                switch (LoginType)
                {
                    case LoginIdentity.System:
                        if (!userIBLL.RevisePassword(NewPassword, OldPassword))
                        {
                            throw new Exception("旧密码错误");
                        }
                        break;
                    default:
                        break;
                }
                return Success("成功");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }
        #endregion

        #region 用户获取
        /// <summary>
        /// 通过用户ID和登录类型得到用户信息
        /// </summary>
        /// <param name="UserID">用户ID</param>
        /// <param name="LoginType">登录类型</param>
        /// <returns></returns>
        [Route("detail")]
        [HttpGet]
        public ResParameter Detail(string UserID, int LoginType = 0)
        {
            try
            {
                var temp = LoginUser;
                List<UserInfo> datas = new List<UserInfo>();
                UserInfo userInfo = null;
                switch (LoginType)
                {
                    case 0://系统用户
                        var user = userIBLL.GetEntityByUserId(UserID);
                        if (user != null)
                        {
                            userInfo = user.MapToUserInfo;
                        }
                        break;
                    case 1://司机
                        //var cadre = new BF_CadreBLL().GetEntity(UserID);
                        //if (cadre != null)
                        //{
                        //    userInfo = cadre.MapToUserInfo;
                        //}
                        break;
                    default:
                        break;
                }
                if (userInfo != null) { userInfo.LoginType = LoginType; datas.Add(userInfo); }
                if (datas.Count == 0) { throw new Exception("用户不存在"); }
                return Success(datas);
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }
        #endregion

        #region 修改用户
        /// <summary>
        /// 修改管理员用户信息
        /// </summary>
        /// <param name="UserID">用户ID</param>
        /// <param name="LoginType">登录类型</param>
        /// <returns></returns>
        [Route("modify")]
        [HttpPost]
        public ResParameter Modify(UserEntity data)
        {
            try
            {
                var test = new
                {
                    data
                };
                return Success(test);
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }
        #endregion
    }
}
