
using Common.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR.WizPlant.ScadaTool
{
    public class ScanWorker : IWorker
    {
       public static string basePath = System.Configuration.ConfigurationManager.AppSettings["basePath"];//e文件存放位置
       public static string successPath = System.Configuration.ConfigurationManager.AppSettings["successPath"];//处理成功的e文件保存位置
       public static string errorPath = System.Configuration.ConfigurationManager.AppSettings["errorPath"];//处理失败的e文件保存位置
       public static string strConn = System.Configuration.ConfigurationManager.AppSettings["strConn"];//数据库连接字段
       private static ILog logger = LogManager.GetLogger<ScanWorker>();

       private static List<string> _measurePointInSqlList;
       public static List<string> MeasurePointInSqlList
       {
           get 
           {
               if (_measurePointInSqlList == null)
               {
                   _measurePointInSqlList = GetPointListInSql();
               }
               return _measurePointInSqlList; 
           }
       }
       public void Run()
       {
           //log("处理数据库历史数据开始...");
           //DealHistoryData();
           log("处理数据库历史数据结束");

           log("处理已扫描完成的e文件开始...");
           DealHistoryFile(successPath);//处理成功目录
           //DealHistoryFile(errorPath);//处理错误目录
           log("处理已扫描完成的e文件结束");
           log("开始扫描...");
           System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(basePath);
           if (!di.Exists)
           {
               log(string.Format("路径 {0} 不存在", basePath));
               return;
           }
           Stopwatch sw = new Stopwatch();
           sw.Start();
           var files = di.GetFiles("*.DT").Where(p=>p.Length > 2.4*1024*1024).ToArray();//处理大于2.4M的文件。大于2.4M可认为已传输完成
           var totalFiles = files.Length;
           log("文件总数:" + totalFiles);

           int errCnt = 0;

           if (!Directory.Exists(successPath))
           {
               Directory.CreateDirectory(successPath);
           }

           if (!Directory.Exists(errorPath))
           {
               Directory.CreateDirectory(errorPath);
           }

           string targetFile = null;

           foreach(var file in files)
           {
               string sourceFileName = basePath + "\\" + file.Name;
               string successFileName = successPath + "\\" + file.Name;               
               string errorFileName = errorPath + "\\" + file.Name;
               bool succ = false;
               
               try
               {
                   //dealFile(file);
                   succ = ScalaEFileHelper.ScanEFileToDb(sourceFileName);
                   
               }
               catch (Exception ex)
               {
                   log(string.Format("处理报错的文件名 {0} 错误信息:{1}", file.Name, ex.Message));
                   errCnt++;
                   succ = false;
               }

               if (succ)
               {
                   targetFile = successFileName;
               }
               else
               {
                   targetFile = errorFileName;
               }
               try
               {
                   File.Move(sourceFileName, targetFile);
               }
               catch (Exception ex)
               {
                   log(string.Format("移动文件{0} 到 {2} 时出错，错误信息:{3}", sourceFileName, targetFile,ex.Message));
               }
           }
           
           log("结束扫描");
           if (errCnt > 0)
           {
               log(string.Format("===========处理文件总数:{0},成功:{1},错误:{2},总耗时:{3}毫秒", totalFiles, totalFiles - errCnt, errCnt, sw.ElapsedMilliseconds));
           }
           else
           {
               log(string.Format("处理文件总数:{0},成功:{1},错误:{2},总耗时:{3}毫秒", totalFiles, totalFiles - errCnt, errCnt, sw.ElapsedMilliseconds));
           }
       }

       //获取[PRW_SCADA_DeviceMeasurePoint]表中所有测点Id，采集数据只采集数据库表中存在的测点，其他测点舍弃
       static List<string> GetPointListInSql()
       {
           List<string> list = new List<string>();
           string sql = @"select distinct(TagId) from [dbo].[PRW_SCADA_DeviceMeasurePoint]";
           SqlDataReader dr = GetReader(sql, null, CommandType.Text);
           while (dr.Read())
           {
               string point = dr["TagId"].ToString(); 
               list.Add(point);
           }
           return list;
       }

       /// <summary>
       /// 处理数据库历史数据
       /// </summary>
       void DealHistoryData()
       {
           try
           {
               //PRW_SCADA_EFileData中保存一个小时的数据，超过一个小时的数据保存到PRW_SCADA_EFileDataHistory表中
               string historyDataKeepTime = System.Configuration.ConfigurationManager.AppSettings["historyDataKeepTime"];
               string sqlinsert = @"insert into [dbo].[PRW_SCADA_EFileDataHistory]
                                    select * from [dbo].[PRW_SCADA_EFileData]
                                    where CreationTime < DATEADD(hour, -" + historyDataKeepTime + ", GETDATE())";
               int insertCount = ExcuteSQL(sqlinsert, null, CommandType.Text);
               log(string.Format("向PRW_SCADA_EFileDataHistory表插入 {0} 条记录", insertCount));

               //超过一个小时数据插入PRW_SCADA_EFileDataHistory表完成后，删除PRW_SCADA_EFileData表中超过一个小时的数据
               string sql = @"delete from [PRW_SCADA_EFileData]
                        where CreationTime < DATEADD(hour, -" + historyDataKeepTime + ", GETDATE())";
               int deleteCount = ExcuteSQL(sql, null, CommandType.Text);
               log(string.Format("从PRW_SCADA_EFileData表删除 {0} 条记录", deleteCount));
           }
           catch (Exception ex)
           {
               throw new Exception("处理数据库历史数据错误！" + ex.Message.ToString());
           }
       }

       /// <summary>
       /// 处理扫描成功和扫描错误的e文件，保留一天的文件，根据文件名上的时间来判断。
       /// </summary>
       /// <param name="path">需处理历史文件所在的路径</param>
       void DealHistoryFile(string path)
       {
           try 
           {
               //文件保存时间
               string historyFileKeepTime = System.Configuration.ConfigurationManager.AppSettings["historyFileKeepTime"];
               System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(path);
               if (!di.Exists)
               {
                   log(string.Format("已扫描文件处理：路径 {0} 不存在", path));
                   return;
               }
               var files = di.GetFiles("*.DT");
               int deleteFileCount = 0;
               foreach (var file in files)
               {
                   string fileName = file.Name;
                   DateTime time = GetTimeByFileName(fileName);
                   if (time < DateTime.Now.AddHours(-Convert.ToInt32(historyFileKeepTime)))
                   {
                       file.Delete();
                       deleteFileCount++;
                   }
               }
               log(string.Format("路径 {0}。成功删除文件 {1} 个", path, deleteFileCount));
           }
           catch (Exception ex)
           {
               throw new Exception(string.Format("处理历史e文件错误！路径：{0} {1}", path, ex.Message.ToString()));
           }
           
       }

      

       /// <summary>
       /// parse efile to db
       /// </summary>
       /// <param name="file">e file</param>
        void dealFile(FileInfo file)
        {
            string fileName = file.Name;
            string path = basePath + "\\" + fileName;
            DateTime CreationTime = GetTimeByFileName(fileName);
            string content = File.ReadAllText(path, Encoding.Default);
            string contentValue = content.Substring(content.IndexOf("<ValueInfo>") + "<ValueInfo>".Length, content.IndexOf("</ValueInfo>") - content.IndexOf("<ValueInfo>") - "<ValueInfo>".Length);
            contentValue = contentValue.Replace("\n", " ").Replace("\t", " ").Replace("\r", "");
            string[] arrValue = contentValue.Split(new[] { " # " }, StringSplitOptions.None);

            List<string> listData = new List<string>();//保存所有数据，sql每次只能插入1000条数据，将所有数据分开插入
            
            if(arrValue.Length > 0)
            {
                for (int i = 0; i < arrValue.Length; i++)
                {
                    if (arrValue[i].IndexOf("@") > -1)//整条记录中有@则舍弃
                    {
                        continue;
                    }
                    string[] arrModel = arrValue[i].Split(' ');//处理行中的每列数据
                    if (arrModel[0].IndexOf("35kV") > -1 || arrModel[0].IndexOf("sysstation") > -1)//场站名中包含35kV和sysstation可以舍弃
                    {
                        continue;
                    }
                    if (!MeasurePointInSqlList.Contains(arrModel[1]))//若测点Id不在数据库中，则不取值。
                    {
                        continue;
                    }
                    if (arrModel.Length > 5)//处理测点名中也存在空格的问题
                    {
                        //将中间合并取得完整的测点名(前后排除两个正确时，中间所有合并为测点名)
                        for (int j = 3; j < arrModel.Length - 2; j++)
                        {
                            arrModel[2] += arrModel[j].ToString().Trim();
                        }
                        arrModel[3] = arrModel[arrModel.Length - 2];
                        arrModel[4] = arrModel[arrModel.Length - 1];
                    }
                    string sqlPart = "(NEWID(),'" + CreationTime + "','" + arrModel[0].ToString() + "','" + arrModel[1].ToString() + "','" + arrModel[2].ToString() + "','" + arrModel[3].ToString() + "','" + arrModel[4].ToString() + "','YC')";
                    listData.Add(sqlPart);
                }
            }

            string contentYxValue = content.Substring(content.IndexOf("<YxValueInfo>") + "<YxValueInfo>".Length, content.IndexOf("</YxValueInfo>") - content.IndexOf("<YxValueInfo>") - "<YxValueInfo>".Length);
            contentYxValue = contentYxValue.Replace("\n", " ").Replace("\t", " ").Replace("\r", "");
            string[] arrYxValue = contentYxValue.Split(new[] { " # " }, StringSplitOptions.None);
            
            if (arrYxValue.Length > 0)
            {
                for (int i = 0; i < arrYxValue.Length; i++)
                {
                    if (arrYxValue[i].IndexOf("@") > -1)
                    {
                        continue;
                    }
                    string[] arrYxModel = arrYxValue[i].Split(' ');
                    if (arrYxModel[0].IndexOf("35kV") > -1 || arrYxModel[0].IndexOf("sysstation") > -1)//场站名中包含35kV和sysstation可以舍弃
                    {
                        continue;
                    }
                    if (!MeasurePointInSqlList.Contains(arrYxModel[1]))//若测点Id不在数据库中，则不取值。
                    {
                        continue;
                    }
                    if (arrYxModel.Length > 5)//处理测点名中也存在空格的问题
                    {
                        //将中间合并取得完整的测点名
                        for (int j = 3; j < arrYxModel.Length - 2; j++)
                        {
                            arrYxModel[2] += arrYxModel[j].ToString().Trim();
                        }
                        arrYxModel[3] = arrYxModel[arrYxModel.Length - 2];
                        arrYxModel[4] = arrYxModel[arrYxModel.Length -1];
                    }
                    string sqlPart = "(NEWID(),'" + CreationTime + "','" + arrYxModel[0].ToString() + "','" + arrYxModel[1].ToString() + "','" + arrYxModel[2].ToString() + "','" + arrYxModel[3].ToString() + "','" + arrYxModel[4].ToString() + "','YX')";
                    listData.Add(sqlPart);
                }
            }
            
            int pageCount = listData.Count / 1000 + 1;

            for (int i = 1; i <= pageCount; i++)
            {
                string sql = @"insert into PRW_SCADA_EFileData values";
                List<string> strlist = listData.Take(i * 1000).Skip((i - 1) * 1000).ToList();
                foreach (var item in strlist)
                {
                    sql += item + ",";
                }
                sql = sql.Substring(0, sql.Length - 1);
                ExcuteSQL(sql, null, CommandType.Text);
            }
        }

        void log(string message)
        {
            Console.WriteLine("{0:HH:mm:ss}  {1}", DateTime.Now, message);
            logger.Info(message);
        }

        public static int ExcuteSQL(string strSQL, SqlParameter[] paras, CommandType cmdType) 
        {
            try
            {
                int i = 0;
                using (SqlConnection conn = new SqlConnection(strConn))
                {
                    SqlCommand cmd = new SqlCommand(strSQL, conn);
                    cmd.CommandType = cmdType;
                    if (paras != null)
                    {
                        cmd.Parameters.AddRange(paras);
                    }
                    conn.Open();
                    i = cmd.ExecuteNonQuery();
                    conn.Close();
                }
                return i; 
            }
            catch (Exception ex)
            {
                throw new Exception("插入数据库错误！" + ex.Message.ToString());
            }
        }

        public static SqlDataReader GetReader(string strSQL, SqlParameter[] paras, CommandType cmdtype)
        {
            try
            {
                SqlDataReader sqldr = null;
                SqlConnection conn = new SqlConnection(strConn);
                SqlCommand cmd = new SqlCommand(strSQL, conn);
                cmd.CommandType = cmdtype;
                if (paras != null)
                {
                    cmd.Parameters.AddRange(paras);
                }
                conn.Open();
                //CommandBehavior.CloseConnection的作用是如果关联的DataReader对象关闭，则连接自动关闭            
                sqldr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                return sqldr;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

       /// <summary>
       /// 根据文件名解析时间
       /// </summary>
       /// <param name="filename"></param>
       /// <returns></returns>
        public DateTime GetTimeByFileName(string filename)
        {
            try 
            {
                string start = "普洱_";
                string end = "_SCADA.DT";
                string timestr = filename.Replace(start, "").Replace(end, "");//移除前后
                string time = timestr.Split('_')[0].Substring(0, 4) + "-" + timestr.Split('_')[0].Substring(4, 2) + "-" + timestr.Split('_')[0].Substring(6, 2) + " " + timestr.Split('_')[1].Substring(0, 2) + ":" + timestr.Split('_')[1].Substring(2, 2) + ":" + timestr.Split('_')[1].Substring(4, 2);
                return Convert.ToDateTime(time);
            }
            catch (Exception ex)
            {
                throw new Exception("时间解析错误！" + ex.Message.ToString());
            }
        }
    }
}
