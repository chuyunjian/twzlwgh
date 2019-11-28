using Learun.Application.Base.AuthorizeModule;
using Learun.Application.Base.SystemModule;
using Learun.Application.Organization;
using Learun.Cache.Base;
using Learun.Cache.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Learun.Util.Login
{
    /// <summary>
    /// 登录帮助类
    /// </summary>
    public class LoginHelper
    {
        #region 基础数据类
        private UserIBLL userIBLL = new UserBLL();
        private UserRelationIBLL userRelationIBLL = new UserRelationBLL();
        private AreaIBLL areaIBLL = new AreaBLL();
        private CompanyIBLL companyIBLL = new CompanyBLL();
        private DepartmentIBLL departmentIBLL = new DepartmentBLL();

        #endregion

        #region 缓存相关
        /// <summary>
        /// 缓存操作类
        /// </summary>
        private ICache redisCache = CacheFactory.CaChe();
        private string cacheKeyOperator = "learun_adms_operator_";// +登录者token
        private string cacheKeyToken = "learun_adms_token_";// +登录者token
        private string cacheKeyError = "learun_adms_error_";// + Mark
        private string cacheKeyUserInfo = "learun_adms_userInfo_";// +登录者用户信息
        /// <summary>
        /// 秘钥
        /// </summary>
        private string LoginUserToken = "Learun_ADMS_V7_Token";
        /// <summary>
        /// 标记登录的浏览器
        /// </summary>
        private string LoginUserMarkKey = "Learun_ADMS_V7_Mark";
        /// <summary>
        /// 获取实例
        /// </summary>
        public static LoginHelper Instance
        {
            get { return new LoginHelper(); }
        }

        #endregion

        #region 获取浏览器设配号
        /// <summary>
        /// 获取浏览器设配号
        /// </summary>
        /// <returns></returns>
        public string GetMark()
        {
            string cookieMark = WebHelper.GetCookie(LoginUserMarkKey).ToString();
            if (string.IsNullOrEmpty(cookieMark))
            {
                cookieMark = Guid.NewGuid().ToString();
                WebHelper.WriteCookie(LoginUserMarkKey, cookieMark);
            }
            return cookieMark;
        }
        #endregion

        #region 清空当前登录信息
        /// <summary>
        /// 清空当前登录信息
        /// </summary>
        public void EmptyCurrent()
        {
            try
            {
                WriteLog("EmptyCurrent");
                string token = WebHelper.GetCookie(LoginUserToken).ToString();
                string loginMark = WebHelper.GetCookie(LoginUserMarkKey).ToString();
                EmptyCurrent(token, loginMark);
            }
            catch (Exception)
            {
            }
        }
        /// <summary>
        /// 清空当前登录信息
        /// </summary>
        /// <param name="token">登录票据</param>
        /// <param name="loginMark">登录设备标识</param>
        public void EmptyCurrent(string token, string loginMark)
        {
            try
            {



                LoginUser operatorInfo = redisCache.Read<LoginUser>(cacheKeyOperator + loginMark, CacheId.loginInfo);
                if (operatorInfo != null && operatorInfo.token == token)
                {
                    WriteLog("EmptyCurrent:" + cacheKeyToken + operatorInfo.account);
                    Dictionary<string, string> tokenMarkList = redisCache.Read<Dictionary<string, string>>(cacheKeyToken + operatorInfo.account, CacheId.loginInfo);
                    tokenMarkList.Remove(loginMark);
                    redisCache.Remove(cacheKeyOperator + loginMark, CacheId.loginInfo);
                    redisCache.Write<Dictionary<string, string>>(cacheKeyToken + operatorInfo.account, tokenMarkList, CacheId.loginInfo);
                }
            }
            catch (Exception)
            {
            }
        }
        #endregion

        #region 登录用户缓存相关
        /// <summary>
        /// 登录者信息添加到缓存中
        /// </summary>
        /// <param name="account">账号</param>
        /// <param name="appId">应用id</param>
        /// <param name="loginMark">设备标识</param>
        /// <param name="cookie">是否保存cookie，默认是</param>
        /// <returns></returns>
        public string AddLoginUser(LoginIdentity loginIdentity, LoginMode loginMode, string account, string appId, string loginMark, bool cookie = true)
        {
            string token = Guid.NewGuid().ToString();
            try
            {
                string cacheKey = cacheKeyToken + account + "_" + loginIdentity.ToString() + "_" + loginMode.ToString();
                // 填写登录信息
                LoginUser operatorInfo = new LoginUser();
                operatorInfo.appId = appId;
                operatorInfo.account = account;
                operatorInfo.logTime = DateTime.Now;
                operatorInfo.iPAddress = Net.Ip;
                operatorInfo.browser = Net.Browser;
                operatorInfo.token = token;
                operatorInfo.loginIdentity = loginIdentity;
                operatorInfo.loginMode = loginMode;
                if (cookie)
                {
                    string cookieMark = WebHelper.GetCookie(LoginUserMarkKey).ToString();
                    if (string.IsNullOrEmpty(cookieMark))
                    {
                        operatorInfo.loginMark = Guid.NewGuid().ToString();
                        WebHelper.WriteCookie(LoginUserMarkKey, operatorInfo.loginMark);
                    }
                    else
                    {
                        operatorInfo.loginMark = cookieMark;
                    }
                    WebHelper.WriteCookie(LoginUserToken, token);
                }
                else
                {
                    operatorInfo.loginMark = loginMark;
                }
                Dictionary<string, string> tokenMarkList = redisCache.Read<Dictionary<string, string>>(cacheKey, CacheId.loginInfo);
                if (tokenMarkList == null)// 此账号第一次登录
                {
                    tokenMarkList = new Dictionary<string, string>();
                    tokenMarkList.Add(operatorInfo.loginMark, token);
                }
                else
                {
                    if (tokenMarkList.ContainsKey(operatorInfo.loginMark))
                    {
                        tokenMarkList[operatorInfo.loginMark] = token;
                    }
                    else
                    {
                        tokenMarkList.Add(operatorInfo.loginMark, token);
                    }
                }
                //设置过期时间为3天，超过三天未操作，需要重新登录
                redisCache.Write<Dictionary<string, string>>(cacheKey, tokenMarkList, DateTime.Now.AddDays(3), CacheId.loginInfo);
                redisCache.Write<LoginUser>(cacheKeyOperator + operatorInfo.loginMark, operatorInfo, DateTime.Now.AddDays(3), CacheId.loginInfo);

                return token;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// 判断登录状态
        /// </summary>
        /// <returns>-1未登录,1登录成功,0登录过期</returns>
        public LoginResult IsOnLine()
        {
            try
            {
                string token = WebHelper.GetCookie(LoginUserToken).ToString();
                string loginMark = WebHelper.GetCookie(LoginUserMarkKey).ToString();
                return IsOnLine(token, loginMark);
            }
            catch (Exception)
            {
                return new LoginResult { stateCode = -1 };
            }
        }
        /// <summary>
        /// 判断登录状态
        /// </summary>
        /// <param name="token">登录票据</param>
        /// <param name="loginMark">登录设备标识</param>
        /// <param name="loginType">登录类型</param>
        /// <returns>-1未登录,1登录成功,0登录过期</returns>
        public LoginResult IsOnLine(string token, string loginMark)
        {
            LoginResult operatorResult = new LoginResult();
            operatorResult.stateCode = -1; // -1未登录,1登录成功,0登录过期
            try
            {
                if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(loginMark))
                {
                    return operatorResult;
                }
                LoginUser operatorInfo = redisCache.Read<LoginUser>(cacheKeyOperator + loginMark, CacheId.loginInfo);
                if (operatorInfo != null && operatorInfo.token == token)
                {
                    //token缓存键
                    string cacheKey = cacheKeyToken + operatorInfo.account + "_" + (int)operatorInfo.loginIdentity + "_" + (int)operatorInfo.loginMode;
                    //用户缓存键
                    string cacheUserKey = cacheKeyToken + "_UserInfo_" + operatorInfo.account + "_" + (int)operatorInfo.loginIdentity + "_" + (int)operatorInfo.loginMode;
                    //取用户缓存信息
                    LoginResult op = redisCache.Read<LoginResult>(cacheUserKey, CacheId.operatorresult);
                    if (op == null)
                    {
                        UserInfo userInfo = new UserInfo();
                        switch (operatorInfo.loginIdentity)
                        {
                            case LoginIdentity.System:
                                #region 系统用户信息
                                UserEntity userEntity = userIBLL.GetEntityByAccount(operatorInfo.account);
                                if (userEntity != null)
                                {
                                    userInfo = userEntity.MapToUserInfo;
                                    userInfo.isSystem = userEntity.F_SecurityLevel == 1 ? true : false;

                                    userInfo.roleIds = userRelationIBLL.GetObjectIds(userEntity.F_UserId, 1);
                                    RoleIBLL roleIBLL = new RoleBLL();
                                    userInfo.roles = !userInfo.roleIds.IsEmpty() ? roleIBLL.GetListByRoleIds(userInfo.roleIds).ToList() : new List<RoleEntity>();
                                    userInfo.postIds = userRelationIBLL.GetObjectIds(userEntity.F_UserId, 2);
                                    userInfo.companyIds = companyIBLL.GetSubNodes(userEntity.F_CompanyId);
                                    userInfo.departmentIds = departmentIBLL.GetSubNodes(userEntity.F_CompanyId, userEntity.F_DepartmentId);

                                    operatorResult.stateCode = 1;
                                }
                                else
                                {
                                    operatorResult.stateCode = 0;
                                }
                                #endregion
                                break;
                            default:
                                break;
                        }
                        var company = companyIBLL.GetEntity(userInfo.companyId);
                        if (company != null)
                        {
                            userInfo.companyName = company.F_FullName;
                        }
                        var department = departmentIBLL.GetEntity(userInfo.departmentId);
                        if (department != null)
                        {
                            userInfo.departmentName = department.F_FullName;
                        }
                        userInfo.appId = operatorInfo.appId;
                        userInfo.logTime = operatorInfo.logTime;
                        userInfo.iPAddress = operatorInfo.iPAddress;
                        userInfo.browser = operatorInfo.browser;
                        userInfo.loginMark = operatorInfo.loginMark;
                        userInfo.token = operatorInfo.token;
                        userInfo.account = operatorInfo.account;
                        userInfo.loginMode = operatorInfo.loginMode.GetValue();
                        userInfo.LoginType = (int)operatorInfo.loginIdentity;
                        if (operatorResult.stateCode == 1)
                        {
                            operatorResult.userInfo = userInfo;
                            //加入用户信息缓存，用户信息保存6小时，超过时间重新获取
                            redisCache.Write<LoginResult>(cacheUserKey, operatorResult, DateTime.Now.AddHours(6), CacheId.operatorresult);
                            if (HttpContext.Current != null)
                            {
                                HttpContext.Current.Items.Add("LoginUserInfo", userInfo);
                            }
                        }
                    }
                    else
                    {
                        operatorResult = op;
                        if (HttpContext.Current != null)
                        {
                            HttpContext.Current.Items.Add("LoginUserInfo", op.userInfo);
                        }
                    }
                    //重新设置登录标识的过期时间
                    Dictionary<string, string> tokenMarkList = redisCache.Read<Dictionary<string, string>>(cacheKey, CacheId.loginInfo);
                    redisCache.Write<Dictionary<string, string>>(cacheKey, tokenMarkList, DateTime.Now.AddDays(3), CacheId.loginInfo);
                    redisCache.Write<LoginUser>(cacheKeyOperator + operatorInfo.loginMark, operatorInfo, DateTime.Now.AddDays(3), CacheId.loginInfo);
                }
                return operatorResult;
            }
            catch (Exception ex)
            {
                return operatorResult;
            }
        }
        #endregion

        #region 用户登录
        /// <summary>
        /// 通用用户登录
        /// </summary>
        /// <param name="loginIdentity">登录身份</param>
        /// <param name="loginMode">登录方式</param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public UserInfo LoginUser(LoginIdentity loginIdentity, LoginMode loginMode, string username, string password, out string msg)
        {
            LogEntity logEntity = new LogEntity();//日志
            UserInfo userInfo = null;//登录用户信息
            try
            {
                msg = "";
                string token = "";
                string guid = "";
                string loginMark = null;
                if (loginMode == LoginMode.APP)
                {
                    loginMark = Md5Helper.Encrypt(Net.Ip + "_" + username + "_" + loginIdentity.ToString() + "_" + loginMode.ToString(), 32);//登录设备唯一标识
                }
                /**
                 * 需要去IsOnLine方法中设置缓存用户信息
                 */
                switch (loginIdentity)
                {
                    case LoginIdentity.System:
                        #region Admin用户的登录操作
                        UserEntity userEntity = userIBLL.CheckLogin(username, password);
                        switch (loginMode)
                        {
                            case LoginMode.PC:
                                if (!userEntity.LoginOk)//登录失败
                                {
                                    int num = LoginHelper.Instance.AddCurrentErrorNum();
                                }
                                else
                                {
                                    LoginHelper.Instance.ClearCurrentErrorNum();
                                }
                                break;
                            case LoginMode.APP:
                                if (!userEntity.LoginOk)//登录失败
                                {
                                    throw new ExceptionEx(userEntity.LoginMsg, null);
                                }
                                break;
                            default:
                                break;
                        }
                        if (!userEntity.LoginOk)//登录失败
                        {
                            //写入日志
                            logEntity.F_ExecuteResult = 0;
                            logEntity.F_ExecuteResultJson = "登录失败:" + userEntity.LoginMsg;
                            throw new Exception(userEntity.LoginMsg);
                        }
                        else
                        {
                            userInfo = userEntity.MapToUserInfo;
                            guid = AddLoginUser(loginIdentity, loginMode, userEntity.F_Account, "Learun_ADMS_V7_PC", loginMark, loginMode == LoginMode.PC);//写入缓存信息
                            logEntity.F_ExecuteResult = 1;
                            logEntity.F_ExecuteResultJson = "登录成功";
                        }
                        #endregion
                        break;
                    default:
                        throw new Exception("请选择登录身份");
                }
                switch (loginMode)
                {
                    case LoginMode.PC:
                        break;
                    case LoginMode.APP:
                        #region 得到登录用户token
                        var payload = new Dictionary<string, object>
                                    {
                                        {"guid", guid },
                                        {"account", username },
                                        {"loginType", (Int32)loginIdentity},
                                        {"loginIdentity", loginIdentity.ToString()},
                                        {"time",DateTime.Now.ToDateTimeString() }
                                    };
                        token = JwtHelp.SetJwtEncode(payload);
                        userInfo.loginMark = loginMark;
                        #endregion
                        break;
                    default:
                        break;
                }
                var cache = IsOnLine(guid, loginMark);
                if (cache.stateCode == 1)
                {
                    userInfo = cache.userInfo;
                }
                userInfo.token = token;
                return userInfo;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return null;
            }
            finally
            {
                #region 写入日志
                logEntity.F_ExecuteResultJson = loginIdentity.GetDescription() + loginMode.GetDescription() + "。" + logEntity.F_ExecuteResultJson;
                logEntity.F_CategoryId = 1;
                logEntity.F_OperateTypeId = ((int)LoginType.Login).ToString();
                logEntity.F_OperateType = EnumAttribute.GetDescription(LoginType.Login);
                if (userInfo != null)
                {
                    logEntity.F_OperateAccount = username + "(" + userInfo.realName + ")";
                    logEntity.F_OperateUserId = !string.IsNullOrEmpty(userInfo.userId) ? userInfo.userId : username;
                }
                else
                {
                    logEntity.F_OperateAccount = username;
                    logEntity.F_OperateUserId = username;
                }
                logEntity.F_Module = Config.GetValue("SoftName");
                logEntity.WriteLog();
                #endregion
            }
        }
        #endregion

        #region 登录错误次数记录
        /// <summary>
        /// 获取当前登录错误次数
        /// </summary>
        /// <returns></returns>
        public int GetCurrentErrorNum()
        {
            int res = 0;
            try
            {
                string cookieMark = WebHelper.GetCookie(LoginUserMarkKey).ToString();
                if (string.IsNullOrEmpty(cookieMark))
                {
                    cookieMark = Guid.NewGuid().ToString();
                    WebHelper.WriteCookie(LoginUserMarkKey, cookieMark);
                }
                string num = redisCache.Read<string>(cacheKeyError + cookieMark, CacheId.loginInfo);
                if (!string.IsNullOrEmpty(num))
                {
                    res = Convert.ToInt32(num);
                }
            }
            catch (Exception)
            {
            }
            return res;
        }
        /// <summary>
        /// 增加错误次数
        /// </summary>
        /// <returns></returns>
        public int AddCurrentErrorNum()
        {
            int res = 0;
            try
            {
                string cookieMark = WebHelper.GetCookie(LoginUserMarkKey).ToString();
                if (string.IsNullOrEmpty(cookieMark))
                {
                    cookieMark = Guid.NewGuid().ToString();
                    WebHelper.WriteCookie(LoginUserMarkKey, cookieMark);
                }
                string num = redisCache.Read<string>(cacheKeyError + cookieMark, CacheId.loginInfo);
                if (!string.IsNullOrEmpty(num))
                {
                    res = Convert.ToInt32(num);
                }
                res++;
                num = res + "";
                redisCache.Write<string>(cacheKeyError + cookieMark, num, CacheId.loginInfo);
            }
            catch (Exception)
            {
            }
            return res;
        }
        /// <summary>
        /// 清除当前登录错误次数
        /// </summary>
        public void ClearCurrentErrorNum()
        {
            try
            {
                string cookieMark = WebHelper.GetCookie(LoginUserMarkKey).ToString();
                if (string.IsNullOrEmpty(cookieMark))
                {
                    cookieMark = Guid.NewGuid().ToString();
                    WebHelper.WriteCookie(LoginUserMarkKey, cookieMark);
                }
                redisCache.Remove(cacheKeyError + cookieMark, CacheId.loginInfo);
            }
            catch (Exception)
            {
            }
        }
        #endregion

        #region 写入操作日志
        /// <summary>
        /// 写操作日志
        /// </summary>
        public void WriteOperateLog(LoginLogModel operateLogModel)
        {
            try
            {
                if (operateLogModel.userInfo == null)
                {
                    operateLogModel.userInfo = LoginUserInfo.Get();
                }
                LogEntity logEntity = new LogEntity();
                logEntity.F_CategoryId = 3;
                logEntity.F_OperateTypeId = ((int)operateLogModel.type).ToString();
                logEntity.F_OperateType = EnumAttribute.GetDescription(operateLogModel.type);
                logEntity.F_OperateAccount = operateLogModel.userInfo.account;
                logEntity.F_OperateUserId = operateLogModel.userInfo.userId;
                logEntity.F_Module = operateLogModel.title;
                logEntity.F_ExecuteResult = 1;
                logEntity.F_ExecuteResultJson = "访问地址：" + operateLogModel.url;
                logEntity.F_SourceObjectId = operateLogModel.sourceObjectId;
                logEntity.F_SourceContentJson = operateLogModel.sourceContentJson;
                logEntity.WriteLog();
            }
            catch (Exception)
            {
            }
        }


        /// <summary>
        /// 写操作日志
        /// </summary>
        public void WriteLog(string content)
        {
            try
            {
                //记录日志
                LogEntity logEntity = new LogEntity();
                logEntity.F_CategoryId = 3;
                logEntity.F_OperateTypeId = ((int)LoginType.Other).ToString();
                logEntity.F_OperateType = EnumAttribute.GetDescription(LoginType.Other);
                logEntity.F_OperateAccount = "sbdosft";
                logEntity.F_OperateUserId = "sbdosft";
                logEntity.F_Module = "sbdsoft";
                logEntity.F_SourceContentJson = "";
                logEntity.F_ExecuteResult = 1;
                logEntity.F_ExecuteResultJson = content;
                LogBLL.WriteLog(logEntity);
            }
            catch (Exception)
            {
            }
        }

        #endregion

    }
}
