using Common.Logging;
using MongoDB.Driver;
using PR.WizPlant.Integrate.Scada;
using PR.WizPlant.Integrate.Scada.Entities;
using PR.WizPlant.Integrate.Sql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR.WizPlant.Integrate.StaticServer
{
    public class MonthWorker
    {
         #region Service
        /// <summary>
        /// Scada服务
        /// </summary>
        MongDbScadaService scadaService = null;
        #endregion

        string connectionString = ConfigurationManager.AppSettings["strConn"];
        string tableName = ConfigurationManager.AppSettings["tableName"];
        string month = ConfigurationManager.AppSettings["month"];
        /// <summary>
        /// 删除模式
        /// 0 表示不删除
        /// 1 表示统计后删除
        /// 2 表示不统计只删除
        /// </summary>
        int deleteMode = 0;
        
       

        /// <summary>
        /// 日志间隔
        /// </summary>
        int logCount = 100000;

        ILog logger = LogManager.GetLogger<MonthWorker>();

        Stopwatch sw = new Stopwatch();

        public MonthWorker()
        {
            if (String.IsNullOrWhiteSpace(tableName))
            {
                scadaService = new MongDbScadaService();
            }
            else
            {
                scadaService = new MongDbScadaService(tableName);
            }

            string val = ConfigurationManager.AppSettings["deleteMode"];
            if (string.IsNullOrEmpty(val))
            {
                deleteMode = 0;
            }
            else
            {
                if (int.TryParse(val, out deleteMode))
                {
                    if (deleteMode < 0 && deleteMode > 2)
                    {
                        deleteMode = 0;
                    }
                }
                else
                {
                    deleteMode = 0;
                }
            }
            
        }

        /// <summary>
        /// 日统计数据类
        /// </summary>
        class innerData
        {
            public float MaxValue{get;set;}
            public float MinValue { get; set; }
            public DateTime MaxTime{get;set;}
            public DateTime MinTime{get;set;}
            public double SumValue { get; set; }
            public int Count{get;set;}
        }

        /// <summary>
        /// 跳阐统计数据类
        /// </summary>
        class lastData
        {
            public int Value { get; set; }            
            public DateTime TimeStamp { get; set; }           
            public int Count { get; set; }
        }

        /// <summary>
        /// 日统计缓存
        /// </summary>
        Dictionary<string, innerData> dicCount1 = new Dictionary<string, innerData>();
        /// <summary>
        /// 跳阐统计缓存
        /// </summary>
        Dictionary<string, lastData> dicCount2 = new Dictionary<string, lastData>();
        /// <summary>
        /// 开始时间
        /// </summary>
        DateTime startTime;
        /// <summary>
        /// 结束时间
        /// </summary>
        DateTime endTime;

        int currentMonth = 0;

        /// <summary>
        /// 运行
        /// </summary>
        public void Run()
        {
            string val = ConfigurationManager.AppSettings["logCount"];
            int temp = 0;
            if (!string.IsNullOrWhiteSpace(val) && int.TryParse(val, out temp))
            {
                if (temp > 1000)
                {
                    logCount = temp;
                }
            }


            if (deleteMode == 2)
            {
                if (string.IsNullOrEmpty(month))
                {
                    Console.WriteLine("没有配置起止时间");
                    return;
                }
                var times = month.Split(',');
                if (times.Length != 2)
                {
                    Console.WriteLine("配置起止时间错误");
                    return;
                }

                if (times[0].ToLower() == "min")
                {
                    startTime = DateTime.MinValue;
                }
                else
                {
                    try
                    {
                        startTime = DateTime.Parse(times[0]);
                    }
                    catch
                    {
                        Console.WriteLine("配置起止时间错误");
                        return;
                    }
                }

                if (times[1].ToLower() == "max")
                {
                    endTime = DateTime.MaxValue;
                }
                else
                {
                    try
                    {
                        endTime = DateTime.Parse(times[1]);
                    }
                    catch
                    {
                        Console.WriteLine("配置起止时间错误");
                        return;
                    }
                }

                Console.WriteLine("开始删除数据");
                var task = deleteData(startTime, endTime);
                task.Wait();
                Console.WriteLine("完成删除数据");
                return;
            }




            List<int> monthList = new List<int>();




            // 如果配置不为空
            if (!string.IsNullOrEmpty(month))
            {
                string[] ms = month.Split(',');
                int itemp = 0;
                foreach (var item in ms)
                {
                    if (int.TryParse(item, out itemp))
                    {
                        int y = itemp / 100;
                        int m = itemp % 100;
                        try
                        {
                            new DateTime(y, m, 1);
                            if (!monthList.Contains(itemp))
                            {
                                monthList.Add(itemp);
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.ErrorFormat("日期转换出错:{0}", ex, ex.Message);
                        }
                    }
                }
            }

            if (monthList.Count == 0)
            {
                var lastMonth = DateTime.Now.AddMonths(-1);
                monthList.Add(lastMonth.Year * 100 + lastMonth.Month);
            }

            Console.WriteLine("统计月份:{0}", String.Join(",", monthList.ToArray()));
            logger.DebugFormat("统计月份:{0}", String.Join(",", monthList.ToArray()));


            foreach (var m in monthList)
            {
                currentMonth = m;
                doMonthData(m);
            }
        }
       

        private void doMonthData(int thismonth)
        {
             sw.Restart();
            // 年
            int iyear = 0;
            // 月
            int imonth = 0;

            iyear = thismonth / 100;
            imonth = thismonth % 100;
         

            startTime = new DateTime(iyear, imonth, 1);
            endTime = startTime.AddMonths(1).AddSeconds(-1);

            Console.WriteLine();
            logger.Debug("");

            Console.WriteLine("开始统计{0}月份数据", thismonth);
            logger.DebugFormat("开始统计{0}月份数据", thismonth);

            totalCount = 0;
            yxCount = 0;
            ycCount = 0;
            try
            {
                // 执行异步任务
                var task = calcStatics(startTime, endTime);
                task.Wait();
                // 插入数据
                insertToSqlDB();

                Console.WriteLine("{1}月份统计完成,总耗时{0}分钟", sw.Elapsed.TotalMinutes,thismonth);
                logger.DebugFormat("{1}月份统计完成,总耗时{0}分钟", sw.Elapsed.TotalMinutes, thismonth);
                Console.WriteLine("共处理了{0}条记录,其中YC记录{1}条,YX记录{2}条", totalCount, ycCount, yxCount);
                logger.DebugFormat("共处理了{0}条记录,其中YC记录{1}条,YX记录{2}条", totalCount, ycCount, yxCount);
                Console.WriteLine("YC统计数据{0}条，YX统计数据{1}条", dicCount1.Count, dicCount2.Count);
                logger.DebugFormat("YC统计数据{0}条，YX统计数据{1}条", dicCount1.Count, dicCount2.Count);

                if (deleteMode == 1)
                {
                    var result = deleteData(startTime, endTime);
                    result.Wait();
                }
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("在执行统计时出现错误:{0}", ex);
                Console.WriteLine("在执行统计时出现错误:{0}", ex);
            }
        }


        /// <summary>
        /// 总计数
        /// </summary>
        int totalCount = 0;
        /// <summary>
        /// yx计数
        /// </summary>
        int yxCount = 0;
        /// <summary>
        /// yc计数
        /// </summary>
        int ycCount = 0;

        /// <summary>
        /// 统计任务
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        private async Task calcStatics(DateTime startTime, DateTime endTime)
        {
            Console.WriteLine("开始时间{0},结束时间{1}", startTime, endTime);

            var totalms = endTime.Subtract(startTime).TotalMilliseconds;

            var fb = Builders<ScadaData>.Filter;
            var filter = fb.Gte(p => p.TimeStamp, startTime) & fb.Lte(p => p.TimeStamp, endTime);

            var lastTime = startTime;
          

            // 取得数据表
            var table = scadaService.GetTable();
            // 异步游标处理,防止数据量过大占用资源过多
            using (var cur = await table.FindAsync<ScadaData>(filter))
            {                
                while (await cur.MoveNextAsync())
                {
                    var batch = cur.Current;

                    int batchCount = 0;
                    
                    foreach (ScadaData data in batch)
                    {
                        totalCount++;
                        batchCount++;

                        if (lastTime < data.TimeStamp)
                        {
                            lastTime = data.TimeStamp;
                        }
                       
                        switch(data.Type)
                        {
                            case ScadaDataType.YC:
                                countYC(data);
                                break;
                            case ScadaDataType.YX:
                                countYX(data);
                                break;
                        }

                        // 每n条打印日志
                        if (totalCount % logCount == 0)
                        {
                            Console.WriteLine("======================================================");
                            logger.Debug("======================================================");

                            Console.WriteLine("统计{0}月份数据, 当前数据时间{1}", currentMonth, lastTime.ToString("yyyy-MM-dd HH:mm:ss"));
                            logger.DebugFormat("统计{0}月份数据, 当前数据时间{1}", currentMonth, lastTime.ToString("yyyy-MM-dd HH:mm:ss"));
                            var ts1 = sw.Elapsed;
                            var costms = ts1.TotalMilliseconds;
                            var lastms = lastTime.Subtract(startTime).TotalMilliseconds;
                            if (lastms == 0)
                            {
                                lastms = costms;
                            }

                            var times = 10000;
                            var timesx = totalms * times / lastms;
                            var aboutms = costms * timesx / times;
                            var aboutCount = totalCount * timesx / times;
                            var percent = lastms * 100 / totalms;
                            
                            var ts2 = TimeSpan.FromMilliseconds(aboutms);
                            var aboutTime = DateTime.Now.Add(ts2);
                            var speed = totalCount * 1000 / costms;

                            Console.WriteLine("  总耗时：{0}时{1}分{2}秒, 进度:{3:##.00}%, ALL：{4}, YC:{5}, YC:{6}", ts1.Hours, ts1.Minutes, ts1.Seconds, percent,totalCount,ycCount, yxCount);
                            logger.DebugFormat("  总耗时：{0}时{1}分{2}秒, 进度:{3:##.00}%, ALL：{4}, YC:{5}, YC:{6}", ts1.Hours, ts1.Minutes, ts1.Seconds, percent, totalCount, ycCount, yxCount);
                            Console.WriteLine("  处理速度：{2:0}/秒, 统计数：YC:{0},YX:{1}", dicCount1.Count, dicCount2.Count, speed);
                            logger.DebugFormat("  处理速度：{2:0}/秒, 统计数：YC:{0},YX:{1}", dicCount1.Count, dicCount2.Count, speed);
                            Console.WriteLine("  预计：总记录数{0:0}, 剩余:{4:0}, 需:{1}时{2}分{3}秒", aboutCount, ts2.Hours, ts2.Minutes, ts2.Seconds, aboutCount - totalCount);
                            logger.DebugFormat("  预计：总记录数{0:0}, 剩余:{4:0}, 需:{1}时{2}分{3}秒", aboutCount, ts2.Hours, ts2.Minutes, ts2.Seconds, aboutCount - totalCount);   
                            Console.WriteLine("  预计：完成时间:{0:yyyy-MM-dd HH:mm:ss}",aboutTime);
                            logger.DebugFormat("  预计：完成时间:{0:yyyy-MM-dd HH:mm:ss}", aboutTime); 
                            Console.WriteLine();
                            logger.Debug("");
                        }                        
                    }                   
                }
            }
        }

         /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        private async Task deleteData(DateTime startTime, DateTime endTime)
        {

            Console.WriteLine("开始删除数据：开始时间{0},结束时间{1}", startTime, endTime);
            logger.DebugFormat("开始删除数据：开始时间{0},结束时间{1}", startTime, endTime);

            if (endTime == DateTime.MaxValue)
            {
                Console.WriteLine("不允许删除最新数据");
                return;
            }
            sw.Restart();

            var fb = Builders<ScadaData>.Filter;
            FilterDefinition<ScadaData> filter = null;
            if (startTime > DateTime.MinValue)
            {               
                filter = fb.Gte(p => p.TimeStamp, startTime);
                if (endTime < DateTime.MaxValue)
                {
                    filter &= fb.Lte(p => p.TimeStamp, endTime);
                    
                }
            }
            else if (endTime < DateTime.MaxValue)
            {
                filter = fb.Lte(p => p.TimeStamp, endTime);
            }
            else
            {
                filter = FilterDefinition<ScadaData>.Empty;
            }

            // 取得数据表
            var table = scadaService.GetTable();
            Console.WriteLine("统计要删除的记录数有:{0},耗时:{1}ms", await table.CountAsync(filter), sw.ElapsedMilliseconds);

              // 异步游标处理,防止数据量过大占用资源过多
            var deleteResult = await table.DeleteManyAsync(filter);


            Console.WriteLine("完成删除数据：开始时间{0},结束时间{1},记录数:{2}，耗时:{3}ms", startTime, endTime,deleteResult.DeletedCount,sw.ElapsedMilliseconds);
            logger.DebugFormat("完成删除数据：开始时间{0},结束时间{1},记录数:{2}，耗时:{3}ms", startTime, endTime, deleteResult.DeletedCount, sw.ElapsedMilliseconds);


        }
        /// <summary>
        /// 字典key
        /// </summary>
        string key = null;
        /// <summary>
        /// 字典Key格式，厂站名||测点名||时间
        /// </summary>
        string keyformat = "{0}||{1}||{2}"; 
   
        // 临时对象
        innerData tempInnerData = null;
        lastData tempLastData = null;
        
        /// <summary>
        /// 统计YC
        /// </summary>
        /// <param name="data"></param>
        public void countYC(ScadaData data)
        {
            ycCount++; 
            key = string.Format(keyformat, data.SiteName, data.TagNo, data.TimeStamp.ToString("yyyyMMdd"));
            if (dicCount1.TryGetValue(key, out tempInnerData))
            {
                if (tempInnerData.MaxValue < data.Value)
                {
                    tempInnerData.MaxValue = data.Value;
                    tempInnerData.MaxTime = data.TimeStamp;
                }
                if (tempInnerData.MinValue > data.Value)
                {
                    tempInnerData.MinValue = data.Value;
                    tempInnerData.MinTime = data.TimeStamp;
                }
                tempInnerData.SumValue += data.Value;
                tempInnerData.Count += 1;
            }
            else
            {
                tempInnerData = new innerData { MaxValue = data.Value, MinValue = data.Value, MaxTime = data.TimeStamp, MinTime = data.TimeStamp, SumValue=0, Count = 1 };
                dicCount1[key] = tempInnerData;
            }
        }


        // 默认断开值
        int offValue = 0;

        // 统计YX
        public void countYX(ScadaData data)
        {
            yxCount++;            
            key = string.Format(keyformat, data.SiteName, data.TagNo, data.TimeStamp.ToString("yyyyMM"));
            if (dicCount2.TryGetValue(key, out tempLastData))
            {
                if (tempLastData.TimeStamp < data.TimeStamp)
                {
                    int value = (int)data.Value;
                    if (tempLastData.Value != value)
                    {
                        tempLastData.Value = value;
                        tempLastData.TimeStamp = data.TimeStamp;
                        if (value == offValue)
                        {
                            tempLastData.Count += 1;
                        }
                    }
                }
                else
                {
                    if (tempLastData.TimeStamp.Subtract(data.TimeStamp).TotalSeconds > 1)
                    {
                        if (tempLastData.Value != (int)data.Value)
                        {
                            if (tempLastData.Value == offValue)
                            {
                                tempLastData.Count += 1;
                            }
                        }
                    }
                    //else
                    //{
                    //    Console.WriteLine(string.Format("TagNo:{0}, v1:{1},t1:{2}, v2:{3},t2:{4}", data.TagNo, tempLastData.Value, tempLastData.TimeStamp, data.Value, data.TimeStamp));
                    //    logger.WarnFormat("TagNo:{0}, v1:{1},t1:{2},v2:{3},t2:{4}", data.TagNo, tempLastData.Value, tempLastData.TimeStamp, data.Value, data.TimeStamp);
                    //}
                }
            }
            else
            {
                tempLastData = new lastData { Value = (int)data.Value, TimeStamp = data.TimeStamp, Count = 0 };
                dicCount2[key] = tempLastData;
            }
        }

        /// <summary>
        /// 插入sql 数据库
        /// </summary>
        public void insertToSqlDB()
        {
            Console.WriteLine("开始插入统计数据");
            Console.WriteLine(string.Format("共有日统计数据{0}条，共有跳阐次数{1}次", dicCount1.Count, dicCount2.Count));
            logger.Debug("开始插入统计数据");
            logger.DebugFormat("共有日统计数据{0}条，共有跳阐次数{1}次", dicCount1.Count, dicCount2.Count);

            // 每批处理记录数
            int maxBatchSize = 10000;
            string tableName = "PRW_Inte_SCADA_ValueStatistics";
            string deleteSql = "delete from " + tableName + " where ReportDate>=@pstartTime and ReportDate<=@pendTime";

            Console.WriteLine("开始清理日统计旧数据");
            logger.Debug("开始清理日统计旧数据");
            try
            {
                int result = SqlHelper.ExecuteNonQuery(connectionString, deleteSql, new SqlParameter[] { new SqlParameter("@pstartTime", startTime.ToString("yyyyMMdd")), new SqlParameter("@pendTime", endTime.ToString("yyyyMMdd")) });
                Console.WriteLine("清理日统计旧数据{0}条",result);
                logger.DebugFormat("清理日统计旧数据{0}条", result);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("清理日统计旧数据时出现错误{0}", ex);
                Console.WriteLine(string.Format("清理日统计旧数据时出现错误时出现错误{0}", ex));
            }

            var table = SqlHelper.ExecuteEmptyDataTable(connectionString, tableName);
            table.TableName = tableName;

            int count = 0;

            DataRow dr = null;
            string[] keys;      
            string[] splitString = new String[]{"||"};
            DateTime now = DateTime.Now;

           

            foreach (var item in dicCount1)
            {
                count++;
                keys = item.Key.Split(splitString, StringSplitOptions.RemoveEmptyEntries);
                tempInnerData = item.Value;

                dr = table.NewRow();
                table.Rows.Add(dr);
                dr["Id"] = Guid.NewGuid();
                dr["SiteName"] = keys[0];
                dr["TagNo"] = keys[1];
                dr["ReportDate"] = keys[2];                
                dr["TagDesc"] = "";
                dr["MaxValue"] = tempInnerData.MaxValue;
                dr["MaxTime"] = tempInnerData.MaxTime;
                dr["MinValue"] = tempInnerData.MinValue;
                dr["MinTime"] = tempInnerData.MinTime;
                dr["TotalCount"] = tempInnerData.Count;
                dr["AvgValue"] = (float)(tempInnerData.SumValue / tempInnerData.Count);
                dr["CreateTime"] = now;

                if (count % maxBatchSize == 0)
                {
                    try
                    {
                        logger.DebugFormat("执行批量插入数据库表{1}，数量：{0}", count, tableName);
                        Console.WriteLine("执行批量插入数据库表{1}，数量：{0}", count, tableName);
                        SqlHelper.BulkInsert(connectionString, table);                      
                    }
                    catch (Exception ex)
                    {
                        logger.ErrorFormat("在执行批量插入数据库表{1}时出现错误{0}", ex, tableName);
                        Console.WriteLine(string.Format("在执行批量插入数据库表{1}时出现错误{0}", ex, tableName));
                    }
                    finally
                    {
                        count = 0;
                        table.Clear();
                    }
                }
            }

            if (table.Rows.Count > 0)
            {
                try
                {
                    logger.DebugFormat("执行批量插入数据库表{1}，数量：{0}", table.Rows.Count, tableName);
                    Console.WriteLine(string.Format("执行批量插入数据库表{1}，数量：{0}", table.Rows.Count, tableName));
                    SqlHelper.BulkInsert(connectionString, table);
                    
                }
                catch (Exception ex)
                {
                    logger.ErrorFormat("last在执行批量插入数据库表{1}时出现错误{0}", ex, tableName);
                    Console.WriteLine(string.Format("last在执行批量插入数据库表{1}时出现错误{0}", ex, tableName));
                }
                finally
                {
                    count = 0;
                    table.Clear();
                }
            }

            tableName = "PRW_Inte_SCADA_SkipCount";
            int pMonth = Convert.ToInt32(startTime.ToString("yyyyMM"));

            deleteSql = "delete from " + tableName + " where Month=@pMonth";

            Console.WriteLine("开始清理跳阐统计旧数据");
            logger.Debug("开始清理跳阐统计旧数据");
            try
            {
                int result = SqlHelper.ExecuteNonQuery(connectionString, deleteSql, new SqlParameter[] { new SqlParameter("@pMonth", pMonth)});
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("清理跳阐统计旧数据时出现错误{0}", ex);
                Console.WriteLine(string.Format("清理跳阐统计旧数据时出现错误时出现错误{0}", ex));
            }
            table = SqlHelper.ExecuteEmptyDataTable(connectionString, tableName);
            table.TableName = tableName;

            count = 0;

            foreach (var item in dicCount2)
            {
                count++;
                keys = item.Key.Split(splitString, StringSplitOptions.RemoveEmptyEntries);
                tempLastData = item.Value;

                dr = table.NewRow();
                table.Rows.Add(dr);
                dr["Id"] = Guid.NewGuid();
                dr["SiteName"] = keys[0];
                dr["TagNo"] = keys[1];
                dr["Month"] = keys[2];
                dr["TagDesc"] = "";
                dr["MessureType"] = 0;
                dr["SkipCount"] = tempLastData.Count;               
                dr["CreateTime"] = now;

                if (count % maxBatchSize == 0)
                {
                    try
                    {
                        logger.DebugFormat("执行批量插入数据库表{1}，数量：{0}", count, tableName);
                        Console.WriteLine(string.Format("执行批量插入数据库表{1}，数量：{0}", count, tableName));
                        SqlHelper.BulkInsert(connectionString, table);
                    }
                    catch (Exception ex)
                    {
                        logger.ErrorFormat("在执行批量插入数据库表{1}时出现错误{0}", ex, tableName);
                        Console.WriteLine(string.Format("在执行批量插入数据库表{1}时出现错误{0}", ex, tableName));
                    }
                    finally
                    {
                        count = 0;
                        table.Clear();
                    }
                }
            }

            if (table.Rows.Count > 0)
            {
                try
                {
                    logger.DebugFormat("执行批量插入数据库表{1}，数量：{0}", table.Rows.Count, tableName);
                    Console.WriteLine(string.Format("执行批量插入数据库表{1}，数量：{0}", table.Rows.Count, tableName));
                    SqlHelper.BulkInsert(connectionString, table);
                }
                catch (Exception ex)
                {
                    logger.ErrorFormat("last在执行批量插入数据库表{1}时出现错误{0}", ex, tableName);
                    Console.WriteLine(string.Format("last在执行批量插入数据库表{1}时出现错误{0}", ex, tableName));
                }
                finally
                {
                    count = 0;
                    table.Clear();
                }
            }

            logger.Debug("执行批量更新");
            Console.WriteLine("执行批量更新");
            string updateSql = string.Format("update PRW_Inte_SCADA_ValueStatistics set TagDesc=isnull(b.TagDesc,b.TagName) from PRW_Inte_SCADA_ValueStatistics a inner join PRW_Inte_SCADA_Map b on a.TagNo=b.TagNo where a.ReportDate>={0} and a.ReportDate<={1}", startTime.ToString("yyyyMMdd"), endTime.ToString("yyyyMMdd"));
            Console.WriteLine("开始更新日统计描述");
            logger.Debug("开始更新日统计描述");
            try
            {               
                int result = SqlHelper.ExecuteNonQuery(connectionString, updateSql);
                Console.WriteLine("更新日统计描述{0}条",result);
                logger.DebugFormat("更新日统计描述{0}条", result);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("更新日统计描述时出现错误{0}", ex);
                Console.WriteLine("更新日统计描述时出现错误{0}", ex);
            }

            Console.WriteLine("开始更新跳阐统计描述");
            logger.Debug("开始更新跳阐统计描述");
            updateSql = string.Format("update PRW_Inte_SCADA_SkipCount set TagDesc=isnull(b.TagDesc,b.TagName), MessureType=isnull(b.MessureType,0) from PRW_Inte_SCADA_SkipCount a inner join PRW_Inte_SCADA_Map b on a.TagNo=b.TagNo where a.Month={0} and a.Month<={1}", startTime.ToString("yyyyMM"), endTime.ToString("yyyyMM"));
            try
            {
                int result = SqlHelper.ExecuteNonQuery(connectionString, updateSql);
                Console.WriteLine("更新跳阐统计描述{0}条", result);
                logger.DebugFormat("更新跳阐统计描述{0}条", result);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("更新跳阐统计描述时出现错误{0}", ex);
                Console.WriteLine("更新跳阐统计描述时出现错误{0}", ex);
            } 
            
        }
    }
}
