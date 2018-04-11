using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace PR.WizPlant.HumitureTool
{
    public class HttpRequestHelper
    {

        public const string ContentTypeText = "text/html;charset=UTF-8";
        public const string ContentTypeJson = "application/json";
        public const string ContentTypeStream = "application/octet-stream";
        public const string ContentTypeForm = "application/x-www-form-urlencoded";
        /// <summary>
        /// GET请求与获取结果
        /// </summary>
        /// <param name="Url">URL</param>
        /// <param name="getDataStr">get请求参数不带“？”，如：a=1&b=2</param>
        /// <returns></returns>
        public static string HttpGet(string Url, string getDataStr, string contentType = "text/html;charset=UTF-8")
        {
            string retString = string.Empty;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url + (getDataStr == "" ? "" : "?") + getDataStr);
                request.Method = "GET";
                request.ContentType = contentType;

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);
                retString = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();
            }
            catch (Exception ex)
            {

                retString = ex.Message;
            }
          

            return retString;
        }



        /// <summary>
        /// POST请求与获取结果
        /// </summary>
        /// <param name="Url">URL</param>
        /// <param name="postDataStr">post请求参数，如：a=1&b=2</param>
        /// <param name="authtoken">post头部信息，验证身份--Extrabux接口</param>
        /// <returns></returns>
        public static string HttpPostHeader(string Url, string postDataStr, string authtoken)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.KeepAlive = true;
            request.Method = "POST";
            if (!string.IsNullOrWhiteSpace(authtoken))
                request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(new ASCIIEncoding().GetBytes(authtoken)));
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = postDataStr.Length;
            StreamWriter writer = new StreamWriter(request.GetRequestStream(), Encoding.ASCII);
            writer.Write(postDataStr);
            writer.Flush();
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string encoding = response.ContentEncoding;
            if (encoding == null || encoding.Length < 1)
                encoding = "UTF-8"; //默认编码  
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(encoding));
            string retString = reader.ReadToEnd();
            return retString;
        }
        /// <summary>
        /// POST请求与获取结果,UTF-8编码
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="postDataStr"></param>
        /// <returns></returns>
        public static string HttpPost(string Url, string postDataStr, string contentType= ContentTypeJson)
        {
            //发送请求的数据
            WebRequest request = WebRequest.Create(Url);
            request.Method = "POST";
            UTF8Encoding UTF8 = new UTF8Encoding();
            byte[] byte1 = UTF8.GetBytes(postDataStr);
            request.ContentType = contentType;
            request.ContentLength = byte1.Length;
            Stream newStream = request.GetRequestStream();
            newStream.Write(byte1, 0, byte1.Length);
            newStream.Close();

            //发送成功后接收返回的信息
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Encoding enc = Encoding.GetEncoding("UTF-8");
            Stream stream = response.GetResponseStream();
            StreamReader streamReader = new StreamReader(stream, enc);
            string retString = streamReader.ReadToEnd();
            return retString;
        }


        

    }
}
