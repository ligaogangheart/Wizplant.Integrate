using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Collections.Specialized;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using Common.Logging;
using PR.WizPlant.Integrate.Sql;
using System.Data;

namespace PR.WizPlant.Integrate.AnyShare
{
    public class AnyShareProxy
    {
        static Encoding encoding = Encoding.UTF8;
        static ILog logger = LogManager.GetLogger<AnyShareProxy>();
        static AnyShareTicket ticket = new AnyShareTicket();
        static string anyshareauthservice = System.Configuration.ConfigurationManager.AppSettings["anyshareauthservice"];
        static string userName = System.Configuration.ConfigurationManager.AppSettings["anyshareuser"];
        static string password = System.Configuration.ConfigurationManager.AppSettings["anysharepassword"];
        static string rsapublickey = null;
        static string publickey
        {
            get
            {
                if (rsapublickey == null)
                {
                    var val = System.Configuration.ConfigurationManager.AppSettings["anysharepublickey"];
                    if (string.IsNullOrEmpty(val))
                    {
                        logger.Error("没有配置云平台公钥");
                        throw new Exception("没有配置云平台公钥");
                    }
                    val = val.TrimEnd();
                    int len = val.Length;
                    if (len < 20)
                    {
                        rsapublickey = "ERROR";
                        logger.ErrorFormat("配置的云平台公钥无效");
                        throw new Exception("配置的云平台公钥无效");
                    }
                    string modulus = val.Substring(0, len - 4);
                    string exponent = val.Substring(len - 4);
                    rsapublickey = string.Format("<RSAKeyValue><Modulus>{0}</Modulus><Exponent>{1}</Exponent></RSAKeyValue>", modulus, exponent);
                    var rsa = new RSACryptoServiceProvider();
                    try
                    {
                        rsa.FromXmlString(rsapublickey);
                    }
                    catch (Exception ex)
                    {
                        rsapublickey = "ERROR";
                        logger.ErrorFormat("配置的云平台公钥无效");
                        throw new Exception("配置的云平台公钥无效");
                    }
                }
                else if(rsapublickey == "ERROR")
                {
                    logger.ErrorFormat("配置的云平台公钥无效");
                    throw new Exception("配置的云平台公钥无效");
                }
                return rsapublickey;
            }
        }
        public static AnyShareTicket Ticket
        {
            get
            {
                if (!ticket.IsAuth)
                {
                    Login(userName, password);
                }
                return ticket;
            }
        }

        static Dictionary<string, string> mapCache = null;

        public static Dictionary<string, string> MapCache
        {
            get
            {
                if (mapCache == null)
                {
                    mapCache = new Dictionary<string, string>();
                    string selectSql = "SELECT [ObjectId],[DirId] FROM [PRW_Inte_AnyShare_DirMap] WHERE [ObjectId] IS NOT NULL";
                    using (var reader = SqlHelper.ExecuteDataReader(SqlHelper.DefaultConnectionString, CommandType.Text, selectSql))
                    {
                        while (reader.Read())
                        {
                            mapCache[reader.GetGuid(0).ToString().ToLower()] = reader.GetString(1);
                        }
                    }
                }
                return mapCache;
            }
        }
        public static string HttpPost(string Url, string postDataStr)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                request.Method = "POST";
                request.ContentType = string.Format("application/json; charset={0}",encoding.BodyName);

                Stream requestStream = request.GetRequestStream();
                byte[] buffer = encoding.GetBytes(postDataStr);

                requestStream.Write(buffer, 0, buffer.Length);
                requestStream.Close();

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string encodingName = response.ContentEncoding;
                if (encodingName == null || encodingName.Length < 1)
                {
                    encodingName = encoding.BodyName; //默认编码  
                }
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(encodingName));
                string retString = reader.ReadToEnd();
                return retString;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static byte[] DownloadFirstFileBlock(string url, string docId,  out Int64 allSize, out string fileName)
        {
            int sn = 0;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json; charset=UTF-8";

                // 构造请求
                Stream requestStream = request.GetRequestStream();
                JObject jo = new JObject();
                jo.Add(new JProperty("docid", docId));
                jo.Add(new JProperty("sn", sn));

                string jsonStr = jo.ToString();
                byte[] buffer = encoding.GetBytes(jsonStr);

                requestStream.Write(buffer, 0, buffer.Length);
                requestStream.Close();

                // 发送请求
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                // 获取返回的数据
                Stream res = response.GetResponseStream();
                int length = (int)response.ContentLength;
                byte[] bytes = new byte[length];
                int readCnt = 0;
                int readLength = 0;
                while (readCnt != bytes.Length)
                {
                    readLength = res.Read(bytes, readCnt, bytes.Length - readCnt);
                    readCnt = readCnt + readLength;
                }

                // 解析二进制数据

                // index为boundry加上\r\n的长度
                byte[] split = System.Text.Encoding.ASCII.GetBytes("\r\n\r\n");

                // firstBoundaryEnd为不包含\r\n的第一个boundary的结束位置
                int firstBoundaryEnd = Locate(bytes, 0, split);

                byte[] boundary = new byte[firstBoundaryEnd];
                Stream stream = new MemoryStream(bytes);
                stream.Read(boundary, 0, firstBoundaryEnd);

                int blockDatBegin = firstBoundaryEnd + 4;
                int blockDataEnd = Locate(bytes, firstBoundaryEnd, boundary) - 2;

                byte[] blockData = new byte[blockDataEnd - blockDatBegin];
                stream.Seek(blockDatBegin, SeekOrigin.Begin);
                stream.Read(blockData, 0, blockDataEnd - blockDatBegin);

                int jsonDataBegin = blockDataEnd + 2 + boundary.Length + 4;
                int jsonDataEnd = Locate(bytes, jsonDataBegin, boundary) - 2;

                byte[] jsonData = new byte[jsonDataEnd - jsonDataBegin];
                stream.Seek(jsonDataBegin, SeekOrigin.Begin);
                stream.Read(jsonData, 0, jsonDataEnd - jsonDataBegin);

                JObject resJo = JObject.Parse(encoding.GetString(jsonData));
                
                    allSize = (Int64)resJo["size"];
                    fileName = resJo["name"].Value<string>();                   

                //string tmp = String.Format("DownloadFileBlock({0}-{1}, {2}/{3})", docId, sn, downSize, allSize);
                    logger.DebugFormat("DownloadFirstFileBlock({0}-{1}, {2}/{3})", docId, sn, blockData.Length, allSize);
                //Console.WriteLine(tmp);

                return blockData;
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("DownloadFirstFileBlock({0}-{1}) ERROR:{2}", docId, sn, ex.Message, ex);
                throw ex;
            }
        }

        public static byte[] DownloadFileBlock(string url, string docId, int sn)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json; charset=UTF-8";

                // 构造请求
                Stream requestStream = request.GetRequestStream();
                JObject jo = new JObject();
                jo.Add(new JProperty("docid", docId));
                jo.Add(new JProperty("sn", sn));

                string jsonStr = jo.ToString();
                byte[] buffer = encoding.GetBytes(jsonStr);

                requestStream.Write(buffer, 0, buffer.Length);
                requestStream.Close();

                // 发送请求
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                // 获取返回的数据
                Stream res = response.GetResponseStream();
                int length = (int)response.ContentLength;
                byte[] bytes = new byte[length];
                int readCnt = 0;
                int readLength = 0;
                while (readCnt != bytes.Length)
                {
                    readLength = res.Read(bytes, readCnt, bytes.Length - readCnt);
                    readCnt = readCnt + readLength;
                }

                // 解析二进制数据

                // index为boundry加上\r\n的长度
                byte[] split = System.Text.Encoding.ASCII.GetBytes("\r\n\r\n");

                // firstBoundaryEnd为不包含\r\n的第一个boundary的结束位置
                int firstBoundaryEnd = Locate(bytes, 0, split);

                byte[] boundary = new byte[firstBoundaryEnd];
                Stream stream = new MemoryStream(bytes);
                stream.Read(boundary, 0, firstBoundaryEnd);

                int blockDatBegin = firstBoundaryEnd + 4;
                int blockDataEnd = Locate(bytes, firstBoundaryEnd, boundary) - 2;

                byte[] blockData = new byte[blockDataEnd - blockDatBegin];
                stream.Seek(blockDatBegin, SeekOrigin.Begin);
                stream.Read(blockData, 0, blockDataEnd - blockDatBegin);

                //int jsonDataBegin = blockDataEnd + 2 + boundary.Length + 4;
                //int jsonDataEnd = Locate(bytes, jsonDataBegin, boundary) - 2;

                //byte[] jsonData = new byte[jsonDataEnd - jsonDataBegin];
                //stream.Seek(jsonDataBegin, SeekOrigin.Begin);
                //stream.Read(jsonData, 0, jsonDataEnd - jsonDataBegin);

                //JObject resJo = JObject.Parse(encoding.GetString(jsonData));               

                //string tmp = String.Format("DownloadFileBlock({0}-{1}, {2}/{3})", docId, sn, downSize, allSize);
                logger.DebugFormat("DownloadFileBlock({0}-{1}, {2})", docId, sn, blockData.Length );
                //Console.WriteLine(tmp);

                return blockData;
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("DownloadFileBlock({0}-{1}) ERROR:{2}", docId, sn, ex.Message, ex);
                throw ex;
            }
        }

        public static string PostFileBlock(string url, string docId, bool hasMore, int sn, string name, byte[] blockData, int length, ref string rev)
        {
            string tmp = String.Format("PostFileBlock({0}-{1}, {2}, {3}, {4})", docId, sn, name, length, rev);
            Console.WriteLine(tmp);

            byte[] buffer = null;

            string tmpStr = string.Format("{0:N}", Guid.NewGuid());
            string boundary = "--" + tmpStr + "\r\n" + "\r\n";

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Method = "POST";
            webRequest.ContentType = "multipart/form-data; boundary=" + tmpStr;

            Stream requestStream = webRequest.GetRequestStream();

            buffer = Encoding.Default.GetBytes(boundary);
            requestStream.Write(buffer, 0, buffer.Length);

            requestStream.Write(blockData, 0, length);

            buffer = Encoding.Default.GetBytes("\r\n");
            requestStream.Write(buffer, 0, buffer.Length);

            buffer = Encoding.Default.GetBytes(boundary);
            requestStream.Write(buffer, 0, buffer.Length);

            JObject jo = new JObject();
            jo.Add(new JProperty("docid", docId));
            jo.Add(new JProperty("more", hasMore));
            jo.Add(new JProperty("sn", sn));
            jo.Add(new JProperty("rev", rev));
            jo.Add(new JProperty("name", name));
            jo.Add(new JProperty("ondup", 1));
            jo.Add(new JProperty("length", length));

            string jsonStr = jo.ToString();
            buffer = encoding.GetBytes(jsonStr);

            requestStream.Write(buffer, 0, buffer.Length);

            string endBoundary = "\r\n" + "--" + tmpStr + "--";
            buffer = Encoding.Default.GetBytes(endBoundary);
            requestStream.Write(buffer, 0, buffer.Length);

            requestStream.Close();

            HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();

            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("UTF-8"));
            string retString = reader.ReadToEnd();

            JObject retjo = JObject.Parse(retString);

            rev = retjo["rev"].ToString();
            string retDocId = retjo["docid"].ToString();
            return retDocId;
        }

        private static int Locate(byte[] src, int start, byte[] candidate)
        {

            if (IsEmptyLocate(src, candidate))
                return -1;

            for (int i = start; i < src.Length; i++)
            {
                if (!IsMatch(src, i, candidate))
                    continue;

                return i;
            }

            return -1;
        }

        private static bool IsMatch(byte[] array, int position, byte[] candidate)
        {
            if (candidate.Length > (array.Length - position))
                return false;

            for (int i = 0; i < candidate.Length; i++)
                if (array[position + i] != candidate[i])
                    return false;

            return true;
        }

        private static bool IsEmptyLocate(byte[] array, byte[] candidate)
        {
            return array == null
                || candidate == null
                || array.Length == 0
                || candidate.Length == 0
                || candidate.Length > array.Length;
        }

        private static string RSAEncrypt(string content)
        {
            //string publickey = @"<RSAKeyValue><Modulus>uyS9A3GjFB7pknYcV08aogAQQgxEYUSSLADwfvs8dSDYEhCjxm3sQ7daI3DQHNHyPhv8k7kHIB9RFvKaLIFJ4tJnExOgp45FW7/CC4ArocvuHrvtpQKQ8EDw/U6+ifJNtUbrtrFleWdVUbkBahpv3Ob2kzkBOVRTiFz1U2mtuZk=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(publickey);

            byte[] cipherbytes;
            cipherbytes = rsa.Encrypt(encoding.GetBytes(content), false);

            // 由于windows 使用的\r\n对 base64进行换行，而服务器rsa只支持 \n换行的base64 串，故将\r\n替换成 \n
            return Convert.ToBase64String(cipherbytes, Base64FormattingOptions.InsertLineBreaks).Replace("\r\n", "\n");
        }

        public static bool Login(string userName, string password)
        {
            return Login(userName, password, false);
        }

        public static bool Login(string userName, string password,bool isEncrptPassword)
        {
            if (!isEncrptPassword)
            {
                password = RSAEncrypt(password);
            }
            string json = "{ \"account\": " + "\"" + userName + "\"," + "\"password" + "\":\"" + password + "\"}";
            try
            {
                string url = string.Format("{0}/auth1?method=getnew", anyshareauthservice);
                string res = HttpPost(url, json);
                JObject jobj = JObject.Parse(res);
                ticket.UserId = jobj["userid"].Value<string>();
                ticket.TokenId = jobj["tokenid"].Value<string>();
                ticket.Timeout = jobj["expires"].Value<int>();
                ticket.LoginTime = DateTime.Now;
                return true;
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("云平台登录失败：{0}", ex.Message, ex);
                return false;
            }
        }
    }
}
