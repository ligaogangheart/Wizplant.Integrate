using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PR.WizPlant.Integrate.CrossBorderManagement;
using Common.Logging;
using Newtonsoft.Json;

namespace PR.WizPlant.Integrate.WcfHost
{
    /// <summary>
    /// CrossBorderManagementService 的摘要说明
    /// </summary>
    public class CrossBorderManagementService : IHttpHandler
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

            // 用户身份
            string userToken = context.Request["userToken"];
            if (!checkUser(userToken))
            {
                logger.ErrorFormat("userToken[{0}] is invalid", userToken);
                resp.Write(buildInfo(string.Format("{0}:用户身份验证失败!", errorPrex), jsonP));
                return;
            }

            switch (methodName)
            {
                case "getRegions":
                    {
                        string siteId = context.Request["siteId"];

                        if (!string.IsNullOrEmpty(siteId))
                        {
                            try
                            {
                                var regions = CrossBorderService.GetAllRegionDtoBySite(siteId);
                                resp.Write(buildInfo(JsonConvert.SerializeObject(regions), jsonP));
                            }
                            catch (Exception ex)
                            {
                                resp.Write(buildInfo(errorPrex.ToString(), jsonP));
                            }
                        }
                        else
                        {
                            resp.Write(buildInfo(errorPrex.ToString(), jsonP));
                        }
                    }
                    break;
                case "checkInRegion":
                    {
                        string longitudeStr = context.Request["longitude"];
                        string latitudeStr = context.Request["latitude"];
                        string altitudeStr=context.Request["altitude"];
                        string siteId=context.Request["siteId"];

                        double longitude = 0;
                        double latitude = 0;
                        double altitude = 0;
                        if (double.TryParse(longitudeStr, out longitude) && double.TryParse(latitudeStr, out latitude) && double.TryParse(altitudeStr,out altitude))
                        {
                            PositionGeog _position = new PositionGeog() { Longitude=longitude,Latitude=latitude,Altitude=altitude};

                            bool isInRegion = CrossBorderService.CheckIsInRegion(_position,siteId);
                            resp.Write(buildInfo(isInRegion.ToString(),jsonP));
                        }
                        else
                        {
                            logger.ErrorFormat("验证区域参数错误，经度：{0}，纬度：{1}",longitudeStr,latitudeStr);
                            resp.Write(buildInfo(string.Format("验证区域参数错误，经度：{0}，纬度：{1}", longitudeStr, latitudeStr), jsonP));
                        }
                    }
                    break;
                case "insertRegionParam":
                    {
                        string regionParam=context.Request["regionParam"];
                        string siteId = context.Request["siteId"];
                        string regionName=context.Request["regionName"];

                        if (!string.IsNullOrEmpty(regionParam) && !string.IsNullOrEmpty(siteId))
                        {
                            try
                            {
                                CrossBorderService.AddDangerousRegion(siteId, regionName, regionParam);
                                resp.Write(buildInfo(successReply.ToString(), jsonP));
                            }
                            catch (Exception ex)
                            {
                                resp.Write(buildInfo(errorPrex.ToString(), jsonP));
                            }
                            
                        }
                        else {
                            resp.Write(buildInfo(errorPrex.ToString(), jsonP));
                        }
                    }
                    break;
                case "deleteRegion":
                    {
                        string regionId= context.Request["regionId"];

                        if (!string.IsNullOrEmpty(regionId))
                        {
                            try
                            {
                                CrossBorderService.DeleteDangerousRegion(regionId);
                                resp.Write(buildInfo(successReply.ToString(), jsonP));
                            }
                            catch (Exception ex)
                            {
                                resp.Write(buildInfo(errorPrex.ToString(), jsonP));
                            }

                        }
                        else
                        {
                            resp.Write(buildInfo(errorPrex.ToString(), jsonP));
                        }
                    }
                    break;
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

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}