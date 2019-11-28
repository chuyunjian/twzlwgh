using Learun.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Learun.Application.Web.Controllers
{
    /// <summary>
    /// 自定义controller
    /// </summary>
    public class OtherController : MvcControllerBase
    {
        #region 获取数据
        /// <summary>
        /// 字符+加密字符串得到MD5的签名
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult GetMD5Token(string Value)
        {
            bool flag = false;
            string MerKey = ConfigurationManager.AppSettings["SecurityKey"];//密钥
            if (MerKey == null)
            {
                MerKey = "";
            }
            string outMD = string.Empty;
            outMD = GetMD5(Value + MerKey).ToUpper();
            var jsonData = new
            {
                MD5 = outMD
            };
            return Success(jsonData);
        }
        /// <summary>
        /// 与ASP兼容的MD5加密算法
        /// </summary>
        private string GetMD5(string s)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] t = md5.ComputeHash(Encoding.UTF8.GetBytes(s));
            StringBuilder sb = new StringBuilder(32);
            for (int i = 0; i < t.Length; i++)
            {
                sb.Append(t[i].ToString("x").PadLeft(2, '0'));
            }
            return sb.ToString();
        }
        #endregion
    }
}