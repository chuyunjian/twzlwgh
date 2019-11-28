using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Learun.Util
{
    public class AJAX
    {
        /// <summary>
        /// post 请求 模拟Form提交
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postString"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static string Post(string url, object postData, string token = "", string EncodingStr = "GB2312")
        {
            return Post(url, GetProperties(postData), token, EncodingStr);
        }
        /// <summary>
        /// Post数据到指定页面
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        public static string Post(string url, string postData, string token = "", string EncodingStr = "GB2312")
        {
            byte[] byteArray;
            byte[] responseArray;
            string strTemp = string.Empty;
            Encoding encoding = Encoding.GetEncoding(EncodingStr);
            try
            {
                WebClient myWebClient = new WebClient();
                WebHeaderCollection myWebHeaderCollection;
                myWebClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                if (!token.IsEmpty())
                {
                    myWebClient.Headers.Add("Authorization", token);
                }
                myWebHeaderCollection = myWebClient.Headers;
                byteArray = encoding.GetBytes(postData);
                responseArray = myWebClient.UploadData(url, "POST", byteArray);
                strTemp = Encoding.UTF8.GetString(responseArray);
            }
            catch (Exception ex)
            {
                strTemp = ex.Message;
            }
            return strTemp;
        }
        /// <summary>
        /// GET 请求
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string Get(string url, string token = "")
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "GET";
            httpWebRequest.Timeout = 20000;
            //byte[] btBodys = Encoding.UTF8.GetBytes(body);
            //httpWebRequest.ContentLength = btBodys.Length;
            //httpWebRequest.GetRequestStream().Write(btBodys, 0, btBodys.Length);
            if (!token.IsEmpty())
            {
                httpWebRequest.Headers.Add("Authorization", token);
            }
            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream());
            string responseContent = streamReader.ReadToEnd();

            httpWebResponse.Close();
            streamReader.Close();

            return responseContent;
        }

        /// <summary>
        /// 将对象转为地址拼接字符串
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private static string GetProperties(object t)
        {
            string tStr = string.Empty;
            if (t == null)
            {
                return tStr;
            }
            System.Reflection.PropertyInfo[] properties = t.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

            if (properties.Length <= 0)
            {
                return tStr;
            }
            foreach (System.Reflection.PropertyInfo item in properties)
            {
                string name = item.Name;
                object value = item.GetValue(t, null);
                if (item.PropertyType.IsValueType || item.PropertyType.Name.StartsWith("String"))
                {
                    tStr += string.Format("&{0}={1}", name, value);
                }
                else
                {
                    GetProperties(value);
                }
            }
            return tStr.Trim('&');
        }
    }
}
