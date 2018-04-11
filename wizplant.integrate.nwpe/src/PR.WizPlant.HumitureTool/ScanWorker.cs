using log4net;
using Newtonsoft.Json;
using PR.WizPlant.Integrate.Humiture.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace PR.WizPlant.HumitureTool
{
    public class ScanWorker
    {
       private static string url = ConfigurationManager.AppSettings["serurl"];
        private string getLastestDataUrl = url + "getLastestData/";
        private string getSensorDataUrl = url + "getSensorData/";
        private string mac = "d8001150";

        public void Run() {
            log4net.Config.XmlConfigurator.Configure();
            TokenData token = new TokenData() {  token="0001"};
            string data1=    HttpRequestHelper.HttpPost(getLastestDataUrl+ mac, JsonConvert.SerializeObject(token));
            SerachData serachdata = new SerachData() { token = "0001", startDate = "2018-01-01 9:50:00", endDate = "2018-01-22 9:50:00", pageNo="1",pageSize="10" };
            string data2 = HttpRequestHelper.HttpPost(getSensorDataUrl + mac, JsonConvert.SerializeObject(serachdata));
            SensorObjectData objdata = JsonConvert.DeserializeObject<SensorObjectData>(data2);
           
            OutputLog(string.Format("查询实时温湿度信息：{0}",data1));
            OutputLog(string.Format("查询历史温湿度信息：{0}", data2));
        }



        #region Log
        ILog logger = LogManager.GetLogger("loginfo");
        void OutputLog(string message)
        {
            OutputLog(message, false);
        }
        void OutputLog(string message,bool onlyLog)
        {
            if (!onlyLog)
            {
                Console.WriteLine("{0:HH:mm:ss} {1}", DateTime.Now, message);
            }
            logger.Info(message);
        }

        void OutputLog(string message, Exception ex)
        {
            OutputLog(message, ex,false);
        }

        void OutputLog(string message, Exception ex, bool onlyLog)
        {
            if (!onlyLog)
            {
                Console.WriteLine("{0:HH:mm:ss} Error:{1}", DateTime.Now, message);
            }
            if (ex != null)
            {
                logger.Error(message, ex);
            }
            else
            {
                logger.Error(message);
            }
        }
        #endregion

    }
}
