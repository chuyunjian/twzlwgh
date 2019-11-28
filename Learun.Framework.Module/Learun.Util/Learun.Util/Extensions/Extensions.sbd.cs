using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Learun.Util
{
    public static partial class Extensions
    {
        #region 除去html标签
        /// <summary>
        /// 除去html标签
        /// </summary>
        /// <param name="Htmlstring"></param>
        /// <returns></returns>
        public static string removeHTML(this string Htmlstring)  //替换HTML标记
        {
            //删除脚本
            Htmlstring = Regex.Replace(Htmlstring, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
            //删除HTML
            Htmlstring = Regex.Replace(Htmlstring, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"([rn])[s]+", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"-->", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(nbsp|#160);", " ", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(iexcl|#161);", "xa1", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(cent|#162);", "xa2", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(pound|#163);", "xa3", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(copy|#169);", "xa9", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&#(d+);", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"<img[^>]*>;", "", RegexOptions.IgnoreCase);
            Htmlstring.Replace("<", "");
            Htmlstring.Replace(">", "");
            Htmlstring.Replace("rn", "");
            Htmlstring = Htmlstring.Trim();
            return Htmlstring;
        }
        #endregion

        #region 截取等宽中英文字符串
        /// <summary>
        /// 截取等宽中英文字符串
        /// </summary>
        /// <param name="length">要截取的中文字符长度</param>
        /// <param name="appendStr">截取后后追加的字符串</param>
        /// <returns>截取后的字符串</returns>
        public static string SubString(this string str, int length, string appendStr)
        {
            if (str == null) return string.Empty;

            int len = length * 2;
            //aequilateLength为中英文等宽长度,cutLength为要截取的字符串长度
            int aequilateLength = 0, cutLength = 0;
            Encoding encoding = Encoding.GetEncoding("gb2312");

            string cutStr = str.ToString();
            int strLength = cutStr.Length;
            byte[] bytes;
            for (int i = 0; i < strLength; i++)
            {
                bytes = encoding.GetBytes(cutStr.Substring(i, 1));
                if (bytes.Length == 2)//不是英文
                    aequilateLength += 2;
                else
                    aequilateLength++;

                if (aequilateLength <= len) cutLength += 1;

                if (aequilateLength > len)
                    return cutStr.Substring(0, cutLength) + appendStr;
            }
            return cutStr;
        }
        #endregion
        #region 截取身份证号码中符串
        
        #endregion

        #region 给文本中图片和视频src添加admin地址

        /// <summary>
        /// 给文本中图片和视频src添加admin地址
        /// </summary>
        /// <param name="html">文本</param>
        /// <param name="url">admin地址</param>
        /// <param name="regstr">图片正则</param>
        /// <param name="regVideoStr">视频正则</param>
        /// <returns></returns>
        public static string AdditionalDomainName(this string html, string url, string regstr = "<img[^>]+src\\s*=\\s*['\"]([^'\"]+)['\"][^>]*>", string regVideoStr = "<video[^>]+src\\s*=\\s*['\"]([^'\"]+)['\"][^>]*>")
        {
            Regex r = new Regex(regstr, RegexOptions.IgnoreCase);
            Regex r2 = new Regex(regVideoStr, RegexOptions.IgnoreCase);

            MatchCollection mc = r.Matches(html);
            MatchCollection mc2 = r2.Matches(html);
            foreach (Match m in mc)
            {
                if (!m.Groups[1].Value.Contains("http"))
                {
                    html = html.Replace(m.Groups[1].Value, url + m.Groups[1].Value);
                }
            }

            foreach (Match m in mc2)
            {
                if (!m.Groups[1].Value.Contains("http"))
                {
                    html = html.Replace(m.Groups[1].Value, url + m.Groups[1].Value);
                }
            }
            return html;
        }
        #endregion
    }
}
