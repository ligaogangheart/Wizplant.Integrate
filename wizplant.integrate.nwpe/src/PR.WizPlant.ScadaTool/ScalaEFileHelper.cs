using Common.Logging;
using PR.WizPlant.Integrate.Sql;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR.WizPlant.ScadaTool
{
    public class ScalaEFileHelper
    {
         static List<String> activeTagIds = null;

         static string connectionString = System.Configuration.ConfigurationManager.AppSettings["strConn"];
         private static ILog logger = LogManager.GetLogger<ScalaEFileHelper>();

         public static List<String> ActiveTagIds
         {
             get
             {
                 if (activeTagIds == null)
                 {
                     activeTagIds = new List<string>();
                     string sql = @"select distinct(TagId) from [dbo].[PRW_Inte_SCADA_Map]";

                     using (var dr = SqlHelper.ExecuteDataReader(connectionString, sql))
                     {
                         while (dr.Read())
                         {
                             string point = dr["TagId"].ToString();
                             activeTagIds.Add(point);
                         }
                     }
                    
                 }
                 return activeTagIds;
             }
         }

         public static bool ScanEFileToDb(string filename)
         {
             if (!File.Exists(filename))
             {
                 log(string.Format("文件:{0}不存在!",filename));
                 return false;
             }
             DateTime monitorTime;
             bool result = getMonitorTimeByFileName(filename, out monitorTime);
             if (result)
             {
                 result = ScanEFileToDb(filename, monitorTime);
             }

             return result;

         }

         public static bool ScanEFileToDb(string filename, DateTime monitorTime)
        {
            if (!File.Exists(filename))
            {
                log(string.Format("文件:{0}不存在!", filename));
                return false;
            }

            List<string> lables = new List<string>{ "<ValueInfo>", "<YxValueInfo>" };

            
            string line;
            string beginTag = null;
            string endTag = null;
            string type = null;

            int toDoCnt = lables.Count;
            int doneCnt = 0;


            var dt = SqlHelper.ExecuteEmptyDataTable(connectionString, "PRW_Inte_SCADA_EFileData");
            dt.TableName = "PRW_Inte_SCADA_EFileData";
            DataRow dr = null;

            using (StreamReader sr = new StreamReader(filename, Encoding.Default))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    // 还未读取到开始标志
                    if (beginTag == null)
                    {
                        int idx = lables.IndexOf(line);
                        // 是开始标志
                        if (idx > -1)
                        {
                            // 开始标志
                            beginTag = line;
                            // 结束标志
                            endTag = "</" + beginTag.Substring(1);

                            // 测点类型
                            if (idx == 0)
                            {
                                type = "YC";
                            }
                            else
                            {
                                type = "YX";
                            }
                        }
                    }
                    else
                    {
                        // 读取结束标志
                        if (line == endTag)
                        {
                            beginTag = null;
                            endTag = null;
                            type = null;
                            // 已处理数+1
                            if (++doneCnt >= toDoCnt)
                            {
                                break;
                            }
                        }
                        else if (line.StartsWith("#"))
                        {
                            // 监测数据
                            var tagData = line.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);

                            if (ActiveTagIds.Contains(tagData[2]))
                            {
                                dr = dt.NewRow();
                                dt.Rows.Add(dr);

                                dr["Id"] = Guid.NewGuid();
                                dr["CreateDate"] = monitorTime.ToString("yyyyMMdd");
                                dr["CreateTime"] = monitorTime.ToString("HHmmss");
                                dr["SiteName"] = tagData[1];
                                dr["TagId"] = tagData[2];
                                dr["TagName"] = tagData[3];
                                dr["MeasureValue"] = tagData[4];
                                dr["Status"] = Convert.ToBoolean(int.Parse(tagData[5]));
                                dr["Type"] = type;
                            }
                        }
                    }
                }
            }

            if (dt.Rows.Count > 0)
            {
                SqlHelper.BulkInsert(connectionString, dt);                
            }

            return true;

        }

        /// <summary>
        /// 根据文件名解析时间
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
         static bool getMonitorTimeByFileName(string fullpath, out DateTime monitorTime)
        {
            monitorTime = DateTime.MinValue;
            string filename = Path.GetFileName(fullpath);
            try
            {
                string start = "普洱_";
                string end = "_SCADA.DT";
                string timestr = filename.Replace(start, "").Replace(end, "");//移除前后
                string time = timestr.Split('_')[0].Substring(0, 4) + "-" + timestr.Split('_')[0].Substring(4, 2) + "-" + timestr.Split('_')[0].Substring(6, 2) + " " + timestr.Split('_')[1].Substring(0, 2) + ":" + timestr.Split('_')[1].Substring(2, 2) + ":" + timestr.Split('_')[1].Substring(4, 2);
                monitorTime = Convert.ToDateTime(time);
                return true;
            }                
            catch (Exception ex)
            {
                error(string.Format("文件{0}命名不规范，处理失败!", filename),ex);
                return false;
               // throw new Exception("时间解析错误！" + ex.Message.ToString());
            }
        }

        static void log(string message)
        {
            Console.WriteLine("{0:HH:mm:ss}  {1}", DateTime.Now, message);
            logger.Info(message);
        }

        static void error(string message,Exception ex)
        {
            Console.WriteLine("{0:HH:mm:ss}  {1}", DateTime.Now, message);
            if (ex == null)
            {
                logger.Error(message);
            }
            else
            {
                logger.Error(message, ex);
            }
        }
    }
}
