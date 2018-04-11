using Common.Logging;
using PR.WizPlant.Integrate.Scada;
using PR.WizPlant.Integrate.Scada.Service;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using PR.WizPlant.Integrate.Scada.Entities;

namespace PR.WizPlant.Integrate.WcfHost
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“ScadaRestWcfService”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 ScadaRestWcfService.svc 或 ScadaRestWcfService.svc.cs，然后开始调试。
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class ScadaRestWcfService : IScadaRestWcfService
    {
       
        static bool needTestData = ConfigurationManager.AppSettings["scadatest"] == "true";

        /// <summary>
        /// 日志
        /// </summary>
        ILog logger = LogManager.GetLogger<ScadaRestWcfService>();
        /// <summary>
        /// 获取设备下测点列表
        /// </summary>
        /// <param name="objId">设备对象Id</param>
        /// <returns>[{"TagNo":"#TagNo"},{"TagNo":"#TagNo"}]</returns>
        [WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "GetObjectTags")]
        public string GetObjectTags(Guid objId)
        {
            string result ="[]";
            logger.DebugFormat("Enter GetObjectTags({0})", objId);

            try
            {
                result = ScadaDataService.GetObjectTags(objId);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error GetObjectTags({0}):{1}", objId, ex);
            }

            logger.DebugFormat("Exit GetObjectTags({0})", objId);
            return result;
        }

        /// <summary>
        /// 根据隔离开关id列表获取对应的测点列表
        /// </summary>
        /// <param name="objId"></param>
        /// <returns></returns>
        [WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "GetSwitchObjectTagsList")]
        public string GetSwitchObjectTagsList(Guid[] objIds)
        {
            string result = "[]";
            logger.DebugFormat("Enter GetObjectTags({0})", objIds);

            try
            {
                result = ScadaDataService.GetSwitchObjectTagsList(objIds);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error GetObjectTags({0}):{1}", objIds, ex);
            }

            logger.DebugFormat("Exit GetObjectTags({0})", objIds);
            return result;
        }

        /// <summary>
        /// 根据隔离开关id列表获取对应的测点列表
        /// </summary>
        /// <param name="objId"></param>
        /// <returns></returns>
        [WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "GetSwitchObjectTagsListByList")]
        public string GetSwitchObjectTagsListByList(List<SegregateSwitchData> switchObjList)
        {
            string result = "[]";
            var dict = switchObjList.ToDictionary(key => key.OriginObjId, code => code.AliasObjCode);
            Guid[] objIds=dict.Keys.Select(key=>new Guid(key)).ToArray();
            logger.DebugFormat("Enter GetObjectTags({0})", objIds);

            try
            {
                result = ScadaDataService.GetSwitchObjectTagsList(dict);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error GetObjectTags({0}):{1}", objIds, ex);
            }

            logger.DebugFormat("Exit GetObjectTags({0})", objIds);
            return result;
        }

        /// <summary>
        /// 获取测点集合的模型值
        /// </summary>
        /// <param name="date"></param>
        /// <param name="tags"></param>
        /// <param name="isShowDetail">是否查询部件名称</param>
        /// <returns>[{"ModelName":"","Value",""},{"ModelName":"","Value",""}]</returns>
        /// <remarks>ModelName，测点对应的模型名称</remarks>
        [WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "GetTagModelValue")]
        public string GetTagModelValue(int date, string[] tags, bool isShowDetail)
        {
            string result = "[]";
            logger.DebugFormat("Enter GetTagModelValue({0},{1},{2})", date, string.Join(",",tags), isShowDetail);
            try
            {
                result = ScadaDataService.GetTagModelValue(date, tags, isShowDetail);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error GetTagModelValue({0},{1},{2}): {3}", date, string.Join(",", tags), isShowDetail,ex.Message,ex);
            }
            logger.DebugFormat("Exit GetTagModelValue({0},{1},{2})", date, string.Join(",", tags), isShowDetail);
            return result;
        }

        /// <summary>
        /// 获取测点集合的模型值
        /// </summary>
        /// <param name="date"></param>
        /// <param name="colName">测点集合组名称</param>
        /// <param name="isShowDetail">是否查询部件名称</param>
        /// <returns>[{"ModelName":"","Value",""},{"ModelName":"","Value",""}]</returns>
        /// <remarks>ModelName，测点对应的模型名称</remarks>
        [WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "GetTagModelValueByColName")]
        public string GetTagModelValueByColName(int date, string colName, bool isShowDetail)
        {
            logger.DebugFormat("Enter GetTagModelValue({0},{1},{2})", date, colName, isShowDetail);
            string val = System.Configuration.ConfigurationManager.AppSettings[colName];
            if (needTestData)
            {

                string modelName = val;
                if (string.IsNullOrEmpty(val))
                {
                    modelName = "/110kV_洗江T线_1811_单接地隔离开关";
                }
                Random r = new Random();
                int status = r.Next(2);

                return "[{\"ModelName\":\"" + modelName + "\",\"Value\":\"" + status + "\"}]";
            }
            string[] tags = null;
            if (string.IsNullOrEmpty(val))
            {
                logger.ErrorFormat("没有配置测点集合:{0}", colName);
            }
            else
            {
                tags = System.Configuration.ConfigurationManager.AppSettings[colName].Split(',');
            }
            string result = GetTagModelValue(date, tags, isShowDetail);
            logger.DebugFormat("Exit GetTagModelValue({0},{1},{2})", date, colName, isShowDetail);
            return result;
        }



        /// <summary>
        /// 获取测点集合的详细值，含测点本身属性
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
        [WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "GetTagDetailValue")]
        public string GetTagDetailValue(int date, string[] tags, bool onlyLast)
        {
            logger.DebugFormat("Enter GetTagDetailValue({0},{1},{2})", date, string.Join(",", tags), onlyLast);
            string result = "[]";
            try
            {
                result = ScadaDataService.GetTagDetailValue(date, tags, onlyLast);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Invoke GetTagDetailValue Error:{0}", ex.Message,ex);
            }
            logger.DebugFormat("Exit GetTagDetailValue({0},{1},{2})", date, string.Join(",", tags), onlyLast);
            return result;
        }
      

        /// <summary>
        /// 获取测点集合的最新值
        /// </summary>
        /// <param name="date"></param>
        /// <param name="colName">测点集合组名称</param>
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
        [WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "GetTagDetailValueByColName")]
        public string GetTagDetailValueByColName(int date, string colName, bool onlyLast)
        {
            logger.DebugFormat("Enter GetTagDetailValueByColName({0},{1},{2})", date, colName, onlyLast);
            string val = System.Configuration.ConfigurationManager.AppSettings[colName];
            string[] tags = null;
            string result ="[]";
            if (string.IsNullOrEmpty(val))
            {
                logger.ErrorFormat("没有配置测点集合:{0}", colName);
            }
            else
            {
                tags = System.Configuration.ConfigurationManager.AppSettings[colName].Split(',');
                result = GetTagDetailValue(date, tags, onlyLast);
            }
            logger.DebugFormat("Exit GetTagDetailValueByColName({0},{1},{2})", date, colName, onlyLast);
            return result;
        }

        /// <summary>
        /// 获取测点集合的模型值
        /// </summary>
        /// <param name="endTime"></param>
        /// <param name="totalSeconds">总区间时间数</param>
        /// <param name="intervalSeconds">数据点间隔秒数</param>
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
        [WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "GetTagDetailValue2")]
        public string GetTagDetailValue2(DateTime endTime, int totalSeconds, int intervalSeconds, string[] tags, bool onlyLast)
        {
            logger.DebugFormat("Enter GetTagDetailValue2({0},{1},{2},{3},{4})", endTime, totalSeconds, intervalSeconds, string.Join(",", tags), onlyLast);
            string result = "[]";
            try
            {
                result = ScadaDataService.GetTagDetailValue(endTime, totalSeconds, intervalSeconds, tags, onlyLast);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("GetTagDetailValue2 Error:{0}", ex.Message, ex);
            }
            logger.DebugFormat("Exit GetTagDetailValue2({0},{1},{2},{3},{4})", endTime, totalSeconds, intervalSeconds, string.Join(",", tags), onlyLast);
            return result;
        }

        /// <summary>
        /// 获取测点集合的模型值
        /// </summary>
        /// <param name="endTime"></param>
        /// <param name="totalSeconds">总区间时间数</param>
        /// <param name="intervalSeconds">数据点间隔秒数</param>
        /// <param name="colName"></param>
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
       [WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "GetTagDetailValueByColName2")]
        public string GetTagDetailValueByColName2(DateTime endTime, int totalSeconds, int intervalSeconds, string colName, bool onlyLast)
        {
            logger.DebugFormat("Enter GetTagDetailValueByColName2({0},{1},{2},{3},{4})", endTime, totalSeconds, intervalSeconds, colName, onlyLast);
            string val = System.Configuration.ConfigurationManager.AppSettings[colName];
            string[] tags = null;
            string result = "[]";
            if (string.IsNullOrEmpty(val))
            {
                logger.ErrorFormat("没有配置测点集合:{0}", colName);
            }
            else
            {
                tags = System.Configuration.ConfigurationManager.AppSettings[colName].Split(',');
                try
                {
                    result = ScadaDataService.GetTagDetailValue(endTime, totalSeconds, intervalSeconds, tags, onlyLast);
                }
                catch (Exception ex)
                {
                    logger.ErrorFormat("GetTagDetailValueByColName2 Error:{0}", ex.Message, ex);
                }
            }
            logger.DebugFormat("Exit GetTagDetailValueByColName2({0},{1},{2},{3},{4})", endTime, totalSeconds, intervalSeconds, colName, onlyLast);
            return result;
        }

        /// <summary>
        /// 获取测点集合的模型值
        /// </summary>
        /// <param name="date">日期</param>
        /// <param name="tags">测点集合</param>
        /// <returns>[{"TagNo":"","Value",""},{"TagNo":"","Value",""}]</returns>
        [WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "GetTagListValue")]
        public string GetTagListValue(int date, string[] tags)
        {
            logger.DebugFormat("Enter GetTagListValue({0},{1})", date, string.Join(",", tags));
            //string result = MongDbScadaService.GetTagListValue(date,tags);
            string result = "[]";
            try
            {
                result = ScadaDataService.GetTagListLastValue(DateTime.Now, 60 * 60, 5, tags);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("GetTagListValue Error:{0}", ex.Message, ex);
            }
            logger.DebugFormat("Exit GetTagListValue({0},{1})", date, string.Join(",", tags));
            return result;
        }

        /// <summary>
        /// 获取隔离开关测点集合的模型值
        /// </summary>
        /// <param name="date">日期</param>
        /// <param name="tags">测点集合</param>
        /// <returns>[{"TagNo":"","Value",""},{"TagNo":"","Value",""}]</returns>
        [WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "GetSwitchTagListValue")]
        public string GetSwitchTagListValue(int date, List<SegregateSwitchData> switchObjList)
        {
            string[] tags=switchObjList.Select(item=>item.TagNo).ToArray();
            logger.DebugFormat("Enter GetTagListValue({0},{1})", date, string.Join(",", tags));
            //string result = MongDbScadaService.GetTagListValue(date,tags);
            string result = "[]";
            try
            {
                result = ScadaDataService.GetTagListLastValue(DateTime.Now, 60 * 60, 5, switchObjList);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("GetTagListValue Error:{0}", ex.Message, ex);
            }
            logger.DebugFormat("Exit GetTagListValue({0},{1})", date, string.Join(",", tags));
            return result;
        }

        /// <summary>
        /// 获取测点集合的最新值
        /// </summary>
        /// <param name="date">日期</param>
        /// <param name="colName">测点集合组名称</param>
        /// <returns>[{"TagNo":"","Value",""},{"TagNo":"","Value",""}]</returns>
        [WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "GetTagListValueByColName")]
        public string GetTagListValueByColName(int date, string colName)
        {
            logger.DebugFormat("Enter GetTagListValueByColName({0},{1})", date, colName);
            string val = System.Configuration.ConfigurationManager.AppSettings[colName];
            string[] tags = null;
            string result = "[]";
            if (string.IsNullOrEmpty(val))
            {
                logger.ErrorFormat("没有配置测点集合:{0}", colName);
            }
            else
            {
                tags = System.Configuration.ConfigurationManager.AppSettings[colName].Split(',');
                result = ScadaDataService.GetTagListValue(date, tags);
            }
            logger.DebugFormat("Exit GetTagListValueByColName({0},{1})", date, colName);
            return result;
        }
    }
}
