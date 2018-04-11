using PR.WizPlant.Integrate.Sql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace PR.WizPlant.Integrate.WcfHost.utilities
{
    public static class VideoMonitorHelper
    {
        private static Dictionary<string, string> cameraCache = null;
        public static Dictionary<string, string> CameraCache
        {
            get
            {
                if (cameraCache == null)
                {
                    cameraCache = new Dictionary<string, string>();
                    StringBuilder sb = new StringBuilder();
                    string selectSql = @" SELECT [MonitorIp]
      ,[MonitorPort]
      ,[UserName]
      ,[Password]
      ,[Name]     
      ,[ChannelNo]
      ,[ObjectId]
  FROM [PRW_Inte_VideoMonitor_CameraMap] where objectId is not null";
                    using (var reader = SqlHelper.ExecuteDataReader(SqlHelper.DefaultConnectionString, CommandType.Text, selectSql))
                    {
                        while (reader.Read())
                        {
                            sb.Clear();
                            string objId = reader.GetGuid(6).ToString().ToLower();
                            sb.Append("{")                                
                                .AppendFormat("\"MonitorIp\":\"{0}\"", reader.GetString(0))
                                .AppendFormat(",\"MonitorPort\":\"{0}\"", reader.GetString(1))
                                .AppendFormat(",\"UserName\":\"{0}\"", reader.GetString(2))
                                 .AppendFormat(",\"Password\":\"{0}\"", reader.GetString(3))
                                  .AppendFormat(",\"Name\":\"{0}\"", reader.GetString(4))
                                .AppendFormat(",\"ChannelNo\":\"{0}\"", reader.GetString(5))
                                .AppendFormat(",\"ObjectId\":\"{0}\"", objId)
                                .Append("}");

                            cameraCache[objId] = sb.ToString();
                        }
                    }
                }
                return cameraCache;
            }
        }
    }
}