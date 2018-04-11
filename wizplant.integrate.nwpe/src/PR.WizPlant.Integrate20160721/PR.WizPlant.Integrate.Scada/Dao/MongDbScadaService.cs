using Common.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using PR.WizPlant.Integrate.Scada.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR.WizPlant.Integrate.Scada
{
    public class TimeValue
    {
        public string Date { get; set; }
        public string Time { get; set; }
        public float Value { get; set; }
        public bool B { get; set; }
    }
    public class MongDbScadaService : MongDbScadaServiceBase<ScadaData>
    {
        private static ILog logger = LogManager.GetLogger<MongDbScadaService>();
        //public static DateTime GetStartDate(int date)
        //{
        //    return new DateTime(date / 10000, (date % 10000) / 100, (date % 10000) % 100);
        //}

        //public static DateTime GetEndDate(int date)
        //{
        //    return new DateTime(date / 10000, (date % 10000) / 100, (date % 10000) % 100, 23, 59, 59);
        //}

        protected override string Name
        {
            get { return "ScadaData"; }
        }

        public override void UpdateOne(ScadaData data)
        {           
            table.ReplaceOne(p => p.TagNo == data.TagNo && p.TimeStamp==data.TimeStamp, data, updateOptions);
        }

        public override async Task UpdateOneAsync(ScadaData data)
        {
            await table.ReplaceOneAsync(p => p.TagNo == data.TagNo && p.TimeStamp == data.TimeStamp, data, updateOptions);
        }

        public virtual List<ScadaData> GetList(DateTime endTime, string[] tags)
        {
            var fb = Builders<ScadaData>.Filter;
            var filter = fb.In(p => p.TagNo, tags) & fb.Lte(p => p.TimeStamp, endTime);
            return table.Find(filter).ToListAsync().Result;
        }

        public virtual async Task<List<ScadaData>> GetListAsync(DateTime endTime, string[] tags)
        {
            var fb = Builders<ScadaData>.Filter;
            var filter = fb.In(p => p.TagNo, tags) & fb.Lte(p => p.TimeStamp, endTime);
            return await table.Find(filter).ToListAsync();
        }


        public virtual List<ScadaData> GetList(DateTime startTime, DateTime endTime, string[] tags)
        {
            var fb = Builders<ScadaData>.Filter;
            var filter = fb.In(p => p.TagNo, tags) & fb.Gte(p=>p.TimeStamp,startTime) & fb.Lte(p=>p.TimeStamp,endTime);
            return table.Find(filter).ToListAsync().Result;
        }

        public virtual async Task<List<ScadaData>> GetListAsync(DateTime startTime, DateTime endTime, string[] tags)
        {
            var fb = Builders<ScadaData>.Filter;
            var filter = fb.In(p => p.TagNo, tags) & fb.Gte(p => p.TimeStamp, startTime) & fb.Lte(p => p.TimeStamp, endTime);
           return await table.Find(filter).ToListAsync();
        }

        #region Static 

       // public static readonly string MapTableName = "ScadaMap";
       // public static readonly string ScadaDataTableName = "ScadaData";

       // static object cacheLockObject = new object();      

       // private static Dictionary<string, List<ScadaObjectMap>> object2TagMapCache = null;
       // public static Dictionary<string, List<ScadaObjectMap>> Object2TagMapCache
       // {
       //     get
       //     {
       //         if (object2TagMapCache == null)
       //         {
       //             ResetMapCache();
       //         }
       //         return object2TagMapCache;
       //     }
       // }


       // private static Dictionary<string, List<ScadaObjectMap>> tag2ObjectMapCache = null;
       // public static Dictionary<string, List<ScadaObjectMap>> Tag2ObjectMapCache
       // {
       //     get
       //     {
       //         if (tag2ObjectMapCache == null)
       //         {
       //             ResetMapCache();
       //         }
       //         return tag2ObjectMapCache;
       //     }
       // }

       // static void ResetMapCache()
       // {
       //     lock (cacheLockObject)
       //     {
       //         var table = scadaDB.GetCollection<ScadaObjectMap>(MapTableName);
       //         var list = table.Find(Builders<ScadaObjectMap>.Filter.Where(p => String.IsNullOrEmpty(p.ObjectId) == false)).ToListAsync().Result;               

       //         if (tag2ObjectMapCache == null)
       //         {
       //             tag2ObjectMapCache = new Dictionary<string, List<ScadaObjectMap>>();
       //         }
       //         else
       //         {
       //             tag2ObjectMapCache.Clear();
       //         }

       //         list.GroupBy(p => p.TagNo).ToList().ForEach(item =>
       //         {
       //             tag2ObjectMapCache[item.Key] = item.ToList();
       //         });

       //         if (object2TagMapCache == null)
       //         {
       //             object2TagMapCache = new Dictionary<string, List<ScadaObjectMap>>();
       //         }
       //         else
       //         {
       //             object2TagMapCache.Clear();
       //         }

       //         list.GroupBy(p => p.ObjectId.ToLower()).ToList().ForEach(item =>
       //         {
       //             object2TagMapCache[item.Key] = item.ToList();
       //         });
       //     }
       // }

       // /// <summary>
       // /// 获取设备下测点列表
       // /// </summary>
       // /// <param name="objId">设备对象Id</param>
       // /// <returns>[{"TagNo":"#TagNo"},{"TagNo":"#TagNo"}]</returns>
       // public static string GetObjectTags(Guid objId)
       // {            
       //     string result = null;
       //     StringBuilder sb = new StringBuilder();
       //     string key = objId.ToString().ToLower();
       //     sb.Append("[");
       //     if (Object2TagMapCache.ContainsKey(key))
       //     {
       //         var list = Object2TagMapCache[key];
       //         foreach (var item in list)
       //         {
       //             sb.Append("{");
       //             sb.AppendFormat("\"TagNo\":\"{0}\"", item.TagNo);
       //             sb.Append("},");
       //         }

       //         if (sb.Length > 1)
       //         {
       //             sb.Remove(sb.Length - 1, 1);
       //         }
       //     }

       //     sb.Append("]");

       //     result = sb.ToString();
       //     return result;
       // }

       // /// <summary>
       // /// 获取测点集合的对应的模型值
       // /// </summary>
       // /// <param name="date">日期</param>
       // /// <param name="tags">测点Id数组</param>
       // /// <param name="isShowDetail">是否显示部件</param>
       // /// <returns>[{"ModelName":"","Value",""},{"ModelName":"","Value",""}]</returns>
       // public static string GetTagModelValue(int date, string[] tags, bool isShowDetail)
       // {
       //     if (tags == null || tags.Length == 0)
       //     {
       //         return "[]";
       //     }

       //     var table = scadaDB.GetCollection<ScadaData>(ScadaDataTableName);
       //     var fb = Builders<ScadaData>.Filter;
       //     var fbTags = fb.In(p => p.TagNo, tags);
       //     var fbLteEndTime = fb.Lte(p => p.TimeStamp, GetEndDate(date));
            
       //     var task = table.Find( fbTags & fbLteEndTime ).ToListAsync();

       //     string name = null;
       //     StringBuilder sb = new StringBuilder();
       //     var maps = Tag2ObjectMapCache;
       //    // ScadaData sd = null;
       //     List<ScadaObjectMap> tempList = null;
       //     List<ScadaData> list = new List<ScadaData>();
       //     //task.Result.GroupBy(p => p.TagNo).OrderBy(p=>p.First().MessureType).ToList().ForEach(p =>
       //     task.Result.GroupBy(p => p.TagNo).ToList().ForEach(p =>
       //     {
       //         //sd = p.OrderByDescending(v => v.TimeStamp).First();
       //         list.Add(p.OrderByDescending(v => v.TimeStamp).First());                
       //     });


       //     list = list.OrderBy(p => p.MessureType).ToList();
       //     foreach (var sd in list)
       //     {
       //         name = sd.Name;
       //         if (maps.ContainsKey(sd.TagNo))
       //         {
       //             tempList = maps[sd.TagNo];
       //             foreach (var map in tempList)
       //             {
       //                 name = map.Name;
       //                 if (isShowDetail && !string.IsNullOrEmpty(map.Name2))
       //                 {
       //                     name = map.Name2;
       //                 }
       //                 sb.Append("{");
       //                 sb.AppendFormat("\"ModelName\":\"{0}\",", name);
       //                 sb.AppendFormat("\"Value\":\"{0}\"", sd.Value);
       //                 sb.Append("},");
       //             }
       //         }
       //         else
       //         {
       //             sb.Append("{");
       //             sb.AppendFormat("\"ModelName\":\"{0}\",", name);
       //             sb.AppendFormat("\"Value\":\"{0}\"", sd.Value);
       //             sb.Append("},");
       //         }
       //     }



       //     if (sb.Length > 1)
       //     {
       //         sb.Remove(sb.Length - 1, 1);
       //     }
       //     sb.Insert(0, "[").Append("]");

       //     return sb.ToString();
       // }


       // /// <summary>
       // /// 获取测点集合的模型值
       // /// </summary>
       // /// <param name="date"></param>
       // /// <param name="tags"></param>
       // /// <param name="onlyLast">是否只取最新记录</param>
       // /// <returns>
       // /// [
       // /// {"TagNo":"","SiteName":"","Name","","Type":"YX","MessureType":"101","Count":-1,
       // ///     "TimeValue":[{"Value","","Date":"20160606","Time":"200819","B":1},{"Value","","Date":"20160606","Time":"200819","B":1}]
       // /// },
       // /// {"TagNo":"","SiteName":"","Name","","Type":"YX","MessureType":"101","Count":-1,
       // ///     "TimeValue":[{"Value","","Date":"20160606","Time":"200819","B":1},{"Value","","Date":"20160606","Time":"200819","B":0}]
       // /// }
       // /// ]
       // /// </returns>
       // public static string GetTagDetailValue(int date, string[] tags, bool onlyLast)
       // {
       //     DateTime endTime = GetEndDate(date);
       //     string result = MongDbScadaService.GetTagDetailValue(endTime, oneDaySeconds, defaultInterval, tags, onlyLast);
       //     return result;          
       // }

       // /// <summary>
       // /// 生成Scada数据头部
       // /// </summary>
       // /// <param name="sb"></param>
       // /// <param name="item"></param>
       // private static void AppendScadaHeader(StringBuilder sb, ScadaData item)
       // {
       //     sb.AppendFormat("\"TagNo\":\"{0}\",", item.TagNo);
       //     sb.AppendFormat("\"SiteName\":\"{0}\",", item.SiteName);
       //     sb.AppendFormat("\"Name\":\"{0}\",", item.Name);
       //     sb.AppendFormat("\"Type\":\"{0}\",", item.Type);
       //     sb.AppendFormat("\"MessureType\":\"{0}\",", item.MessureType);

       //     string tagDesc = item.Name;
       //     if (Tag2ObjectMapCache.ContainsKey(item.TagNo))
       //     {
       //         tagDesc = Tag2ObjectMapCache[item.TagNo].First().TagDesc;
       //     }
       //     sb.AppendFormat("\"TagDesc\":\"{0}\",", tagDesc);
       // }

       // /// <summary>
       // /// 生成Scada数据值
       // /// </summary>
       // /// <param name="sb"></param>
       // /// <param name="item"></param>
       // private static void AppendScadaValue(StringBuilder sb, ScadaData item)
       // {
       //      AppendScadaValue(sb, item, true);
       //      return;
       //     sb.Append("{");           
       //     sb.AppendFormat("\"Value\":\"{0}\",", item.Value);          
       //     sb.AppendFormat("\"Date\":\"{0}\",", item.TimeStamp.ToString("yyyyMMdd"));
       //     sb.AppendFormat("\"Time\":\"{0}\",", item.TimeStamp.ToString("HHmmss"));
       //     // 1表示实际测点发出数据
       //     sb.Append("\"B\":1");
       //     sb.Append("}");
       // }

       // /// <summary>
       // /// 生成Scada数据值
       // /// </summary>
       // /// <param name="sb"></param>
       // /// <param name="item"></param>
       // /// <param name="b">是否变化数据</param>
       // private static void AppendScadaValue(StringBuilder sb, ScadaData item, bool b)
       // {
       //     sb.Append("{");
       //     sb.AppendFormat("\"Value\":\"{0}\",", item.Value);
       //     sb.AppendFormat("\"Date\":\"{0}\",", item.TimeStamp.ToString("yyyyMMdd"));
       //     sb.AppendFormat("\"Time\":\"{0}\",", item.TimeStamp.ToString("HHmmss"));
       //     // 1表示实际测点发出数据
       //     sb.AppendFormat("\"B\":{0}", b ? 1 : 0);
       //     sb.Append("}");
       // }

        

       ///// <summary>
       // /// 生成Scada数据值
       ///// </summary>
       ///// <param name="sb"></param>
       ///// <param name="value"></param>
       ///// <param name="date"></param>
       ///// <param name="time"></param>
       ///// <param name="b">是否变化数据</param>
       // private static void AppendScadaValue(StringBuilder sb, float value,string date,string time, bool b)
       // {
       //     sb.Append("{");
       //     sb.AppendFormat("\"Value\":\"{0}\",", value);
       //     sb.AppendFormat("\"Date\":\"{0}\",", date);
       //     sb.AppendFormat("\"Time\":\"{0}\",", time);
       //     // 1表示实际测点发出数据
       //     sb.AppendFormat("\"B\":{0}", b ? 1 : 0);
       //     sb.Append("}");
       // }
        
       // /// <summary>
       // /// 获取测点集合的模型值
       // /// </summary>
       // /// <param name="date"></param>
       // /// <param name="tags"></param>
       // /// <param name="onlyLast">是否只取最新记录</param>
       // /// <returns>
       // /// [
       // /// {"TagNo":"","SiteName":"","Name","","Type":"YX","MessureType":"101","Count":-1,
       // ///     "TimeValue":[{"Value","","Date":"20160606","Time":"200819","B":1},{"Value","","Date":"20160606","Time":"200819","B":1}]
       // /// },
       // /// {"TagNo":"","SiteName":"","Name","","Type":"YX","MessureType":"101","Count":-1,
       // ///     "TimeValue":[{"Value","","Date":"20160606","Time":"200819","B":1},{"Value","","Date":"20160606","Time":"200819","B":0}]
       // /// }
       // /// ]
       // /// </returns>
       // public static string GetTagDetailValue(DateTime endTime, int totalSeconds, int intervalSeconds, string[] tags, bool onlyLast)
       // {
       //     if (tags == null || tags.Length == 0)
       //     {
       //         return "[]";
       //     }
       //     if (totalSeconds < 0 || intervalSeconds < 0)
       //     {
       //         return "[]";
       //     }

       //     var table = scadaDB.GetCollection<ScadaData>(ScadaDataTableName);
       //     var beginTime = endTime.AddSeconds(-totalSeconds);

       //     var fb = Builders<ScadaData>.Filter;
       //     var fbTags = fb.In(p => p.TagNo, tags);            
       //     var fbLteLastTime = fb.Lte(p => p.TimeStamp, endTime);

       //     StringBuilder sb = new StringBuilder();

       //     sb.Append("[");
            

       //     // 分组列表
       //     Dictionary<string, List<ScadaData>> dic = null;
       //     DateTime startTime = DateTime.Now;

       //     Dictionary<string, TimeValue> temp = new Dictionary<string, TimeValue>();
       //     if (onlyLast)
       //     {
                
       //         var task = table.Find(fb.And(new FilterDefinition<ScadaData>[] { fbTags, fbLteLastTime })).ToListAsync();
       //         var list = new List<ScadaData>();
       //         var result = task.Result;
       //         logger.DebugFormat("GetTagDetailValue query Cost:{5}ms,paras:({0:yyyyMMddHHmmss},{1},{2},{3},{4})", endTime, totalSeconds, intervalSeconds, string.Join(",", tags), onlyLast,DateTime.Now.Subtract(startTime).TotalMilliseconds);
       //         result.GroupBy(p => p.TagNo).ToList().ForEach(p =>
       //             {
       //                 list.Add(p.OrderByDescending(v => v.TimeStamp).First());
       //             });

       //         logger.DebugFormat("GetTagDetailValue query and group Cost:{5}ms,paras:({0:yyyyMMddHHmmss},{1},{2},{3},{4})", endTime, totalSeconds, intervalSeconds, string.Join(",", tags), onlyLast, DateTime.Now.Subtract(startTime).TotalMilliseconds);

       //         if (list.Count > 0)
       //         {
       //             list.OrderBy(p => p.MessureType).ToList().ForEach(item =>
       //                 {
       //                     sb.Append("{");
       //                     AppendScadaHeader(sb, item);
       //                     sb.Append("\"TimeValue\":[");
       //                     AppendScadaValue(sb, item);
       //                     sb.Append("]},");
       //                 }
       //             );
       //             sb.Remove(sb.Length - 1, 1);
       //         }
       //     }
       //     else
       //     {
       //         dic = new Dictionary<string, List<ScadaData>>();
       //         var fbLteBeginTime = fb.Lte(p => p.TimeStamp, beginTime);

       //         var task = table.Find(fb.And(new FilterDefinition<ScadaData>[] { fbTags, fbLteBeginTime })).ToListAsync();
       //         var result = task.Result;
       //         logger.DebugFormat("GetTagDetailValue query Cost:{5}ms,paras:({0:yyyyMMddHHmmss},{1},{2},{3},{4})", endTime, totalSeconds, intervalSeconds, string.Join(",", tags), onlyLast, DateTime.Now.Subtract(startTime).TotalMilliseconds);
       //         result.GroupBy(p => p.TagNo).ToList().ForEach(p =>
       //         {
       //             dic[p.Key] = new List<ScadaData>();
       //             dic[p.Key].Add(p.OrderByDescending(v => v.TimeStamp).First());
       //         });
       //         logger.DebugFormat("GetTagDetailValue query and group Cost:{5}ms,paras:({0:yyyyMMddHHmmss},{1},{2},{3},{4})", endTime, totalSeconds, intervalSeconds, string.Join(",", tags), onlyLast, DateTime.Now.Subtract(startTime).TotalMilliseconds);


       //         logger.DebugFormat("justBeforeOne cost:{0}ms", DateTime.Now.Subtract(startTime).TotalMilliseconds);

       //         var fbGtBeginTime = fb.Gt(p => p.TimeStamp, beginTime);

       //         startTime = DateTime.Now;
       //         task = table.Find(fbTags & fbGtBeginTime & fbLteLastTime).ToListAsync();
       //         task.Result.GroupBy(p => p.TagNo).OrderBy(p => p.First().MessureType).ToList().ForEach(p =>
       //         {
       //             if (!dic.ContainsKey(p.Key))
       //             {
       //                 dic[p.Key] = new List<ScadaData>();
       //                 dic[p.Key].AddRange(p.OrderByDescending(v => v.TimeStamp));
       //             }
       //             else
       //             {
       //                 // 从低位到高位时间逆序
       //                 dic[p.Key].InsertRange(0, p.OrderByDescending(v => v.TimeStamp));
       //             }
       //         });


       //         logger.DebugFormat("interval cost:{0}ms", DateTime.Now.Subtract(startTime).TotalMilliseconds);

       //         ScadaData sd = null;

       //         DateTime maxTime = DateTime.Now;//.AddSeconds(-intervalSeconds);
       //         if (maxTime > endTime)
       //         {
       //             maxTime = endTime;
                    
       //         }
       //         DateTime stepTime = beginTime;
       //         List<string> seqTimeList = new List<string>();
       //         while (stepTime < maxTime)
       //         {
       //             seqTimeList.Add(stepTime.ToString("HHmmss"));                    
       //             stepTime = stepTime.AddSeconds(intervalSeconds);
       //         }

       //         string thisDate = maxTime.ToString("yyyyMMdd");              

       //         string date = null;
       //         string time = null;

       //         foreach (var list in dic.Values)
       //         {
       //             sd = list.First();
       //             sb.Append("{");
       //             AppendScadaHeader(sb, sd);
       //             // 增加计数占位符,如果没有替换，-1表示不进行计数
       //             sb.Append("\"Count\":-1,");
       //             sb.Append("\"TimeValue\":[");

       //             int i = list.Count;

       //             // 返回记录数
       //             int count = 0;

       //             if (i > 0)
       //             {
       //                 temp.Clear();

       //                 foreach (var key in seqTimeList)
       //                 {
       //                     temp.Add(key, null);
       //                 }

       //                 foreach (var item in list)
       //                 {
       //                     date = item.TimeStamp.ToString("yyyyMMdd");
       //                     if (date.CompareTo(thisDate) <= 0)
       //                     {
       //                         time = item.TimeStamp.ToString("HHmmss");
       //                         temp[time] = new TimeValue { Value = item.Value, Date = date, Time = time, B = true };
       //                     }
       //                 }

       //                 TimeValue pre = null;

       //                 temp.OrderBy(p => (p.Value == null ? thisDate : p.Value.Date) + p.Key).ToList().ForEach(p =>
       //                     {
       //                         if (p.Value == null)
       //                         {
       //                             if (pre != null)
       //                             {
       //                                 if (p.Key.CompareTo(beginTime.ToString("HHmmss")) >= 0)
       //                                 {
       //                                     count++;
       //                                     AppendScadaValue(sb, pre.Value, thisDate, p.Key, false);
       //                                     sb.Append(",");
       //                                 }
       //                             }
       //                         }
       //                         else
       //                         {
       //                             pre = p.Value;
       //                             if (pre.Date == thisDate)
       //                             {
       //                                 count++;
       //                                 AppendScadaValue(sb, pre.Value, pre.Date, pre.Time, pre.B);
       //                                 sb.Append(",");
       //                             }                                    
       //                         }
       //                     });
                       
       //                 sb.Remove(sb.Length - 1, 1);
       //             }

       //             // 替换计数
       //             sb.Replace("\"Count\":-1",string.Format("\"Count\":{0}",count));

       //             sb.Append("]");
       //             sb.Append("},");
       //         }
       //         if (sb.Length > 1)
       //         {
       //             sb.Remove(sb.Length - 1, 1);
       //         }                
       //     }
       //     sb.Append("]");

       //     return sb.ToString();
       // }

       // /// <summary>
       // /// 1天的秒数，用来做默认的显示时间长度
       // /// </summary>
       // static int oneDaySeconds = 60 * 60 * 24 - 1;

       // /// <summary>
       // /// 默认间隔秒数
       // /// </summary>
       // static int defaultInterval = 60;

       // /// <summary>
       // /// 获取测点集合的最新值
       // /// </summary>
       // /// <param name="date"></param>
       // /// <param name="tags"></param>
       // /// <returns>[{"TagNo":"","Value",""},{"TagNo":"","Value",""}]</returns>
       // public static string GetTagListValue(int date, string[] tags)
       // {
       //     if (tags == null || tags.Length == 0)
       //     {
       //         return "[]";
       //     }


       //     return GetTagListLastValue(GetEndDate(date), oneDaySeconds, defaultInterval, tags);
       // }

       // /// <summary>
       // /// 获取测点集合的最新值
       // /// </summary>
       // /// <param name="endTime"></param>
       // /// <param name="totalSeconds"></param>
       // /// <param name="intervalSeconds"></param>
       // /// <param name="tags"></param>
       // /// <returns>[{"TagNo":"","Value",""},{"TagNo":"","Value",""}]</returns>
       // public static string GetTagListLastValue(DateTime endTime, int totalSeconds, int intervalSeconds, string[] tags)
       // {
       //     if (tags == null || tags.Length == 0)
       //     {
       //         return "[]";
       //     }
       //     if (totalSeconds < 0 || intervalSeconds < 0)
       //     {
       //         return "[]";
       //     }

       //     DateTime startTime = DateTime.Now;

       //     var table = scadaDB.GetCollection<ScadaData>(ScadaDataTableName);

       //     var fb = Builders<ScadaData>.Filter;
       //     var fbTags = fb.In(p => p.TagNo, tags);
       //     var fbLteEndTime = fb.Lte(p => p.TimeStamp, endTime);
       //     var task = table.Find(fb.And(new FilterDefinition<ScadaData>[] { fbTags, fbLteEndTime })).ToListAsync();
       //     List<ScadaData> list = new List<ScadaData>();

       //     int count = 0;

       //     Dictionary<string, ScadaData> dic = new Dictionary<string, ScadaData>();
       //     table.Find(fb.And(new FilterDefinition<ScadaData>[] { fbTags, fbLteEndTime })).ForEachAsync(p =>
       //     {
       //         count++;
       //         if (dic.ContainsKey(p.TagNo))
       //         {
       //             if (p.TimeStamp > dic[p.TagNo].TimeStamp)
       //             {
       //                 dic[p.TagNo] = p;
       //             }
       //         }
       //         else
       //         {
       //             dic[p.TagNo] = p;
       //         }
       //     }).Wait();

       //     //List<ScadaData> list = null;
       //     //list = new List<ScadaData>();

       //     StringBuilder sb = new StringBuilder();
       //     sb.Append("[");

       //   //  ScadaData sd = null;
            
       //     //task.Result.GroupBy(p => p.TagNo).OrderBy(p => p.First().MessureType).ToList().ForEach(p =>
       //    //var result = task.Result;
       //    logger.DebugFormat("GetTagListLastValue query Cost:{4}ms,paras:({0:yyyyMMddHHmmss},{1},{2},{3})", endTime, totalSeconds, intervalSeconds, string.Join(",", tags), DateTime.Now.Subtract(startTime).TotalMilliseconds);
       //    logger.DebugFormat("总记录数为：{0}", count);
       //     //    result.GroupBy(p => p.TagNo).ToList().ForEach(p =>
       //     //{
       //     //    list.Add(p.OrderByDescending(v => v.TimeStamp).First());
       //     //    //sd = p.OrderByDescending(v => v.TimeStamp).First();
       //     //    //sb.Append("{");

       //     //    //sb.AppendFormat("\"TagNo\":\"{0}\",", sd.TagNo);
       //     //    //sb.AppendFormat("\"Value\":\"{0}\"", sd.Value);
       //     //    //sb.Append("},");
       //     //});

       //    list = dic.Values.OrderBy(p => p.MessureType).ToList();
       //     logger.DebugFormat("GetTagListLastValue query and group Cost:{4}ms,paras:({0:yyyyMMddHHmmss},{1},{2},{3})", endTime, totalSeconds, intervalSeconds, string.Join(",", tags), DateTime.Now.Subtract(startTime).TotalMilliseconds);
       //     foreach (var item in list)
       //     {
       //         sb.Append("{");

       //         sb.AppendFormat("\"TagNo\":\"{0}\",", item.TagNo);
       //         sb.AppendFormat("\"Value\":\"{0}\"", item.Value);
       //         sb.Append("},");
       //     }

       //     if (sb.Length > 1)
       //     {
       //         sb.Remove(sb.Length - 1, 1);
       //     }
       //     sb.Append("]");

       //     return sb.ToString();
       // }
        #endregion
    }
}
