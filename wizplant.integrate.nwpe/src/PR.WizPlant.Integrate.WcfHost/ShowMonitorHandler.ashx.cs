using Newtonsoft.Json.Linq;
using PR.WizPlant.Integrate.Sql;
using PR.WizPlant.Integrate.WcfHost.utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;

namespace PR.WizPlant.Integrate.WcfHost
{
    /// <summary>
    /// ShowMonitorHandler 的摘要说明
    /// </summary>
    public class ShowMonitorHandler : IHttpHandler, IRequiresSessionState
    {
        string ErrorPrex = "ERROR";
      
        public void ProcessRequest(HttpContext context)
        {
            string methodName = context.Request["methodName"];

            string jsonP = context.Request["jsonPCallback"];

            string result = null;
            switch (methodName)
            {
                case "001":
                    string objectId = context.Request["objectId"];
                    if (string.IsNullOrEmpty(objectId))
                    {
                        result = string.Format("{0}:没有提供对象Id", ErrorPrex);
                    }
                    else
                    {
                        var key = objectId.ToLower();
                        if (VideoMonitorHelper.CameraCache.ContainsKey(key))
                        {
                            result = VideoMonitorHelper.CameraCache[key];
                        }

                        if (string.IsNullOrEmpty(result))
                        {
                            result = string.Format("{0}:对象[{1}]不存在监控摄像头", ErrorPrex, objectId);
                        }
                    }
                    break;
                case "002":
                    string monitorIp = context.Request["monitorIp"];
                    string channelNo = context.Request["channelNo"];
                    if (string.IsNullOrEmpty(monitorIp) || string.IsNullOrEmpty(channelNo))
                    {
                        result = string.Format("{0}:没有提供Ip或通道号", ErrorPrex);
                    }
                    else 
                    {
                        Dictionary<string, string> dic = VideoMonitorHelper.CameraCache.Where(p => JObject.Parse(p.Value)["MonitorIp"].ToString() == monitorIp && JObject.Parse(p.Value)["ChannelNo"].ToString() == channelNo).ToDictionary(p => p.Key, p => p.Value);
                        if (dic.Count > 0)
                        {
                            result = dic.First().Value;
                        }
                        else
                        {
                            result = string.Format("{0}:根据Ip[{1}],通道号[{2}未找到对应摄像头]", ErrorPrex, monitorIp, channelNo);
                        }
                    }
                    break;
            }

            context.Response.ContentType = "text/plain";
            context.Response.Write(buildInfo(result, jsonP));
        }

        public string buildInfo(string msg, string jsonP)
        {
            if (string.IsNullOrEmpty(jsonP))
            {
                return msg;
            }
            else
            {
                return jsonP + "('" + msg + "')";
            }
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