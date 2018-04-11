using Common.Logging;
using Newtonsoft.Json.Linq;
using PR.WizPlant.Integrate.AnyShare;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.Web;

namespace PR.WizPlant.Integrate.WcfHost
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“NWCloudRestWcfService”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 NWCloudRestWcfService.svc 或 NWCloudRestWcfService.svc.cs，然后开始调试。
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class AnyShareRestWcfService : IAnyShareRestWcfService
    {
        static ILog logger = LogManager.GetLogger<AnyShareRestWcfService>();
        static string fileservice = System.Configuration.ConfigurationManager.AppSettings["anysharefileservice"];
        static string errorPrex = "ERROR";
        static bool needTestData = ConfigurationManager.AppSettings["anysharetest"] == "true";


        /// <summary>
        /// 根据对象Id获取云平台对应目录gns路径
        /// </summary>
        /// <param name="objId">对象Id</param>
        /// <returns></returns>
        private string getCloudDirId(string objId)
        {
            string result = null;
            var key = objId.ToLower();
            if (AnyShareProxy.MapCache.ContainsKey(key))
            {
                result = AnyShareProxy.MapCache[key];
            }

            if (result == null && needTestData)
            {
                result = objId;
            }
            return result;
        }
        /// <summary>
        /// 根据对象Id获取对象关联云平台文件列表
        /// </summary>
        /// <param name="objId">对象Id</param>
        /// <param name="pageNo">页码</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="totalRecords">总记录数，如果为-1表示需要计算总记录数，如果为大于-1的整数则表示为总记录数，则服务不再计算总记录数</param>
        /// <returns>{"total":1000,"pages":50,"docs":[{"locationId":"gns","fileName":"xyz","length":12345},{"locationId":"gns","fileName":"xyz","length":12345}]}</returns>
        [WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "GetPagedAssociationDocs")]
        public string GetFileList(string objId,int pageNo,int pageSize,int totalRecords)
        {
            if (needTestData)
            {
                return GetFileListByCloudDirId(objId, pageNo, pageSize, totalRecords);
            }
            var ticket = AnyShareProxy.Ticket;
            if (!ticket.IsAuth)
            {
                string msg = string.Format("{0}:没有权限使用",errorPrex);
                logger.Error(msg);
                return msg;
            }

            var dirId = getCloudDirId(objId);

            if (string.IsNullOrEmpty(dirId))
            {
                string msg = string.Format("{0}:还没有为对象{1}创建云目录", errorPrex, objId);
                logger.Error(msg);
                return msg;
            }
            return GetFileListByCloudDirId(dirId, pageNo,pageSize,totalRecords);
        }

        /// <summary>
        /// 根据云平台目录Id获取目录下文件列表，分页
        /// </summary>
        /// <param name="dirId">目录对象Id</param>
        /// <param name="pageNo">页码</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="totalRecords">总记录数，如果为-1表示需要计算总记录数，如果为大于-1的整数则表示为总记录数，则服务不再计算总记录数</param>
        /// <returns>{"total":1000,"pages":50,"docs":[{"locationId":"gns","fileName":"xyz","length":12345},{"locationId":"gns","fileName":"xyz","length":12345}]}</returns>
        [WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "GetPagedDocsByDirId")]
        public string GetFileListByCloudDirId(string dirId, int pageNo, int pageSize, int totalRecords)
        {
            if (needTestData)
            {
                string testData = "{\"total\":\"1\",\"pages\":\"1\",\"docs\":[{\"locationId\":\"gns://test\",\"fileName\":\"test.txt\",\"length\":100}]}";

                return testData;
            }
            var ticket = AnyShareProxy.Ticket;
            if (!ticket.IsAuth)
            {
                string msg = string.Format("{0}:没有权限使用", errorPrex);
                logger.Error(msg);
                return msg;
            }

            var cache = HttpContext.Current.Cache[dirId] as List<JObject>;
            if (cache == null)
            {
                string json = null;
                try
                {
                    string url = string.Format("{0}/dir?method=list&userid={1}&tokenid={2}", fileservice, ticket.UserId, ticket.TokenId);
                    string request = "{\"docid\":\"" + dirId + "\",\"by\":\"name\"}";
                    logger.DebugFormat("{0},{1}", url, request);
                    json = AnyShareProxy.HttpPost(url, request);
                }
                catch (Exception ex)
                {
                    string msg = string.Format("{0}:调用AnyShareProxy出错:{1}", errorPrex, ex.Message);
                    logger.Error(msg, ex);
                    return msg;
                }
                var jobj = JObject.Parse(json);
                cache = jobj["files"].Values<JObject>().ToList();
                if (cache.Count > pageSize)
                {
                    HttpContext.Current.Cache.Add(dirId, cache, null, DateTime.Now.AddMinutes(1), System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Default, null);
                }
            }
            StringBuilder sb = new StringBuilder();
            var pages = cache.Count / pageSize;
            if (cache.Count % pageSize > 0)
            {
                pages += 1;
            }
            sb.Append("{\"total\":\"").Append(cache.Count).Append("\",\"pages\":\"").Append(pages).Append("\",\"docs\":");
            sb.Append("[");
            var curPage = cache.Skip((pageNo - 1) * pageSize).Take(pageSize);
            if (curPage != null && curPage.Count() > 0)
            {
                foreach (var file in curPage)
                {
                    sb.Append("{\"locationId\":\"")
                        .Append(file["docid"].Value<string>())
                        .Append("\",\"fileName\":\"")
                        .Append(file["name"].Value<string>())
                        .Append("\",\"length\":").Append(file["size"].Value<long>()).Append("},");
                }
                sb.Remove(sb.Length - 1, 1);
            }
            sb.Append("]}");
            return sb.ToString();
        }

        /// <summary>
        /// 根据对象Id获取对象关联云平台文件和子目录列表
        /// </summary>
        /// <param name="objId">对象Id</param>
        /// <returns>{"docs":[{"locationId":"gns","fileName":"xyz","length":12345},{"locationId":"gns","fileName":"xyz","length":12345}],"dirs":[{"locationId":"gns","dirName":"xyz","length":12345},{"locationId":"gns","dirName":"xyz","length":12345}]}</returns>
        [WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "GetAssociationSubList")]
        public string GetSubList(string objId)
        {
            logger.DebugFormat("Enter GetAssociationSubList({0})", objId);
            try
            {
                if (needTestData)
                {
                    string testData = "{\"docs\":[{\"locationId\":\"gns://test\",\"fileName\":\"test.txt\",\"length\":100}],\"dirs\":[]}";

                    return testData;
                }
                var ticket = AnyShareProxy.Ticket;
                if (!ticket.IsAuth)
                {
                    string msg = string.Format("{0}:没有权限使用", errorPrex);
                    logger.Error(msg);
                    return msg;
                }
                string dirId = getCloudDirId(objId);
                if (string.IsNullOrEmpty(dirId))
                {
                    string msg = string.Format("{0}:还没有为对象{1}创建云目录", errorPrex, objId);
                    logger.Error(msg);
                    return msg;
                }

                return GetSubListByCloudDirId(dirId);
            }
            catch (Exception ex)
            {
                string msg = string.Format("{0}:GetAssociationSubList方法报错。msg:{1}", errorPrex, ex.Message.ToString());
                logger.Error(msg);
                return msg;
            }
        }

        /// <summary>
        /// 根据云平台目录Id获取目录下文件和子目录列表
        /// </summary>
        /// <param name="dirId">目录对象Id</param>       
        /// <returns>{"docs":[{"locationId":"gns","fileName":"xyz","length":12345},{"locationId":"gns","fileName":"xyz","length":12345}],"dirs":[{"locationId":"gns","dirName":"xyz","length":12345},{"locationId":"gns","dirName":"xyz","length":12345}]}</returns>
        [WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "GetSubListByDirId")]
        public string GetSubListByCloudDirId(string dirId)
        {
            logger.DebugFormat("Enter GetSubListByDirId({0})", dirId);
            if (needTestData)
            {
                string testData = "{\"docs\":[{\"locationId\":\"gns://test\",\"fileName\":\"test.txt\",\"length\":100}],\"dirs\":[]}";

                return testData;
            }
            var ticket = AnyShareProxy.Ticket;
            if (!ticket.IsAuth)
            {
                string msg = string.Format("{0}:没有权限使用", errorPrex);
                logger.Error(msg);
                return msg;
            }            

            string json = null;
            try
            {
                string url = string.Format("{0}/dir?method=list&userid={1}&tokenid={2}", fileservice, ticket.UserId, ticket.TokenId);
                string request = "{\"docid\":\"" + dirId + "\",\"by\":\"name\"}";
                logger.DebugFormat("{0},{1}", url, request);
                json = AnyShareProxy.HttpPost(url, request);
            }
            catch (Exception ex)
            {
                string msg = string.Format("{0}:调用AnyShareProxy出错:{1}", errorPrex, ex.Message);
                logger.Error(msg, ex);
                return msg;
            }
            StringBuilder sb = new StringBuilder();

            try
            {
                var jobj = JObject.Parse(json);

                sb.Append("{");

                var cache = jobj["files"].Values<JObject>().ToList();
                sb.Append("\"docs\":");
                sb.Append("[");
                if (cache != null && cache.Count > 0)
                {
                    foreach (var file in cache)
                    {
                        sb.Append("{\"locationId\":\"")
                            .Append(file["docid"].Value<string>())
                            .Append("\",\"fileName\":\"")
                            .Append(file["name"].Value<string>())
                            .Append("\",\"length\":").Append(file["size"].Value<long>()).Append("},");
                    }
                    sb.Remove(sb.Length - 1, 1);
                }
                sb.Append("],");

                cache = jobj["dirs"].Values<JObject>().ToList();
                sb.Append("\"dirs\":");
                sb.Append("[");
                if (cache != null && cache.Count > 0)
                {
                    foreach (var file in cache)
                    {
                        sb.Append("{\"locationId\":\"")
                            .Append(file["docid"].Value<string>())
                            .Append("\",\"dirName\":\"")
                            .Append(file["name"].Value<string>())
                            .Append("\",\"length\":").Append(file["size"].Value<long>()).Append("},");
                    }
                    sb.Remove(sb.Length - 1, 1);
                }
                sb.Append("]}");

                logger.DebugFormat("result:{0}", sb.ToString());
                return sb.ToString();
            }
            catch (Exception ex)
            {
                string msg = string.Format("{0}:调用AnyShareProxy出错:{1}", errorPrex, ex.Message);
                logger.Error(msg, ex);
                return msg;
            }
            
        }


        /// <summary>
        /// 根据文件名关键字在指定对象的云文档目录中查找文件
        /// </summary>
        /// <param name="objId">对象Id</param>
        /// /// <param name="key">要查询的文件名，模糊匹配，不含后缀名</param>
        /// <param name="pageNo">页码</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="totalRecords">总记录数，如果为-1表示需要计算总记录数，如果为大于-1的整数则表示为总记录数，则服务不再计算总记录数</param>
        /// <returns>{"total":1000,"pages":50,"docs":[{"locationId":"gns","fileName":"xyz","length":12345},{"locationId":"gns","fileName":"xyz","length":12345}]}</returns>
        [WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "SearchPagedFileList")]
        public string SearchFileList(string objId,string key, int pageNo, int pageSize, int totalRecords)
        {
            if (needTestData)
            {
                return SearchFileListByCloudDirId(objId,key, pageNo, pageSize, totalRecords);
            }
            if (string.IsNullOrEmpty(key))
            {
                string msg = string.Format("{0}:关键字key不能为空", errorPrex);
                logger.Error(msg);
                return msg;
            }
            var ticket = AnyShareProxy.Ticket;
            if (!ticket.IsAuth)
            {
                string msg = string.Format("{0}:没有权限使用", errorPrex);
                logger.Error(msg);
                return msg;
            }

            var dirId = getCloudDirId(objId);

            if (string.IsNullOrEmpty(dirId))
            {
                string msg = string.Format("{0}:还没有为对象{1}创建云目录", errorPrex, objId);
                logger.Error(msg);
                return msg;
            }
            return SearchFileListByCloudDirId(dirId, key, pageNo, pageSize, totalRecords);
        }

        /// <summary>
        /// 根据文件名关键字在云文档指定目录中查找文件
        /// </summary>
        /// <param name="dirId">目录对象Id</param>
        /// <param name="key">要查询的文件名，模糊匹配，不含后缀名</param>
        /// <param name="pageNo">页码</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="totalRecords">总记录数，如果为-1表示需要计算总记录数，如果为大于-1的整数则表示为总记录数，则服务不再计算总记录数</param>
        /// <returns>{"total":1000,"pages":50,"docs":[{"locationId":"gns","fileName":"xyz","length":12345},{"locationId":"gns","fileName":"xyz","length":12345}]}</returns>
        [WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "SearchPageFileListByCloudDirId")]
        public string SearchFileListByCloudDirId(string dirId,string key, int pageNo, int pageSize, int totalRecords)
        {
            
            if (needTestData)
            {
                string testData = "{\"total\":\"1\",\"pages\":\"1\",\"docs\":[{\"locationId\":\"gns://test\",\"fileName\":\"test.txt\",\"length\":100}]}";

                return testData;
            }
            if (string.IsNullOrEmpty(key))
            {
                string msg = string.Format("{0}:关键字key不能为空", errorPrex);
                logger.Error(msg);
                return msg;
            }
            var ticket = AnyShareProxy.Ticket;
            if (!ticket.IsAuth)
            {
                string msg = string.Format("{0}:没有权限使用", errorPrex);
                logger.Error(msg);
                return msg;
            }

            if (pageNo < 1) pageNo = 1;
            if (pageSize < 1) pageSize = 20;

            if (pageNo > 1)
            {
                if (totalRecords < 0)
                {
                    string msg = string.Format("{0}:当页码大于1时必须提供总记录数totalRecords", errorPrex);
                    logger.Error(msg);
                    return msg;
                }
            }

            int start = (pageNo - 1) * pageSize;

            
                string json = null;
                try
                {
                    string url = string.Format("{0}/search?method=search&userid={1}&tokenid={2}", fileservice, ticket.UserId, ticket.TokenId);
                    StringBuilder requestBuilder = new StringBuilder();
                    requestBuilder.Append("{")
                        .Append("\"start\":").Append(start)
                        .Append(",\"rows\":").Append(20)
                        .Append(",\"range\":[\"").Append(dirId.Replace(":","?").Replace("/","\\/")).Append("\\/*\"]")
                    .Append(",\"keys\":\"").Append(key).Append("\"")
                    .Append(",\"keysfields\":[\"basename\"]")
                    //.Append(",\"ext\":[\".*\"]")
                    .Append("}");
                    string request = requestBuilder.ToString();
                    logger.DebugFormat("{0},{1}", url, request);
                    json = AnyShareProxy.HttpPost(url, request);
                }
                catch (Exception ex)
                {
                    string msg = string.Format("{0}:调用SearchPageFileListByCloudDirId出错:{1}", errorPrex, ex.Message);
                    logger.Error(msg, ex);
                    return msg;
                }

            JObject jobj = null;
            try
            {
                jobj = JObject.Parse(json);
                jobj = jobj["response"].Value<JObject>();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Parse JObject Error:{0}", ex.Message, ex);
                logger.ErrorFormat("response json : {0}", json);
                string msg = string.Format("{0}:调用SearchPageFileListByCloudDirId出错:{1}", errorPrex, ex.Message);
                logger.Error(msg, ex);
                return msg;
            }

                if (start == 0)
                {
                    totalRecords = jobj["hits"].Value<int>();
                }              
            
            StringBuilder sb = new StringBuilder();

            var pages = totalRecords / pageSize;
            if (totalRecords % pageSize > 0)
            {
                pages += 1;
            }

            sb.Append("{\"total\":\"").Append(totalRecords).Append("\",\"pages\":\"").Append(pages).Append("\",\"docs\":");
            sb.Append("[");
            var curPage = jobj["docs"].Values<JObject>().ToList();
            if (curPage != null && curPage.Count() > 0)
            {
                foreach (var file in curPage)
                {
                    sb.Append("{\"locationId\":\"")
                        .Append(file["docid"].Value<string>())
                        .Append("\",\"fileName\":\"")
                        .Append(file["basename"].Value<string>() + file["ext"].Value<string>())
                        .Append("\",\"length\":").Append(file["size"].Value<long>()).Append("},");
                }
                sb.Remove(sb.Length - 1, 1);
            }
            sb.Append("]}");
            return sb.ToString();
        }

    }
}
