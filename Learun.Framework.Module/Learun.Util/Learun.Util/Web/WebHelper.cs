using Newtonsoft.Json;
using NPOI.XWPF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Weiz.MQ;

namespace Learun.Util
{
    /// <summary>
    /// 版 本 Learun-ADMS V7.0.0 力软敏捷开发框架
    /// Copyright (c) 2013-2018 上海力软信息技术有限公司
    /// 创建人：力软-框架开发组
    /// 日 期：2017.03.08
    /// 描 述：Web操作
    /// </summary>
    public class WebHelper
    {
        #region Host(获取主机名)

        /// <summary>
        /// 获取主机名,即域名，
        /// 范例：用户输入网址http://www.a.com/b.htm?a=1&amp;b=2，
        /// 返回值为: www.a.com
        /// </summary>
        public static string Host
        {
            get
            {
                return HttpContext.Current.Request.Url.Host;
            }
        }
        /// <summary>
        /// 本地的访问地址，加端口
        /// </summary>
        /// <returns></returns>
        public static string WebUrl
        {
            get
            {
                string domainUrl = "";
                if (HttpContext.Current != null)
                {
                    switch (HttpContext.Current.Request.Url.Port)
                    {
                        case 80:
                            domainUrl = HttpContext.Current.Request.Url.Host;
                            break;
                        default:
                            domainUrl = HttpContext.Current.Request.Url.Host + ":" + HttpContext.Current.Request.Url.Port.ToString();
                            break;
                    }
                    domainUrl = HttpContext.Current.Request.Url.Scheme + "://" + domainUrl;
                    domainUrl = domainUrl.Trim('/').Trim('\\') + "/";
                }
                return domainUrl;
            }
        }
        #endregion

        #region ResolveUrl(解析相对Url)

        /// <summary>
        /// 解析相对Url
        /// </summary>
        /// <param name="relativeUrl">相对Url</param>
        public static string ResolveUrl(string relativeUrl)
        {
            if (string.IsNullOrWhiteSpace(relativeUrl))
                return string.Empty;
            relativeUrl = relativeUrl.Replace("\\", "/");
            if (relativeUrl.StartsWith("/"))
                return relativeUrl;
            if (relativeUrl.Contains("://"))
                return relativeUrl;
            return VirtualPathUtility.ToAbsolute(relativeUrl);
        }

        #endregion

        #region HtmlEncode(对html字符串进行编码)

        /// <summary>
        /// 对html字符串进行编码
        /// </summary>
        /// <param name="html">html字符串</param>
        public static string HtmlEncode(string html)
        {
            return HttpUtility.HtmlEncode(html);
        }
        /// <summary>
        /// 对html字符串进行解码
        /// </summary>
        /// <param name="html">html字符串</param>
        public static string HtmlDecode(string html)
        {
            return HttpUtility.HtmlDecode(html);
        }

        #endregion

        #region UrlEncode(对Url进行编码)

        /// <summary>
        /// 对Url进行编码
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="isUpper">编码字符是否转成大写,范例,"http://"转成"http%3A%2F%2F"</param>
        public static string UrlEncode(string url, bool isUpper = false)
        {
            return UrlEncode(url, Encoding.UTF8, isUpper);
        }

        /// <summary>
        /// 对Url进行编码
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="encoding">字符编码</param>
        /// <param name="isUpper">编码字符是否转成大写,范例,"http://"转成"http%3A%2F%2F"</param>
        public static string UrlEncode(string url, Encoding encoding, bool isUpper = false)
        {
            var result = HttpUtility.UrlEncode(url, encoding);
            if (!isUpper)
                return result;
            return GetUpperEncode(result);
        }

        /// <summary>
        /// 获取大写编码字符串
        /// </summary>
        /// <param name="encode">字串</param>
        /// <returns></returns>
        private static string GetUpperEncode(string encode)
        {
            var result = new StringBuilder();
            int index = int.MinValue;
            for (int i = 0; i < encode.Length; i++)
            {
                string character = encode[i].ToString();
                if (character == "%")
                    index = i;
                if (i - index == 1 || i - index == 2)
                    character = character.ToUpper();
                result.Append(character);
            }
            return result.ToString();
        }

        #endregion

        #region UrlDecode(对Url进行解码)

        /// <summary>
        /// 对Url进行解码,对于javascript的encodeURIComponent函数编码参数,应使用utf-8字符编码来解码
        /// </summary>
        /// <param name="url">url</param>
        public static string UrlDecode(string url)
        {
            return HttpUtility.UrlDecode(url);
        }

        /// <summary>
        /// 对Url进行解码,对于javascript的encodeURIComponent函数编码参数,应使用utf-8字符编码来解码
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="encoding">字符编码,对于javascript的encodeURIComponent函数编码参数,应使用utf-8字符编码来解码</param>
        public static string UrlDecode(string url, Encoding encoding)
        {
            return HttpUtility.UrlDecode(url, encoding);
        }

        #endregion

        #region Session操作
        /// <summary>
        /// 写Session
        /// </summary>
        /// <typeparam name="T">Session键值的类型</typeparam>
        /// <param name="key">Session的键名</param>
        /// <param name="value">Session的键值</param>
        public static void WriteSession<T>(string key, T value)
        {
            if (key.IsEmpty())
                return;
            HttpContext.Current.Session[key] = value;
        }

        /// <summary>
        /// 写Session
        /// </summary>
        /// <param name="key">Session的键名</param>
        /// <param name="value">Session的键值</param>
        public static void WriteSession(string key, string value)
        {
            WriteSession<string>(key, value);
        }

        /// <summary>
        /// 读取Session的值
        /// </summary>
        /// <param name="key">Session的键名</param>        
        public static string GetSession(string key)
        {
            if (key.IsEmpty())
                return string.Empty;
            return HttpContext.Current.Session[key] as string;
        }
        /// <summary>
        /// 删除指定Session
        /// </summary>
        /// <param name="key">Session的键名</param>
        public static void RemoveSession(string key)
        {
            if (key.IsEmpty())
                return;
            HttpContext.Current.Session.Contents.Remove(key);
        }

        #endregion

        #region Cookie操作
        /// <summary>
        /// 写cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <param name="strValue">值</param>
        public static void WriteCookie(string strName, string strValue)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[strName];
            if (cookie == null)
            {
                cookie = new HttpCookie(strName);
            }
            cookie.Value = strValue;
            HttpContext.Current.Response.AppendCookie(cookie);
        }
        /// <summary>
        /// 写cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <param name="strValue">值</param>
        /// <param name="strValue">过期时间(分钟)</param>
        public static void WriteCookie(string strName, string strValue, int expires)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[strName];
            if (cookie == null)
            {
                cookie = new HttpCookie(strName);
            }
            cookie.Value = strValue;
            cookie.Expires = DateTime.Now.AddMinutes(expires);
            HttpContext.Current.Response.AppendCookie(cookie);
        }
        /// <summary>
        /// 读cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <returns>cookie值</returns>
        public static string GetCookie(string strName)
        {
            if (HttpContext.Current.Request.Cookies != null && HttpContext.Current.Request.Cookies[strName] != null)
            {
                return HttpContext.Current.Request.Cookies[strName].Value.ToString();
            }
            return "";
        }
        /// <summary>
        /// 删除Cookie对象
        /// </summary>
        /// <param name="CookiesName">Cookie对象名称</param>
        public static void RemoveCookie(string CookiesName)
        {
            HttpCookie objCookie = new HttpCookie(CookiesName.Trim());
            objCookie.Expires = DateTime.Now.AddYears(-5);
            HttpContext.Current.Response.Cookies.Add(objCookie);
        }
        #endregion

        #region GetFileControls(获取客户端文件控件集合)

        /// <summary>
        /// 获取有效客户端文件控件集合,文件控件必须上传了内容，为空将被忽略,
        /// 注意:Form标记必须加入属性 enctype="multipart/form-data",服务器端才能获取客户端file控件.
        /// </summary>
        /// <returns></returns>
        public static List<HttpPostedFile> GetFileControls()
        {
            var result = new List<HttpPostedFile>();
            var files = HttpContext.Current.Request.Files;
            if (files.Count == 0)
                return result;
            for (int i = 0; i < files.Count; i++)
            {
                var file = files[i];
                if (file.ContentLength == 0)
                    continue;
                result.Add(files[i]);
            }
            return result;
        }

        #endregion

        #region GetFileControl(获取第一个有效客户端文件控件)

        /// <summary>
        /// 获取第一个有效客户端文件控件,文件控件必须上传了内容，为空将被忽略,
        /// 注意:Form标记必须加入属性 enctype="multipart/form-data",服务器端才能获取客户端file控件.
        /// </summary>
        /// <returns></returns>
        public static HttpPostedFile GetFileControl()
        {
            var files = GetFileControls();
            if (files == null || files.Count == 0)
                return null;
            return files[0];
        }

        #endregion

        #region 去除HTML标记
        /// <summary>
        /// 去除HTML标记
        /// </summary>
        /// <param name="NoHTML">包括HTML的源码 </param>
        /// <returns>已经去除后的文字</returns>
        public static string NoHtml(string Htmlstring)
        {
            //删除脚本
            Htmlstring = Regex.Replace(Htmlstring, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
            //删除HTML
            Htmlstring = Regex.Replace(Htmlstring, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"-->", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(nbsp|#160);", " ", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&#(\d+);", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&hellip;", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&mdash;", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&ldquo;", "", RegexOptions.IgnoreCase);
            Htmlstring.Replace("<", "");
            Htmlstring = Regex.Replace(Htmlstring, @"&rdquo;", "", RegexOptions.IgnoreCase);
            Htmlstring.Replace(">", "");
            Htmlstring.Replace("\r\n", "");
            Htmlstring = HttpContext.Current.Server.HtmlEncode(Htmlstring).Trim();
            return Htmlstring;

        }
        #endregion

        #region 格式化文本（防止SQL注入）
        /// <summary>
        /// 格式化文本（防止SQL注入）
        /// </summary>
        /// <param name="html">html页面数据</param>
        /// <returns></returns>
        public static string Formatstr(string html)
        {
            System.Text.RegularExpressions.Regex regex1 = new System.Text.RegularExpressions.Regex(@"<script[\s\S]+</script *>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Regex regex2 = new System.Text.RegularExpressions.Regex(@" href *= *[\s\S]*script *:", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Regex regex3 = new System.Text.RegularExpressions.Regex(@" on[\s\S]*=", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Regex regex4 = new System.Text.RegularExpressions.Regex(@"<iframe[\s\S]+</iframe *>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Regex regex5 = new System.Text.RegularExpressions.Regex(@"<frameset[\s\S]+</frameset *>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Regex regex10 = new System.Text.RegularExpressions.Regex(@"select", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Regex regex11 = new System.Text.RegularExpressions.Regex(@"update", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Regex regex12 = new System.Text.RegularExpressions.Regex(@"delete", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            html = regex1.Replace(html, ""); //过滤<script></script>标记
            html = regex2.Replace(html, ""); //过滤href=javascript: (<A>) 属性
            html = regex3.Replace(html, " _disibledevent="); //过滤其它控件的on...事件
            html = regex4.Replace(html, ""); //过滤iframe
            html = regex10.Replace(html, "s_elect");
            html = regex11.Replace(html, "u_pudate");
            html = regex12.Replace(html, "d_elete");
            html = html.Replace("'", "’");
            html = html.Replace("&nbsp;", " ");
            return html;
        }
        #endregion

        #region 获取mac地址
        /// <summary>
        /// 返回描述本地计算机上的网络接口的对象(网络接口也称为网络适配器)。
        /// </summary>
        /// <returns></returns>
        public static NetworkInterface[] NetCardInfo()
        {
            return NetworkInterface.GetAllNetworkInterfaces();
        }

        ///<summary>
        /// 通过NetworkInterface读取网卡Mac
        ///</summary>
        ///<returns></returns>
        public static List<string> GetMacByNetworkInterface()
        {
            List<string> macs = new List<string>();
            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface ni in interfaces)
            {
                macs.Add(ni.GetPhysicalAddress().ToString());
            }
            return macs;
        }
        #endregion

        #region 获取皮肤主题
        /// <summary>
        /// 获取用户当前UI主题皮肤类
        /// </summary>
        /// <returns></returns>
        public static string GetUITheme()
        {
            string res = "lr-uitheme-default";
            try
            {
                string learn_UItheme = WebHelper.GetCookie("Learn_ADMS_V6.1_UItheme");
                switch (learn_UItheme)
                {
                    case "1":
                        res = "lr-uitheme-default";     // 经典版本
                        break;
                    case "2":
                        res = "lr-uitheme-accordion";   // 手风琴版本
                        break;
                    case "3":
                        res = "lr-uitheme-windos";      // Windos版本
                        break;
                    case "4":
                        res = "lr-uitheme-top";        // 顶部菜单版本
                        break;
                    default:
                        res = "lr-uitheme-default";     // 经典版本
                        break;
                }

            }
            catch (Exception)
            {
            }
            return res;
        }
        #endregion

        #region 添加/获取上下文信息
        /// <summary>
        /// 添加链接上下文信息
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="obj">数据</param>
        public static void AddHttpItems(string name, object data)
        {
            HttpContext.Current.Items.Add(name, data);
        }
        /// <summary>
        /// 更新链接上下文信息
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="data">数据</param>
        public static void UpdateHttpItem(string name, object data)
        {
            HttpContext.Current.Items[name] = data;
        }
        /// <summary>
        /// 获取链接上下文信息
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns></returns>
        public static object GetHttpItems(string name)
        {
            if (HttpContext.Current == null)
            {
                return null;
            }
            return HttpContext.Current.Items[name];
        }
        #endregion

        #region WebForm和WinForm通用的取当前根目录的方法
        /// <summary>
        /// WebForm和WinForm通用的取当前根目录的方法 
        /// </summary>
        public static string BasePath
        {
            get
            {
                if (System.Web.HttpContext.Current != null)
                    return System.Web.HttpContext.Current.Server.MapPath("~/").TrimEnd(new char[] { '\\' });
                else //当控件在定时器的触发程序中使用时就为空
                {
                    return System.AppDomain.CurrentDomain.BaseDirectory.TrimEnd(new char[] { '\\' });
                }
            }
        }
        #endregion

        #region 业务附件处理
        /// <summary>
        /// 将业务关联附件
        /// </summary>
        /// <param name="UserID">操作人</param>
        /// <param name="OperationCode">附加编码</param>
        /// <param name="OperationID">业务主键</param>
        /// <param name="AttachmentID">附件ID列表，以逗号分隔</param>
        public static void BindAttachment(string UserID, string OperationCode, string OperationID, string AttachmentID = "")
        {
            try
            {
                if (!AttachmentID.IsEmpty())
                {
                    AttachmentID = AttachmentID.Replace("&nbsp;", "");
                }
                string UploadFlag = Config.GetValue("UploadFlag");
                string domainUrl = Config.GetValue("UploadUrl");
                if (domainUrl.IsEmpty()) { throw new Exception("请配置上传文件地址"); }
                switch (UploadFlag)
                {
                    case "1"://自定义服务器
                        domainUrl = Config.GetValue("UploadUrl");
                        break;
                    default://本地
                        domainUrl = WebUrl;
                        break;
                }
                string Value = UserID + OperationCode + OperationID + AttachmentID;
                bool flag = false;
                string MerKey = Config.GetValue("SecurityKey");//密钥
                if (MerKey == null)
                {
                    MerKey = "";
                }
                string outMD = string.Empty;
                outMD = GetMD5(Value + MerKey).ToUpper();
                string Url = domainUrl + "/apiMD5?parameters=Function=UpdateAttachments|UserID=" + UserID + "|OperationCode=" + OperationCode + "|OperationID=" + OperationID + "|AttachmentID=" + AttachmentID + "|MD5=" + outMD;
                string data = HttpMethods.Get(Url);
            }
            catch (Exception ex)
            {
                throw new Exception("上传文件地址访问失败，请检查上传文件配置");
            }
        }
        /// <summary>
        /// 与ASP兼容的MD5加密算法
        /// </summary>
        private static string GetMD5(string s)
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

        #region 文件大小转换（字节）
        /// <summary>
        /// 转换方法
        /// </summary>
        /// <param name="size">字节值</param>
        /// <returns></returns>
        public static String HumanReadableFilesize(double size)
        {
            String[] units = new String[] { "B", "KB", "MB", "GB", "TB", "PB" };
            double mod = 1024.0;
            int i = 0;
            while (size >= mod)
            {
                size /= mod;
                i++;
            }
            return Math.Round(size) + units[i];
        }
        #endregion

        #region 发送短信
        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="Phone">手机号</param>
        /// <param name="sContent">内容</param>
        /// <returns></returns>
        public static string SendSMS(string Phone, string sContent)
        {
            string SendLog = "";
            try
            {
                MyMessage msg = new MyMessage();
                msg.Phone = Phone;
                msg.MessageBody = sContent;
                msg.MessageRouter = MessageRouter.SMS;
                MQHelper.Publish(msg);
            }
            catch (Exception ex)
            {
                SendLog = ex.Message;
            }
            return SendLog;
        }
        #endregion

        #region 极光推送-通知
        /// <summary>
        /// 极光推送-通知
        /// </summary>
        /// <param name="Title">标题</param>
        /// <param name="Alias">别名集合</param>
        /// <param name="Tag">标签集合</param>
        /// <param name="Extras">其他数据键值对</param>
        /// <param name="PType">类型（0：广播  1：别名互动  2：标签）</param>
        /// <returns></returns>
        public static string SMS_JPushNotification(string Title, string[] Alias, string[] Tag, Dictionary<string, object> Extras, int PType)
        {
            string SendLog = "";
            try
            {
                Web.Model.SMS_JPushNotificationInfo model = new Web.Model.SMS_JPushNotificationInfo();
                model.Alert = Title;
                model.Alias = Alias == null ? "" : string.Join(",", Alias);
                model.Tag = Tag == null ? "" : string.Join(",", Tag);
                model.ExtrasJson = JsonConvert.SerializeObject(Extras);
                model.PType = PType;

                MyMessage msg = new MyMessage();
                msg.MessageBody = JsonConvert.SerializeObject(model);
                msg.MessageRouter = MessageRouter.JPush;
                MQHelper.Publish(msg);
            }
            catch (Exception ex)
            {
                SendLog = ex.Message;
            }
            return SendLog;
        }
        #endregion

        #region 身份证验证
        /// <summary>
        ///  身份证验证函数(标准18位验证) 
        /// </summary>
        /// <param name=""></param>
        public static bool CheckIDCard18(string idNumber)
        {
            long n = 0;
            if (idNumber.Length != 18)
            {
                return false;
            }
            if (long.TryParse(idNumber.Remove(17), out n) == false || n < Math.Pow(10, 16) || long.TryParse(idNumber.Replace('x', '0').Replace('X', '0'), out n) == false)
            {
                return false;//数字验证 
            }
            string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (address.IndexOf(idNumber.Remove(2)) == -1)
            {
                return false;//省份验证  
            }
            string birth = idNumber.Substring(6, 8).Insert(6, "-").Insert(4, "-");
            DateTime time = new DateTime(); if (DateTime.TryParse(birth, out time) == false)
            {
                return false;//生日验证  
            }
            string[] arrVarifyCode = ("1,0,x,9,8,7,6,5,4,3,2").Split(',');
            string[] Wi = ("7,9,10,5,8,4,2,1,6,3,7,9,10,5,8,4,2").Split(',');
            char[] Ai = idNumber.Remove(17).ToCharArray();
            int sum = 0; for (int i = 0; i < 17; i++)
            {
                sum += int.Parse(Wi[i]) * int.Parse(Ai[i].ToString());
            }
            int y = -1; Math.DivRem(sum, 11, out y);
            if (arrVarifyCode[y] != idNumber.Substring(17, 1).ToLower())
            {
                return false;//校验码验证 
            }
            return true;//符合GB11643-1999标准  

        }
        #endregion
        #region 导出Word
        /// <summary>
        /// 导出Word
        /// </summary>
        /// <param name="obj">待导出实体</typeparam>
        /// <param name="Name">Word标题</param>
        /// <param name="tempFilePath">Word模版的相对路径</param>
        /// <returns>Word路径,出错为空</returns>
        public static string ExportWord(object obj, string tempFilePath, string Name = "")
        {
            try
            {
                if (string.IsNullOrEmpty(Name))
                {
                    Name = "暂无标题";
                }
                string outPath = string.Empty;
                if (string.IsNullOrEmpty(outPath))
                {
                    outPath = "/FileUpload/ExportWord/" + Name + "(" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ").docx";
                }
                string DIRPATH = outPath.Substring(0, outPath.LastIndexOf('/'));
                //判断目录是否存在
                if (!Directory.Exists(HttpContext.Current.Server.MapPath(DIRPATH)))
                {
                    Directory.CreateDirectory(HttpContext.Current.Server.MapPath(DIRPATH));
                }
                //判断文件是否存在
                string outTPath = HttpContext.Current.Server.MapPath(outPath);
                if (!File.Exists(outTPath))
                {
                    FileStream fs = new FileStream(outTPath, FileMode.CreateNew);
                    StreamWriter sw = new StreamWriter(fs);
                    sw.Flush();
                    fs.Dispose();
                }

                //调用方法写入数据
                ExportObjet(HttpContext.Current.Server.MapPath(tempFilePath), outTPath, obj);
                return outPath;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
        public static void ExportObjet(string tempFilePath, string outPath, object data)
        {
            using (FileStream stream = File.OpenRead(tempFilePath))
            {
                XWPFDocument doc = new XWPFDocument(stream);
                //遍历段落                  
                foreach (var para in doc.Paragraphs)
                {
                    ReplaceObject(para, data);
                }
                //遍历表格      
                foreach (var table in doc.Tables)
                {
                    foreach (var row in table.Rows)
                    {
                        foreach (var cell in row.GetTableCells())
                        {
                            foreach (var para in cell.Paragraphs)
                            {
                                ReplaceObject(para, data);
                            }
                        }
                    }
                }
                //写文件
                FileStream outFile = new FileStream(outPath, FileMode.Create);
                doc.Write(outFile);
                outFile.Close();
            }
        }

        private static void ReplaceObject(XWPFParagraph para, object model)
        {
            try
            {
                string text = "";
                Type t = model.GetType();
                PropertyInfo[] pi = t.GetProperties();
                foreach (var run in para.Runs)
                {
                    text = run.ToString();

                    foreach (PropertyInfo p in pi)
                    {
                        //$$模板中数据占位符为$KEY$
                        string key = p.Name.ToLower();
                        if (text == key)
                        {
                            try
                            {
                                text = text.Replace(key, p.GetValue(model, null).ToString());
                            }
                            catch (Exception ex)
                            {
                                //可能有空指针异常
                                text = text.Replace(key, "");
                            }
                        }
                    }
                    text = text.Replace("</P>", "</p>");
                    List<string> sArray = Regex.Split(text, "</p>", RegexOptions.IgnoreCase).ToList();
                    List<string> sArray2 = sArray.Where(x => !x.IsEmpty()).ToList();
                    if (sArray2.Count > 1)
                    {
                        var num = 0;
                        foreach (var item in sArray2)
                        {
                            if (!item.removeHTML().IsEmpty())
                            {
                                run.AddTab(); run.AddTab();
                                run.SetText(item.removeHTML(), num);
                                run.AddBreak(BreakType.TEXTWRAPPING);
                                num++;
                            }

                        }
                    }
                    else
                    {
                        run.SetText(text, 0);
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        #endregion

        #region 身份证
        /// <summary>
        /// 根据身份证号获取年龄
        /// </summary>
        /// <param name="identityCard"></param>
        /// <returns></returns>
        public static int GetAgeByIDCard(string identityCard)
        {
            int age = 0;
            int month = 0;
            int day = 0;
            string year = "";
            try
            {
                //获取得到输入的身份证号码
                identityCard = identityCard.Trim();

                //身份证号码不能为空，如果为空返回
                if (string.IsNullOrEmpty(identityCard))
                {
                    return age;
                }
                else
                {
                    //身份证号码只能为15位或18位其它不合法
                    if (identityCard.Length != 15 && identityCard.Length != 18)
                    {
                        return age;
                    }
                }
                //处理18位的身份证号码从号码中得到年龄
                if (identityCard.Length == 18)
                {
                    month = int.Parse(DateTime.Now.Month.ToString()) - int.Parse(identityCard.Substring(10, 2));//获取月份差
                    day = int.Parse(DateTime.Now.Day.ToString()) - int.Parse(identityCard.Substring(12, 2));//获取day差
                    age = int.Parse(DateTime.Now.Year.ToString()) - int.Parse(identityCard.Substring(6, 4));//获取年份差

                }
                //处理15位的身份证号码从号码中得到年龄
                if (identityCard.Length == 15)
                {
                    year = "19" + identityCard.Substring(6, 2);
                    month = int.Parse(DateTime.Now.Month.ToString()) - int.Parse(identityCard.Substring(8, 2));//获取月份差
                    day = int.Parse(DateTime.Now.Day.ToString()) - int.Parse(identityCard.Substring(10, 2));//获取day差
                    age = int.Parse(DateTime.Now.Year.ToString()) - int.Parse(year);//获取年份差      //判断月日

                }
                if (month <= 0 && day < 0)
                {
                    age--;
                }
                return age;
            }
            catch (Exception ex)
            {
                return age;
            }
        }

        /// <summary>
        /// 通过身份证号获取出生年月
        /// </summary>
        /// <param name="IDCard">身份证号</param>
        /// <returns></returns>
        public static string GetBirthdayByIDCard(string IDCard)
        {
            string str = "";
            string birthday = "";
            try
            {
                //获取得到输入的身份证号码
                string identityCard = IDCard.Trim();

                if (string.IsNullOrEmpty(identityCard))
                {
                    //身份证号码不能为空，如果为空返回
                    str = "身份证号码不能为空";
                    return str;
                }
                else
                {
                    //身份证号码只能为15位或18位其它不合法
                    if (identityCard.Length != 15 && identityCard.Length != 18)
                    {
                        str = "身份证号码为15位或18位，请检查！";
                        return str;
                    }
                }
                //处理18位的身份证号码从号码中得到生日和性别代码
                if (identityCard.Length == 18)
                {
                    birthday = identityCard.Substring(6, 4) + "-" + identityCard.Substring(10, 2) + "-" + identityCard.Substring(12, 2);
                }
                //处理15位的身份证号码从号码中得到生日和性别代码
                if (identityCard.Length == 15)
                {
                    birthday = "19" + identityCard.Substring(6, 2) + "-" + identityCard.Substring(8, 2) + "-" + identityCard.Substring(10, 2);
                }
                str = birthday;
                return str;
            }
            catch (Exception ex)
            {
                str = "身份证号码输入有误";
                return str;
            }
        }

        /// <summary>
        /// 通过身份证号获取出生性别
        /// </summary>
        /// <param name="IDCard">身份证号</param>
        /// <returns></returns>
        public static string GetSexByIDCard(string IDCard)
        {
            string str = "";
            string sex = "";
            try
            {
                //获取得到输入的身份证号码
                string identityCard = IDCard.Trim();

                if (string.IsNullOrEmpty(identityCard))
                {
                    //身份证号码不能为空，如果为空返回
                    str = "身份证号码不能为空";
                    return str;
                }
                else
                {
                    //身份证号码只能为15位或18位其它不合法
                    if (identityCard.Length != 15 && identityCard.Length != 18)
                    {
                        str = "身份证号码为15位或18位，请检查！";
                        return str;
                    }
                }
                //处理18位的身份证号码从号码中得到生日和性别代码
                if (identityCard.Length == 18)
                {
                    sex = identityCard.Substring(14, 3);
                }
                //处理15位的身份证号码从号码中得到生日和性别代码
                if (identityCard.Length == 15)
                {
                    sex = identityCard.Substring(12, 3);
                }
                //性别代码为偶数是女性奇数为男性
                if (int.Parse(sex) % 2 == 0)
                {
                    sex = "女";
                }
                else
                {
                    sex = "男";
                }
                str += sex;
                return str;
            }
            catch (Exception ex)
            {
                str = "身份证号码输入有误";
                return str;
            }
        }
        #endregion
        public static string createWord(string filename, string savepath)
        {
            string file = ""; //路径1（我们之前生成的文件路径）
            string file2 = "";//路径2
            file = savepath + filename + ".doc";
            string demo = System.Web.Hosting.HostingEnvironment.MapPath(file);
            file2 = demo + filename + "_.doc";

            Object path = file as Object;
            Object path2 = file2 as Object;
            Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();
            Object Nothing = Missing.Value;
            Object format = Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatDocument;
            Microsoft.Office.Interop.Word.Document wordDoc = wordApp.Documents.Open(ref path, false); //打开之前生成的文件

            wordDoc.Activate();//设为当前操作的文件
            //指定要在页面视图中显示的文档元素
            wordApp.ActiveWindow.View.SeekView = Microsoft.Office.Interop.Word.WdSeekView.wdSeekMainDocument;//设为主文档



            //设置文档为页面视图模式
            wordApp.ActiveWindow.View.Type = Microsoft.Office.Interop.Word.WdViewType.wdNormalView;



            wordApp.ActiveWindow.ActivePane.Selection.WholeStory();
            //指定要应用于段落的行距格式
            wordApp.ActiveWindow.ActivePane.Selection.ParagraphFormat.LineSpacingRule = Microsoft.Office.Interop.Word.WdLineSpacing.wdLineSpace1pt5;//1.5 倍行距。该行距相当于当前字号加 6 磅。
            //设置指定段落的段后间距
            wordApp.ActiveWindow.ActivePane.Selection.ParagraphFormat.LineUnitAfter = 0.5f;

            //把操作后的文件保存到路径2
            wordDoc.SaveAs(ref path2, ref format, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing);
            wordDoc.Close(ref Nothing, ref Nothing, ref Nothing);
            wordApp.Quit(ref Nothing, ref Nothing, ref Nothing);
            //删除原本生成的文件
            File.Delete(file);
            //把路径2的文件剪切到路径1
            File.Move(file2, file);
            //返回路径
            return file;
        }

    }
}
