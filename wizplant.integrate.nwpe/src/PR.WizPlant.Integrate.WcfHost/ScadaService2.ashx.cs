using Common.Logging;
using PR.WizPlant.Integrate.Csgii.SBTZSOA;
using PR.WizPlant.Integrate.Scada;
using PR.WizPlant.Integrate.WcfHost.utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace PR.WizPlant.Integrate.WcfHost
{
    /// <summary>
    /// CsgiiSBTZService 的摘要说明
    /// </summary>
    public class ScadaService2 : IHttpHandler
    {
        public ILog logger = LogManager.GetLogger<ScadaService2>();
        
        string errorPrex = "ERROR";
        public static string[] userTokens = System.Configuration.ConfigurationManager.AppSettings["userToken"].Split(',');

        public void ProcessRequest(HttpContext context)
        {
            string jsonP = context.Request["jsonPCallback"];

            // 方法名称，暂时未用
            string methodName = context.Request["methodName"];

            

            // 用户身份
            string userToken = context.Request["userToken"];

            var resp = context.Response;
            resp.ContentType = "text/plain";

            if (!checkUser(userToken))
            {
                resp.Write(buildError(string.Format("{0}:用户身份验证失败!", errorPrex), jsonP));
                return;
            }

            if (string.IsNullOrEmpty(methodName))
            {
                resp.Write(buildError(string.Format("{0}:参数methodName没有提供!", errorPrex), jsonP));
                return;
            }

            

            string result = null;
            string param = null;
            string date = null;
            string[] tags = null;
            switch (methodName)
            {
                // GetObjectTags
                case "001":                   
                    param = context.Request["objId"];
                    if (string.IsNullOrEmpty(param))
                    {
                        resp.Write(buildError(string.Format("{0}:参数objId没有提供!", errorPrex), jsonP));
                        return;
                    }
                    result = MongDbScadaService.GetObjectTags(param);
                    break;
                // GetTagModelValue
                case "002":
                    // 测点集   
                    param = context.Request["pointListConfig"];
                    if (string.IsNullOrEmpty(param))
                    {
                        resp.Write(buildError(string.Format("{0}:参数pointListConfig没有提供!", errorPrex), jsonP));
                        return;
                    }
                    string pointList = System.Configuration.ConfigurationManager.AppSettings[param];
                    tags = pointList.Split(',');

                    date = context.Request["date"];
                    if (string.IsNullOrEmpty(date))
                    {
                        date = DateTime.Now.ToString("yyyyMMdd");
                    }

                    string isShowDetail = context.Request["isShowDetail"];
                    if (string.IsNullOrEmpty(isShowDetail))
                    {
                        isShowDetail = "0";
                    }
                    result = MongDbScadaService.GetTagModelValue(int.Parse(date), tags, Convert.ToBoolean(int.Parse(isShowDetail)));
                    break;
                // GetTagDetailValue
                case "003":
                    // 测点集   
                    param = context.Request["pointIdStr"];//测点Id使用'|'分隔                    
                    if (string.IsNullOrEmpty(param))
                    {
                        resp.Write(buildError(string.Format("{0}:参数pointIdStr没有提供!", errorPrex), jsonP));
                        return;
                    }
                    tags = param.Split('|');
                     date = context.Request["date"];
                    if (string.IsNullOrEmpty(date))
                    {
                        date = DateTime.Now.ToString("yyyyMMdd");
                    }

                    string onlyLast = context.Request["onlyLast"];
                    if (string.IsNullOrEmpty(onlyLast))
                    {
                        onlyLast = "1";
                    }
                    result = MongDbScadaService.GetTagDetailValue(int.Parse(date), tags, Convert.ToBoolean(int.Parse(onlyLast)));
                    break;
                // GetTagListValue
                case "004":
                    // 测点集   
                    param = context.Request["pointListConfig"];
                    if (string.IsNullOrEmpty(param))
                    {
                        resp.Write(buildError(string.Format("{0}:参数pointListConfig没有提供!", errorPrex), jsonP));
                        return;
                    }
                    tags = System.Configuration.ConfigurationManager.AppSettings[param].Split(',');

                    date = context.Request["date"];
                    if (string.IsNullOrEmpty(date))
                    {
                        date = DateTime.Now.ToString("yyyyMMdd");
                    }

                    result = MongDbScadaService.GetTagListValue(int.Parse(date), tags);
                    break;
            }

            
            if (!string.IsNullOrEmpty(jsonP))
            {
                resp.Write(jsonP + "(" + result + ")");
            }
            else
            {
                resp.Write(result);
            }


           
        }

        public string buildError(string msg,string jsonP)
        {
            if (string.IsNullOrEmpty(jsonP))
            {
            return msg;
            }
            else
            {
                return jsonP + "(\"" + msg + "\")";
            }
        }

        /// <summary>
        /// 用户身份验证
        /// </summary>
        /// <param name="userToken">用户身份信息</param>
        /// <returns>是否成功</returns>
        bool checkUser(string userToken)
        {
           return userTokens.Contains(userToken);
        }


     

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}