using Common.Logging;
using PR.WizPlant.Integrate.Sql;
using PR.WizPlant.Integrate.WcfHost.utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using PR.WizPlant.Integrate.Csgii.SBTZSOA;
using PR.WizPlant.Integrate.Csgii.Service;
using PR.WizPlant.Integrate.Csgii.Entities;
using Newtonsoft.Json;


namespace PR.WizPlant.Integrate.WcfHost
{
    /// <summary>
    /// CsgiiSBTZService 的摘要说明
    /// </summary>
    public class CsgiiSBTZService : IHttpHandler
    {
        public ILog logger = LogManager.GetLogger<CsgiiSBTZService>();
        string successReply = "OK";
        string errorPrex = "ERROR";
        public static string[] userTokens = System.Configuration.ConfigurationManager.AppSettings["userToken"].Split(',');

        public void ProcessRequest(HttpContext context)
        {
            var resp = context.Response;
            resp.ContentType = "text/plain";

            string jsonP = context.Request["jsonPCallback"];

            // 方法名称,device,basicinfo,techinfo,csbg,yxqxjl,ztpj
            string methodName = context.Request["methodName"];
            // 对象id
            string objId = context.Request["objId"];

            
            // 用户身份
            string userToken = context.Request["userToken"];
            if (!checkUser(userToken))
            {
                logger.ErrorFormat("userToken[{0}] is invalid", userToken);
                resp.Write(buildInfo(string.Format("{0}:用户身份验证失败!", errorPrex), jsonP));
                return;
            }
            // 设备编码
            string deviceId = null;
            if (string.IsNullOrEmpty(objId))
            {
                deviceId = context.Request["deviceId"];
            }
            else
            {
                var key = objId.ToLower();
                logger.DebugFormat("objId is {0}", key);
                if (CsgiiService.MapCache.ContainsKey(key))
                {
                    deviceId = CsgiiService.MapCache[key];
                }
                else
                {
                    logger.ErrorFormat("objId[{0}] has no map ", key);
                    resp.Write(buildInfo(string.Format("{0}:该设备没有台账信息",errorPrex), jsonP));
                    return;
                }
            }

            if (string.IsNullOrEmpty(methodName))
            {
                try
                {
                    string result = getSBTZ(deviceId);

                    if (result.StartsWith(errorPrex))
                    {
                        logger.ErrorFormat("getSBTZ ERROR:{0} ", result);
                        resp.Write(buildInfo(result, jsonP));
                    }
                    else
                    {
                         resp.Write(buildInfo(result, jsonP));
                    }
                }
                catch (Exception ex)
                {
                    logger.ErrorFormat("读取设备[{0}]的信息时发生错误：{1}", deviceId, ex.Message, ex);
                    resp.Write(buildInfo(string.Format("{0}:没有读取到设备台账信息!", errorPrex), jsonP));
                }
            }
            else
            {
                switch (methodName)
                {
                        // 设备信息
                    case "device":
                        try
                        {
                            var obj = CsgiiService.GetDevice(deviceId);
                            if (obj == null)
                            {
                                logger.ErrorFormat("没有读取设备[{0}]的{1}信息", deviceId, methodName);
                                resp.Write(buildInfo(string.Format("{0}:没有读取到设备台账{1}信息!", errorPrex, methodName), jsonP));
                            }
                            else
                            {
                                resp.Write(buildInfo(obj.ToJsonString(), jsonP));
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.ErrorFormat("读取设备[{0}]的{1}信息时发生错误：{2}", deviceId,methodName, ex.Message, ex);
                            resp.Write(buildInfo(string.Format("{0}:没有读取到设备台账{1}信息!", errorPrex, methodName), jsonP));
                        }
                        break;
                    // 基本信息device,basicinfo,techinfo,csbg,yxqxjl,ztpj
                    case "basicinfo":
                        try
                        {
                            var obj = CsgiiService.GetBasicInfo(deviceId);
                            if (obj == null)
                            {
                                logger.ErrorFormat("没有读取设备[{0}]的{1}信息", deviceId, methodName);
                                resp.Write(buildInfo(string.Format("{0}:没有读取到设备台账{1}信息!", errorPrex, methodName), jsonP));
                            }
                            else
                            {
                                resp.Write(buildInfo(obj.ToJsonString(), jsonP));
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.ErrorFormat("读取设备[{0}]的{1}信息时发生错误：{2}", deviceId, methodName, ex.Message, ex);
                            resp.Write(buildInfo(string.Format("{0}:没有读取到设备台账{1}信息!", errorPrex, methodName), jsonP));
                        }
                        break;
                    // 技术参数
                    case "techinfo":
                        try
                        {
                            var result = CsgiiService.GetJsonTechInfos(deviceId);
                            resp.Write(buildInfo(result, jsonP));
                        }
                        catch (Exception ex)
                        {
                            logger.ErrorFormat("读取设备[{0}]的{1}信息时发生错误：{2}", deviceId, methodName, ex.Message, ex);
                            resp.Write(buildInfo(string.Format("{0}:没有读取到设备台账{1}信息!", errorPrex, methodName), jsonP));
                        }
                        break;
                    // 测试报告
                    case "csbg":
                        try
                        {
                            var objs = CsgiiService.GetCSBGs(deviceId);
                            if (objs == null || objs.Count == 0)
                            {
                                logger.ErrorFormat("没有读取设备[{0}]的{1}信息", deviceId, methodName);
                            }

                            resp.Write(buildInfo(CsgiiCSBG.ToJsonString(objs), jsonP));
                        }
                        catch (Exception ex)
                        {
                            logger.ErrorFormat("读取设备[{0}]的{1}信息时发生错误：{2}", deviceId, methodName, ex.Message, ex);
                            resp.Write(buildInfo(string.Format("{0}:没有读取到设备台账{1}信息!", errorPrex, methodName), jsonP));
                        }
                        break;
                    // 运行缺陷记录
                    case "yxqxjl":
                        try
                        {
                            var objs = CsgiiService.GetYXQXJLs(deviceId);
                            if (objs == null || objs.Count == 0)
                            {
                                logger.ErrorFormat("没有读取设备[{0}]的{1}信息", deviceId, methodName);
                            }

                            resp.Write(buildInfo(CsgiiYXQXJL.ToJsonString(objs), jsonP));
                        }
                        catch (Exception ex)
                        {
                            logger.ErrorFormat("读取设备[{0}]的{1}信息时发生错误：{2}", deviceId, methodName, ex.Message, ex);
                            resp.Write(buildInfo(string.Format("{0}:没有读取到设备台账{1}信息!", errorPrex, methodName), jsonP));
                        }
                        break;
                    // 状态评价
                    case "ztpj":
                        try
                        {
                            var objs = CsgiiService.GetZTPJs(deviceId);
                            if (objs == null || objs.Count == 0)
                            {
                                logger.InfoFormat("没有读取设备[{0}]的{1}信息", deviceId, methodName);
                            }

                            resp.Write(buildInfo(CsgiiZTPJ.ToJsonString(objs), jsonP));
                        }
                        catch (Exception ex)
                        {
                            logger.ErrorFormat("读取设备[{0}]的{1}信息时发生错误：{2}", deviceId, methodName, ex.Message, ex);
                            resp.Write(buildInfo(string.Format("{0}:没有读取到设备台账{1}信息!", errorPrex, methodName), jsonP));
                        }
                        break;
                    case "qxqueryParam":
                        try
                        {
                            var levelResult = CsgiiService.GetDefectLevels();
                            var classResult = CsgiiService.GetDefectClassList();

                            resp.Write(buildInfo(JsonConvert.SerializeObject(new { levelResult, classResult }), jsonP));
                        }
                        catch (Exception ex)
                        {
                            logger.ErrorFormat("读取缺陷等级数据时发生错误：{0}", ex);
                            resp.Write(buildInfo(string.Format("{0}:读取缺陷等级数据时发生错误{1}", errorPrex, methodName), jsonP));
                        }
                        break;
                    case "queryQX":
                        {
                            #region 检验查询参数
                            string queryDefectLevel = context.Request["defectLevel"];
                            string queryDefectClass = context.Request["defectClass"];
                            string queryAriseTimeStr = context.Request["ariseTime"];
                            string querySolveTimeStr = context.Request["solveTime"];
                            DateTime queryAriseTime=DateTime.Now;
                            DateTime querySolveTime=DateTime.Now;
                            QueryTimeEnum queryTimeEnum = QueryTimeEnum.QueryNoTime;

                            if (!string.IsNullOrEmpty(queryAriseTimeStr))
                            {
                                if (DateTime.TryParse(queryAriseTimeStr, out queryAriseTime))
                                {
                                    queryAriseTime = queryAriseTime.Date;
                                    queryTimeEnum = QueryTimeEnum.QueryStart;
                                }
                                else
                                {
                                    logger.ErrorFormat("查询时间格式不争取，查询查询为：{0}", queryAriseTimeStr);
                                    resp.Write(buildInfo(string.Format("查询时间格式不争取，查询查询为：{0}", queryAriseTimeStr), jsonP));
                                    return;
                                }
                            }
                            if (!string.IsNullOrEmpty(querySolveTimeStr))
                            {
                                if (DateTime.TryParse(querySolveTimeStr, out querySolveTime))
                                {
                                    querySolveTime = querySolveTime.Date;
                                    queryTimeEnum = QueryTimeEnum.QueryEnd;
                                }
                                else
                                {
                                    logger.ErrorFormat("查询时间格式不争取，查询查询为：{0}", querySolveTimeStr);
                                    resp.Write(buildInfo(string.Format("查询时间格式不争取，查询查询为：{0}", querySolveTimeStr), jsonP));
                                    return;
                                }
                            }
                            if (!string.IsNullOrEmpty(queryAriseTimeStr) && !string.IsNullOrEmpty(querySolveTimeStr))
                            {
                                queryTimeEnum = QueryTimeEnum.QueryInterval;
                            }
                            #endregion

                            try
                            {
                                var result = CsgiiService.GetYXQXJLsByQuery(deviceId,queryDefectLevel,queryDefectClass,queryAriseTime,querySolveTime,queryTimeEnum);
                               
                                resp.Write(buildInfo(JsonConvert.SerializeObject(result), jsonP));
                            }
                            catch (Exception ex)
                            {
                                logger.ErrorFormat("查询缺陷数据时发生错误：{0}", ex);
                                resp.Write(buildInfo(string.Format("{0}:查询缺陷数据时发生错误{1}", errorPrex, methodName), jsonP));
                            }
                        }
                        break;
                    default:
                        logger.ErrorFormat("不支持读取设备[{0}]的{1}信息", deviceId, methodName);
                        resp.Write(buildInfo(string.Format("不支持读取设备[{0}]的{1}信息！", errorPrex, methodName), jsonP));
                            break;

                }
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

        public string buildInfo(string msg, string jsonP)
        {
            if (string.IsNullOrEmpty(jsonP))
            {
                return msg;
            }
            else
            {
                msg = msg.Replace("\r", "").Replace("\n", "");
                return jsonP + "('" + msg + "')";
            }
        }


        /// <summary>
        /// 获取设备台账信息
        /// </summary>
        /// <param name="sbbm">设备编码</param>
        /// <returns></returns>
        string getSBTZ(string sbbm)
        {
            //var obj = SqlHelper.ExecuteScalar(System.Configuration.ConfigurationManager.AppSettings["strConn"], string.Format("select DeviceId from PRW_Inte_Csgii_DeviceMap where ObjectId='{0}'", objectId));
            //if (obj == null || Convert.IsDBNull(obj))
            //{
            //    return "ERROR:该设备没有台账信息";
            //}
            //var sbbm = obj.ToString();
            string result = null;
            DeviceGisDTO dto = null;
            #region 测试，从文件获取
            //var reader = File.OpenText(HttpContext.Current.Server.MapPath("data.json"));
            //string json = reader.ReadToEnd();
            //reader.Close();
            //dto = SerializeHelper.FromJson<deviceGisDTO>(json);
            #endregion

            #region 正式，调用Web Service获取
            QueryDeviceInfoByIdRequest request = new QueryDeviceInfoByIdRequest();
            SOAServicePortClient service = new SOAServicePortClient();
            logger.DebugFormat("开始调用服务方法:queryDeviceInfoById({0})", sbbm);
            try
            {
                var response = service.queryDeviceInfoById(request);
                if (response.replyCode != successReply)
                {
                    logger.ErrorFormat("调用服务方法:queryDeviceInfoById({0})失败！返回值:{1}", sbbm, response.replyCode);
                    result = string.Format("{1}:远程服务返回{0}", response.replyCode, errorPrex);
                }
                else
                {
                    dto = response.deviceGisDTO;
                }
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("调用服务方法:queryDeviceInfoById({0})失败！错误消息：{1}", ex, sbbm, ex.Message);
                result = string.Format("{1}:调用服务时发生异常{0}", ex.Message, errorPrex);
            }
            finally
            {
                logger.DebugFormat("完成调用服务方法:queryDeviceInfoById({0})", sbbm);
            }           
            #endregion


            if (dto != null)
            {
                result = dto.ToJsonString();
            }

            return result;
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