using Common.Logging;
using PR.WizPlant.Integrate.AnyShare;
using System;
using System.Configuration;
using System.IO;
using System.Text;
using System.Web;

namespace PR.WizPlant.Integrate.WcfHost
{
    public class AnyShareService : IHttpHandler
    {
        static Encoding encoding = Encoding.UTF8;
        ILog logger = LogManager.GetLogger<AnyShareService>();

        static bool needTestData = ConfigurationManager.AppSettings["anysharetest"] == "true";

        public string AnyshareNeedDownload = ConfigurationManager.AppSettings["AnyshareNeedDownload"];

        /// <summary>
        /// 您将需要在网站的 Web.config 文件中配置此处理程序 
        /// 并向 IIS 注册它，然后才能使用它。有关详细信息，
        /// 请参见下面的链接: http://go.microsoft.com/?linkid=8101007
        /// </summary>
        #region IHttpHandler Members

        public bool IsReusable
        {
            // 如果无法为其他请求重用托管处理程序，则返回 false。
            // 如果按请求保留某些状态信息，则通常这将为 false。
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {            
            string methodName = context.Request["methodName"];

            var response = context.Response;
            if (string.IsNullOrEmpty(methodName))
            {
                logger.ErrorFormat("methodName is empty");
                ResponseError(response, "methodName cant be empty", 400);                
                return;
            }

           

            string fileservice = System.Configuration.ConfigurationManager.AppSettings["anysharefileservice"];
            if (string.IsNullOrEmpty(fileservice))
            {
                logger.ErrorFormat("anysharefileservice is not config");
                ResponseError(response, "anysharefileservice has not config", 400);                
                return;
            }
            
            if (methodName != "download")
            {
                string msg =  string.Format("method[{0}] not supportted!", methodName);
                logger.ErrorFormat(msg);
                ResponseError(response, msg, 400);
                return;
            }

            if (needTestData)
            {
                string content = "test data";
                var stream = response.OutputStream;
                
                stream.Write(encoding.GetBytes(content), 0, content.Length);

               

                //context.Response.AddHeader("Content-disposition", "attachment;filename=" + HttpUtility.UrlEncode(filename, encoding));
                context.Response.AddHeader("Content-Length", content.Length.ToString());
                string fn = "test.txt";
                string ex = Path.GetExtension(fn);
                context.Response.ContentType = MimeMappingHelper.GetMimeMapping(ex);
                context.Response.StatusCode = 200;

                logger.DebugFormat("{0} file {1}[{2}] finished", methodName, "gns://test", fn);
                context.Response.Flush();
                return;
            }

            string docId = context.Request["docId"];
            if (string.IsNullOrEmpty(docId))
            {
                logger.ErrorFormat("docId can't be empty");
                ResponseError(response, "docId can't be empty", 404);
                return;
            }

            AnyShareTicket ticket = AnyShareProxy.Ticket;
            if (!ticket.IsAuth)
            {
                logger.ErrorFormat("没有授权使用服务");
                ResponseError(context.Response, "没有授权使用服务", 401);
                return;
            }            
            string filename = context.Request["filename"];           

            bool hasMore = true;
            Int64 allSize = 0;
            Int64 downSize = 0;
            int sn = 0;

            string url = string.Format("{0}/file?method=download&userid={1}&tokenid={2}", fileservice, ticket.UserId, ticket.TokenId);

            context.Response.Clear();
            context.Response.ClearHeaders();

            logger.DebugFormat("{0} file {1}", methodName, docId);
            byte[] blockData = null;
            try
            {         
                var stream = response.OutputStream;
                string innerFileName = null;
                // first download
                blockData = AnyShareProxy.DownloadFirstFileBlock(url, docId, out allSize, out innerFileName);
                downSize = blockData.Length;
                hasMore = downSize < allSize;
                if (string.IsNullOrEmpty(filename))
                {
                    filename = innerFileName;
                }
                stream.Write(blockData, 0, blockData.Length);

                    while (hasMore)
                    {
                        sn++;
                        blockData = AnyShareProxy.DownloadFileBlock(url, docId, sn);
                        downSize += blockData.Length;
                        stream.Write(blockData, 0, blockData.Length);
                        
                        hasMore = downSize < allSize;
                    }



                string[] extensionArr = AnyshareNeedDownload.Split(',');
                bool flag = false;//true:直接下载文件，false:在线打开
                foreach (var extension in extensionArr)
                {
                    if (filename.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
                    {
                        flag = true;
                        break;
                    }
                }
                if (flag)//下载
                {
                    context.Response.AddHeader("Content-disposition", "attachment;filename=" + HttpUtility.UrlEncode(filename, encoding));
                }
                else
                {
                    context.Response.AddHeader("Content-Length", allSize.ToString());
                }
                    
                    string ex = Path.GetExtension(filename);
                    context.Response.ContentType = MimeMappingHelper.GetMimeMapping(ex);
                    context.Response.StatusCode = 200;
                
                    logger.DebugFormat("{0} file {1}[{2}] finished", methodName, docId,filename);
                    context.Response.Flush();
            }
            catch (Exception ex)
            {
                string msg =string.Format("method[{0}] Exception,Message:{1}", methodName, ex.Message);
                logger.ErrorFormat(msg,ex);
                ResponseError(response, msg, 500);
                return;
            }
        }

        #endregion

        private void ResponseError(HttpResponse response , string msg,int statusCode)
        {
            response.ContentType = "text/plain";
            response.Charset = encoding.BodyName;
            response.StatusCode = statusCode;
            response.ContentEncoding = encoding;
            string errorMsg = string.Format("ERROR:{0}", msg);
            response.BinaryWrite(encoding.GetBytes(errorMsg));
            response.End();
        }
    }
}
