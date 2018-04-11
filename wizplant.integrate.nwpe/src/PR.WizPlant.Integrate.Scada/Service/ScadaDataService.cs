using Common.Logging;
using PR.WizPlant.Integrate.Scada.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace PR.WizPlant.Integrate.Scada.Service
{
    /// <summary>
    /// Scada服务逻辑类
    /// </summary>
    public class ScadaDataService
    {
        static ScadaDataService()
        {
            try
            {
                currentScadaDao = new MongDbCurrentScadaService();
                scadaDao = new MongDbScadaService();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("static ScadaDataService() Error:{0}", ex.Message, ex);
            }
        }

        /// <summary>
        /// 获取开始时间
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime GetStartDate(int date)
        {
            return new DateTime(date / 10000, (date % 10000) / 100, (date % 10000) % 100);
        }

        /// <summary>
        /// 获取结束时间
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime GetEndDate(int date)
        {
            return new DateTime(date / 10000, (date % 10000) / 100, (date % 10000) % 100, 23, 59, 59);
        }

        /// <summary>
        /// 日志类
        /// </summary>
        static ILog logger = LogManager.GetLogger("PR.WizPlant.Integrate.Scada.Service.ScadaDataService");

        /// <summary>
        /// 最新实时数据DAO
        /// </summary>
        static MongDbCurrentScadaService currentScadaDao = null;
        /// <summary>
        /// 全部实时数据DAO
        /// </summary>
        static MongDbScadaService scadaDao = null;

        ///// <summary>
        ///// 测点对象映射缓存
        ///// </summary>
        //static Dictionary<string, List<ScadaObjectMap>> Tag2ObjectMapCache = MongDbScadaMapService.Tag2ObjectMapCache;
        ///// <summary>
        ///// 对象测点映射缓存
        ///// </summary>
        //static Dictionary<string, List<ScadaObjectMap>> Object2TagMapCache = MongDbScadaMapService.Object2TagMapCache;
        #region Static

        /// <summary>
        /// 1天的秒数，用来做默认的显示时间长度
        /// </summary>
        static int oneDaySeconds = 60 * 60 * 24 - 1;

        /// <summary>
        /// 默认间隔秒数
        /// </summary>
        static int defaultInterval = 60;


        /// <summary>
        /// 生成Scada数据头部
        /// </summary>
        /// <param name="sb">字符串生成器</param>
        /// <param name="item">实时数据</param>
        private static void AppendScadaHeader(StringBuilder sb, ScadaData item)
        {
            sb.AppendFormat("\"TagNo\":\"{0}\",", item.TagNo);
            sb.AppendFormat("\"SiteName\":\"{0}\",", item.SiteName);
            sb.AppendFormat("\"Name\":\"{0}\",", item.Name);
            sb.AppendFormat("\"Type\":\"{0}\",", item.Type);
            sb.AppendFormat("\"MessureType\":\"{0}\",", item.MessureType);

            string tagDesc = item.Name;
            if (MongDbScadaMapService.Tag2ObjectMapCache.ContainsKey(item.TagNo))
            {
                tagDesc = MongDbScadaMapService.Tag2ObjectMapCache[item.TagNo].First().TagDesc;
            }
            sb.AppendFormat("\"TagDesc\":\"{0}\",", tagDesc);
        }

        /// <summary>
        /// 生成Scada数据值
        /// </summary>
        /// <param name="sb">字符串生成器</param>
        /// <param name="item">实时数据</param>
        private static void AppendScadaValue(StringBuilder sb, ScadaData item)
        {
            AppendScadaValue(sb, item, true);
            //return;
            //sb.Append("{");
            //sb.AppendFormat("\"Value\":\"{0}\",", item.Value);
            //sb.AppendFormat("\"Date\":\"{0}\",", item.TimeStamp.ToString("yyyyMMdd"));
            //sb.AppendFormat("\"Time\":\"{0}\",", item.TimeStamp.ToString("HHmmss"));
            //// 1表示实际测点发出数据
            //sb.Append("\"B\":1");
            //sb.Append("}");
        }

        /// <summary>
        /// 生成Scada数据值
        /// </summary>
        /// <param name="sb">字符串生成器</param>
        /// <param name="item">实时数据</param>
        /// <param name="b">是否变化数据</param>
        private static void AppendScadaValue(StringBuilder sb, ScadaData item, bool b)
        {
            sb.Append("{");
            sb.AppendFormat("\"Value\":\"{0}\",", item.Value);
            var localTime = item.TimeStamp.ToLocalTime();
            sb.AppendFormat("\"Date\":\"{0}\",", localTime.ToString("yyyyMMdd"));
            sb.AppendFormat("\"Time\":\"{0}\",", localTime.ToString("HHmmss"));
            // 1表示实际测点发出数据
            sb.AppendFormat("\"B\":{0}", b ? 1 : 0);
            sb.Append("}");
        }



        /// <summary>
        /// 生成Scada数据值
        /// </summary>
        /// <param name="sb">字符串生成器</param>
        /// <param name="value">值</param>
        /// <param name="date">日期</param>
        /// <param name="time">时间</param>
        /// <param name="b">是否变化数据</param>
        private static void AppendScadaValue(StringBuilder sb, float value, string date, string time, bool b)
        {
            sb.Append("{");
            sb.AppendFormat("\"Value\":\"{0}\",", value);
            sb.AppendFormat("\"Date\":\"{0}\",", date);
            sb.AppendFormat("\"Time\":\"{0}\",", time);
            // 1表示实际测点发出数据
            sb.AppendFormat("\"B\":{0}", b ? 1 : 0);
            sb.Append("}");
        }


        #region Scada数据查询说明
        /***
         * 为了提高查询效率，Scada实时数据分两处存放:
         * 1 ScadaData，此处存放的是全部的实时数据，在查询过往数据时必须要从此集合中查询，因为目前是单机数据，随着数据的增长，查询效率极差
         * 2 CurrentScadaData，此处存放的测点最新的数据，每个测点最多只会存放一条数据，并且值为最后变化的值
         * 
         * 应用：
         * 1.如果是查询当天最新数据，则直接CurrentScadaData给出
         * 2.如果查询是非当天数据，则由CurrentScadaData和ScadaData组合给出
         *   从CurrentScadaData查询出结束时间以前的所有值，这样每条数据都会有一条数据。
         *   从ScadaData查询开始时间到结束时间之间的所有数据，这部分数据最全，但有可能会漏掉当天没有变化的测数据
         *   将CurrentScadaData结果集中存在而ScadaData结果集中不存在的数据加入到ScadaData，这样保证了查询范围最小数据完整的结果
         * 3.如果查询非当天最新数据，则将2中的结果以测点分组每个分组只取时间最新的那条数据
         * 4.结果后结果以变化时间倒序和测量类型顺序排序
         * 
         * **/
        #endregion


        /// <summary>
        /// 获取设备下测点列表
        /// </summary>
        /// <param name="objId">设备对象Id</param>
        /// <returns>[{"TagNo":"#TagNo"},{"TagNo":"#TagNo"}]</returns>
        public static string GetObjectTags(Guid objId)
        {
            logger.DebugFormat("Enter GetObjectTags({0})", objId);
            string result = null;
            StringBuilder sb = new StringBuilder();
            string key = objId.ToString().ToLower();
            sb.Append("[");
            if (MongDbScadaMapService.Object2TagMapCache.ContainsKey(key))
            {
                var list = MongDbScadaMapService.Object2TagMapCache[key];
                foreach (var item in list)
                {
                    sb.Append("{");
                    sb.AppendFormat("\"TagNo\":\"{0}\"", item.TagNo);
                    sb.Append("},");
                }

                if (sb.Length > 1)
                {
                    sb.Remove(sb.Length - 1, 1);
                }
            }

            sb.Append("]");

            result = sb.ToString();
            logger.DebugFormat("result = {0}", result);
            logger.DebugFormat("Exit GetObjectTags({0})", objId);
            return result;
        }

        /// <summary>
        /// 根据隔离开关设备id列表获取其测点列表
        /// </summary>
        /// <param name="objId">设备对象Id</param>
        /// <returns>[{"TagNo":"#TagNo"},{"TagNo":"#TagNo"}]</returns>
        public static string GetSwitchObjectTagsList(Guid[] objIds)
        {
            logger.DebugFormat("Enter GetObjectTags({0})", objIds);
            string result = null;
            StringBuilder sb = new StringBuilder();

            string key = string.Empty;

            var tagList= MongDbScadaMapService.Object2TagMapCache.Where(item => objIds.Contains(new Guid(item.Key))).
                SelectMany(list=>list.Value).
                Where(tag=>tag.MessureType==801).OrderBy(tag=>tag.ObjectId);

            sb.Append("[");
            foreach (var tag in tagList)
            {
                sb.Append("{");
                sb.AppendFormat("\"OriginObjId\":\"{0}\",", tag.ObjectId);
                sb.AppendFormat("\"TagNo\":\"{0}\"", tag.TagNo);
                sb.Append("},");
            }

            if (sb.Length > 1)
            {
                sb.Remove(sb.Length - 1, 1);
            }

            sb.Append("]");

            result = sb.ToString();
            logger.DebugFormat("result = {0}", result);
            logger.DebugFormat("Exit GetObjectTags({0})", objIds);
            return result;
        }

        /// <summary>
        /// 根据隔离开关设备id列表获取其测点列表
        /// </summary>
        /// <param name="objId">设备对象Id</param>
        /// <returns>[{"TagNo":"#TagNo"},{"TagNo":"#TagNo"}]</returns>
        public static string GetSwitchObjectTagsList(Dictionary<string,string> switchObjDict)
        {
            logger.DebugFormat("Enter GetObjectTags({0})", switchObjDict.Keys.ToArray());
            string result = null;
            StringBuilder sb = new StringBuilder();

            Guid[] objIds = switchObjDict.Keys.Select(item=>new Guid(item)).ToArray();
            
            string key = string.Empty;

            var tagList = MongDbScadaMapService.Object2TagMapCache.Where(item => objIds.Contains(new Guid(item.Key))).
                SelectMany(list => list.Value).
                Where(tag => tag.MessureType == 801).OrderBy(tag => tag.ObjectId);

            sb.Append("[");
            foreach (var tag in tagList)
            {
                sb.Append("{");
                sb.AppendFormat("\"OriginObjId\":\"{0}\",", tag.ObjectId);
                sb.AppendFormat("\"AliasObjCode\":\"{0}\",", switchObjDict[tag.ObjectId]);
                sb.AppendFormat("\"TagNo\":\"{0}\"", tag.TagNo);
                sb.Append("},");
            }

            if (sb.Length > 1)
            {
                sb.Remove(sb.Length - 1, 1);
            }

            sb.Append("]");

            result = sb.ToString();
            logger.DebugFormat("result = {0}", result);
            logger.DebugFormat("Exit GetObjectTags({0})", objIds);
            return result;
        }

      
        /// <summary>
        /// 获取测点集合的对应的模型值
        /// </summary>
        /// <param name="date">日期</param>
        /// <param name="tags">测点Id数组</param>
        /// <param name="isShowDetail">是否显示部件</param>
        /// <returns>[{"ModelName":"","Value",""},{"ModelName":"","Value",""}]</returns>
        /// <remarks></remarks>
        public static string GetTagModelValue(int date, string[] tags, bool isShowDetail)
        {
            if (tags == null || tags.Length == 0)
            {
                return "[]";
            }

            DateTime startTime = GetStartDate(date);
            DateTime endTime = GetEndDate(date);

            List<ScadaData> list = null;
            StringBuilder sb = new StringBuilder();

            // 获取最新数据
            var task1 = currentScadaDao.GetListAsync(endTime, tags);

            // 是否查询当天数据
            if (endTime.Date == DateTime.Now.Date)
            {
                list = task1.Result;
            }
            else
            {
                // 查询指定时间段的数据
                var task2 = scadaDao.GetListAsync(startTime, endTime, tags);
                list = new List<ScadaData>();
                // 以测点分组取最新的那条记录
                task2.Result.GroupBy(p => p.TagNo).ToList().ForEach(p =>
                {
                    list.Add(p.OrderByDescending(v => v.TimeStamp).First());
                }
                    );

                // 将此时间段没有变化的测点的最新数据加入
                task1.Result.ForEach(p =>
                {
                    if (!list.Exists(v => v.TagNo == p.TagNo))
                    {
                        list.Add(p);
                    }
                }
                );
            }

            

            string name = null;
            var maps = MongDbScadaMapService.Tag2ObjectMapCache;
            List<ScadaObjectMap> tempList = null;
            // 以测量类型排序
            list = list.OrderBy(p => p.MessureType).ToList();
            foreach (var sd in list)
            {
                name = sd.Name;
                if (maps.ContainsKey(sd.TagNo))
                {
                    tempList = maps[sd.TagNo];
                    foreach (var map in tempList)
                    {
                        name = map.Name;
                        if (isShowDetail && !string.IsNullOrEmpty(map.Name2))
                        {
                            name = map.Name2;
                        }
                        sb.Append("{");
                        sb.AppendFormat("\"ModelName\":\"{0}\",", name);
                        sb.AppendFormat("\"Value\":\"{0}\"", sd.Value);
                        sb.Append("},");
                    }
                }
                else
                {
                    sb.Append("{");
                    sb.AppendFormat("\"ModelName\":\"{0}\",", name);
                    sb.AppendFormat("\"Value\":\"{0}\"", sd.Value);
                    sb.Append("},");
                }
            }



            if (sb.Length > 1)
            {
                sb.Remove(sb.Length - 1, 1);
            }
            sb.Insert(0, "[").Append("]");

            return sb.ToString();
        }
        
      

        /// <summary>
        /// 获取测点集合的模型值
        /// </summary>
        /// <param name="date"></param>
        /// <param name="tags"></param>
        /// <param name="onlyLast">是否只取最新记录</param>
        /// <returns>
        /// [
        /// {"TagNo":"","SiteName":"","Name","","Type":"YX","MessureType":"101","Count":-1,
        ///     "TimeValue":[{"Value","","Date":"20160606","Time":"200819","B":1},{"Value","","Date":"20160606","Time":"200819","B":1}]
        /// },
        /// {"TagNo":"","SiteName":"","Name","","Type":"YX","MessureType":"101","Count":-1,
        ///     "TimeValue":[{"Value","","Date":"20160606","Time":"200819","B":1},{"Value","","Date":"20160606","Time":"200819","B":0}]
        /// }
        /// ]
        /// </returns>
        public static string GetTagDetailValue(int date, string[] tags, bool onlyLast)
        {
            DateTime endTime = GetEndDate(date);
            string result = GetTagDetailValue(endTime, oneDaySeconds, defaultInterval, tags, onlyLast);
            return result;
        }

        /// <summary>
        /// 获取测点集合的模型值
        /// </summary>
        /// <param name="date"></param>
        /// <param name="tags"></param>
        /// <param name="onlyLast">是否只取最新记录</param>
        /// <returns>
        /// [
        /// {"TagNo":"","SiteName":"","Name","","Type":"YX","MessureType":"101","Count":-1,
        ///     "TimeValue":[{"Value","","Date":"20160606","Time":"200819","B":1},{"Value","","Date":"20160606","Time":"200819","B":1}]
        /// },
        /// {"TagNo":"","SiteName":"","Name","","Type":"YX","MessureType":"101","Count":-1,
        ///     "TimeValue":[{"Value","","Date":"20160606","Time":"200819","B":1},{"Value","","Date":"20160606","Time":"200819","B":0}]
        /// }
        /// ]
        /// </returns>
        public static string GetTagDetailValue(DateTime endTime, int totalSeconds, int intervalSeconds, string[] tags, bool onlyLast)
        {
            if (tags == null || tags.Length == 0)
            {
                return "[]";
            }
            if (totalSeconds < 0 || intervalSeconds < 0)
            {
                return "[]";
            }
           
            var beginTime = endTime.AddSeconds(-totalSeconds);

           

            StringBuilder sb = new StringBuilder();

            sb.Append("[");


            // 分组列表
            Dictionary<string, List<ScadaData>> dic = null;
            DateTime startTime = DateTime.Now;
            Dictionary<string, TimeValue> temp = new Dictionary<string, TimeValue>();

           

            var task1 = currentScadaDao.GetListAsync(endTime, tags);
            if (onlyLast)
            {
                var list = new List<ScadaData>();

                if (DateTime.Now.Date != endTime.Date)
                {
                    var task2 = scadaDao.GetListAsync(beginTime, endTime, tags);
                    task2.Result.GroupBy(p => p.TagNo).ToList().ForEach(
                        p =>
                        {
                            list.Add(p.OrderByDescending(v => v.TimeStamp).First());
                        }
                        );
                    task1.Result.ForEach(
                         p =>
                         {
                             if (!list.Exists(v => v.TagNo == p.TagNo))
                             {
                                 list.Add(p);
                             }
                         }
                        );
                }
                else
                {
                    list.AddRange(task1.Result);
                }

              

                logger.DebugFormat("GetTagDetailValue query and group Cost:{5}ms,paras:({0:yyyyMMddHHmmss},{1},{2},{3},{4})", endTime, totalSeconds, intervalSeconds, string.Join(",", tags), onlyLast, DateTime.Now.Subtract(startTime).TotalMilliseconds);

                if (list.Count > 0)
                {
                    list.OrderBy(p => p.MessureType).ToList().ForEach(item =>
                    {
                        sb.Append("{");
                        AppendScadaHeader(sb, item);
                        sb.Append("\"Count\":1,");
                        sb.Append("\"TimeValue\":[");
                        AppendScadaValue(sb, item);
                        sb.Append("]},");
                    }
                    );
                    sb.Remove(sb.Length - 1, 1);
                }
            }
            else
            {
                dic = new Dictionary<string, List<ScadaData>>();

                var task2 = scadaDao.GetListAsync(beginTime, endTime, tags);
                var result = task2.Result;
                logger.DebugFormat("GetTagDetailValue query Cost:{5}ms,paras:({0:yyyyMMddHHmmss},{1},{2},{3},{4})", endTime, totalSeconds, intervalSeconds, string.Join(",", tags), onlyLast, DateTime.Now.Subtract(startTime).TotalMilliseconds);
                result.GroupBy(p => p.TagNo).ToList().ForEach(p =>
                {                   
                        dic[p.Key] = new List<ScadaData>();
                        dic[p.Key].AddRange(p.OrderByDescending(v => v.TimeStamp).ToArray());
                });
                task1.Result.ForEach(
                    p =>
                    {
                        if (!dic.ContainsKey(p.TagNo))
                        {
                            dic[p.TagNo] = new List<ScadaData>();
                            dic[p.TagNo].Add(p);
                        }
                    }
                    );
                logger.DebugFormat("GetTagDetailValue query and group Cost:{5}ms,paras:({0:yyyyMMddHHmmss},{1},{2},{3},{4})", endTime, totalSeconds, intervalSeconds, string.Join(",", tags), onlyLast, DateTime.Now.Subtract(startTime).TotalMilliseconds);


              

               

                ScadaData sd = null;

                DateTime maxTime = DateTime.Now;//.AddSeconds(-intervalSeconds);
                if (maxTime > endTime)
                {
                    maxTime = endTime;

                }
                DateTime stepTime = beginTime;
                List<string> seqTimeList = new List<string>();
                while (stepTime < maxTime)
                {
                    seqTimeList.Add(stepTime.ToString("HHmmss"));
                    stepTime = stepTime.AddSeconds(intervalSeconds);
                }

                string thisDate = maxTime.ToString("yyyyMMdd");

                string date = null;
                string time = null;

                foreach (var list in dic.Values)
                {
                    sd = list.First();
                    sb.Append("{");
                    AppendScadaHeader(sb, sd);
                    // 增加计数占位符,如果没有替换，-1表示不进行计数
                    sb.Append("\"Count\":-1,");
                    sb.Append("\"TimeValue\":[");

                    int i = list.Count;

                    // 返回记录数
                    int count = 0;

                    if (i > 0)
                    {
                        temp.Clear();

                        foreach (var key in seqTimeList)
                        {
                            temp.Add(key, null);
                        }

                        foreach (var item in list)
                        {
                            var localTime = item.TimeStamp.ToLocalTime();
                            date = localTime.ToString("yyyyMMdd");
                            if (date.CompareTo(thisDate) <= 0)
                            {
                                time = localTime.ToString("HHmmss");
                                temp[time] = new TimeValue { Value = item.Value, Date = date, Time = time, B = true };
                            }
                        }

                        TimeValue pre = null;

                        temp.OrderBy(p => (p.Value == null ? thisDate : p.Value.Date) + p.Key).ToList().ForEach(p =>
                        {
                            if (p.Value == null)
                            {
                                if (pre != null)
                                {
                                    if (p.Key.CompareTo(beginTime.ToString("HHmmss")) >= 0)
                                    {
                                        count++;
                                        AppendScadaValue(sb, pre.Value, thisDate, p.Key, false);
                                        sb.Append(",");
                                    }
                                }
                            }
                            else
                            {
                                pre = p.Value;
                                if (pre.Date == thisDate)
                                {
                                    count++;
                                    AppendScadaValue(sb, pre.Value, pre.Date, pre.Time, pre.B);
                                    sb.Append(",");
                                }
                            }
                        });

                        sb.Remove(sb.Length - 1, 1);
                    }

                    // 替换计数
                    sb.Replace("\"Count\":-1", string.Format("\"Count\":{0}", count));

                    sb.Append("]");
                    sb.Append("},");
                }
                if (sb.Length > 1)
                {
                    sb.Remove(sb.Length - 1, 1);
                }
            }
            sb.Append("]");

            string json = sb.ToString();
            logger.DebugFormat("GetTagDetailValue beforeReturn Cost:{5}ms,paras:({0:yyyyMMddHHmmss},{1},{2},{3},{4})", endTime, totalSeconds, intervalSeconds, string.Join(",", tags), onlyLast, DateTime.Now.Subtract(startTime).TotalMilliseconds);
            logger.DebugFormat(" result = {0}", json);
            return json;
        }

      

        /// <summary>
        /// 获取测点集合的最新值
        /// </summary>
        /// <param name="date"></param>
        /// <param name="tags"></param>
        /// <returns>[{"TagNo":"","Value",""},{"TagNo":"","Value",""}]</returns>
        public static string GetTagListValue(int date, string[] tags)
        {
            if (tags == null || tags.Length == 0)
            {
                return "[]";
            }


            return GetTagListLastValue(GetEndDate(date), oneDaySeconds, defaultInterval, tags);
        }

        /// <summary>
        /// 获取测点集合的最新值
        /// </summary>
        /// <param name="endTime"></param>
        /// <param name="totalSeconds"></param>
        /// <param name="intervalSeconds"></param>
        /// <param name="tags"></param>
        /// <returns>[{"TagNo":"","Value",""},{"TagNo":"","Value",""}]</returns>
        public static string GetTagListLastValue(DateTime endTime, int totalSeconds, int intervalSeconds, string[] tags)
        {
            if (tags == null || tags.Length == 0)
            {
                return "[]";
            }
            if (totalSeconds < 0 || intervalSeconds < 0)
            {
                return "[]";
            }

            
            DateTime startTime = DateTime.Now;
            int millSeconds = int.Parse(ConfigurationManager.AppSettings["DelayOfGetDataFromMongodb"]);
            //当前时间加上时延，防止一直取不到最新数据
            endTime = endTime.AddMilliseconds(millSeconds);
            var task1 = currentScadaDao.GetListAsync(endTime, tags);
            
            List<ScadaData> list = null;
            if (endTime.Date == DateTime.Now.Date)
            {
                list = task1.Result;
                logger.DebugFormat("GetTagListLastValue query Cost:{4}ms,paras:({0:yyyyMMddHHmmss},{1},{2},{3})", endTime, totalSeconds, intervalSeconds, string.Join(",", tags), DateTime.Now.Subtract(startTime).TotalMilliseconds);
                logger.DebugFormat("总记录数为：{0}", list.Count);
            }
            else
            {
                var task2 = scadaDao.GetListAsync(endTime.AddSeconds(-totalSeconds), endTime, tags);
                var result = task2.Result;
                logger.DebugFormat("GetTagListLastValue query Cost:{4}ms,paras:({0:yyyyMMddHHmmss},{1},{2},{3})", endTime, totalSeconds, intervalSeconds, string.Join(",", tags), DateTime.Now.Subtract(startTime).TotalMilliseconds);
                logger.DebugFormat("总记录数为：{0}", result.Count);
                list = new List<ScadaData>();
                result.GroupBy(p => p.TagNo).ToList().ForEach(p =>
                {
                    list.Add(p.OrderByDescending(v => v.TimeStamp).First());
                });
                task1.Result.ForEach(
                    p =>
                    {
                        if (!list.Exists(v=>v.TagNo == p.TagNo))
                        list.Add(p);
                    }
                    );
            }

            list = list.OrderBy(p => p.MessureType).ToList();

          

            StringBuilder sb = new StringBuilder();
            sb.Append("[");

           
            logger.DebugFormat("GetTagListLastValue query Cost:{4}ms,paras:({0:yyyyMMddHHmmss},{1},{2},{3})", endTime, totalSeconds, intervalSeconds, string.Join(",", tags), DateTime.Now.Subtract(startTime).TotalMilliseconds);
            logger.DebugFormat("总记录数为：{0}", list.Count);
           

           foreach (var item in list)
            {
                sb.Append("{");

                sb.AppendFormat("\"TagNo\":\"{0}\",", item.TagNo);
                sb.AppendFormat("\"Value\":\"{0}\"", item.Value);
                sb.Append("},");
            }

            if (sb.Length > 1)
            {
                sb.Remove(sb.Length - 1, 1);
            }
            sb.Append("]");

            return sb.ToString();
        }

        /// <summary>
        /// 获取测点集合的最新值
        /// </summary>
        /// <param name="endTime"></param>
        /// <param name="totalSeconds"></param>
        /// <param name="intervalSeconds"></param>
        /// <param name="tags"></param>
        /// <returns>[{"TagNo":"","Value",""},{"TagNo":"","Value",""}]</returns>
        public static string GetTagListLastValue(DateTime endTime, int totalSeconds, int intervalSeconds, List<SegregateSwitchData> switchObjList)
        {
            string[] tags = switchObjList.Select(item => item.TagNo).ToArray();
            var tagNamedict = switchObjList.ToDictionary(key => key.TagNo, code => code.AliasObjCode);
            var tagIddict = switchObjList.ToDictionary(key => key.TagNo, code => code.OriginObjId);
            if (tags == null || tags.Length == 0)
            {
                return "[]";
            }
            if (totalSeconds < 0 || intervalSeconds < 0)
            {
                return "[]";
            }

            DateTime startTime = DateTime.Now;
            int millSeconds = int.Parse(ConfigurationManager.AppSettings["DelayOfGetDataFromMongodb"]);
            //当前时间加上时延，防止一直取不到最新数据
            endTime = endTime.AddMilliseconds(millSeconds);
            var task1 = currentScadaDao.GetListAsync(endTime, tags);

            List<ScadaData> list = null;
            if (endTime.Date == DateTime.Now.Date)
            {
                list = task1.Result;
                logger.DebugFormat("GetTagListLastValue query Cost:{4}ms,paras:({0:yyyyMMddHHmmss},{1},{2},{3})", endTime, totalSeconds, intervalSeconds, string.Join(",", tags), DateTime.Now.Subtract(startTime).TotalMilliseconds);
                logger.DebugFormat("总记录数为：{0}", list.Count);
            }
            else
            {
                var task2 = scadaDao.GetListAsync(endTime.AddSeconds(-totalSeconds), endTime, tags);
                var result = task2.Result;
                logger.DebugFormat("GetTagListLastValue query Cost:{4}ms,paras:({0:yyyyMMddHHmmss},{1},{2},{3})", endTime, totalSeconds, intervalSeconds, string.Join(",", tags), DateTime.Now.Subtract(startTime).TotalMilliseconds);
                logger.DebugFormat("总记录数为：{0}", result.Count);
                list = new List<ScadaData>();
                result.GroupBy(p => p.TagNo).ToList().ForEach(p =>
                {
                    list.Add(p.OrderByDescending(v => v.TimeStamp).First());
                });
                task1.Result.ForEach(
                    p =>
                    {
                        if (!list.Exists(v => v.TagNo == p.TagNo))
                            list.Add(p);
                    }
                    );
            }

            list = list.OrderBy(p => p.MessureType).ToList();



            StringBuilder sb = new StringBuilder();
            sb.Append("[");


            logger.DebugFormat("GetTagListLastValue query Cost:{4}ms,paras:({0:yyyyMMddHHmmss},{1},{2},{3})", endTime, totalSeconds, intervalSeconds, string.Join(",", tags), DateTime.Now.Subtract(startTime).TotalMilliseconds);
            logger.DebugFormat("总记录数为：{0}", list.Count);


            foreach (var item in list)
            {
                sb.Append("{");

                sb.AppendFormat("\"TagNo\":\"{0}\",", item.TagNo);
                sb.AppendFormat("\"OriginObjId\":\"{0}\",", tagIddict[item.TagNo]);
                sb.AppendFormat("\"AliasObjCode\":\"{0}\",", tagNamedict[item.TagNo]);
                sb.AppendFormat("\"TagValue\":\"{0}\"", item.Value);
                sb.Append("},");
            }

            if (sb.Length > 1)
            {
                sb.Remove(sb.Length - 1, 1);
            }
            sb.Append("]");

            return sb.ToString();
        }
        #endregion
    }
}
