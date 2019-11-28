using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learun.Util.Login
{
    /// <summary>
    /// 登录身份
    /// </summary>
    public enum LoginIdentity
    {
        /// <summary>
        /// 系统用户
        /// </summary>
        [Description("系统用户")]
        System = 0,
    }
    /// <summary>
    /// 登录方式
    /// </summary>
    public enum LoginMode
    {
        /// <summary>
        /// 电脑端登录
        /// </summary>
        [Description("电脑端登录")]
        PC = 0,
        /// <summary>
        /// 移动端接口登录
        /// </summary>
        [Description("移动端接口登录")]
        APP = 1,
    }
}
