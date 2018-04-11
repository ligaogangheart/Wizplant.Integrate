using Common.Logging;
using PR.WizPlant.Integrate.Csgii.SBTZ;
using PR.WizPlant.Integrate.Sql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace PR.WizPlannt.IntegrateTest
{
    public class TestWorker:IWorker
    {
        IDeviceInfoForGisImpl_queryDeviceInfoById service = new DeviceInfoForGisImpl_queryDeviceInfoByIdClient("IDeviceInfoForGisImpl_queryDeviceInfoByIdPort");
        queryDeviceInfoByIdRequest1 request = null;
        queryDeviceInfoByIdResponse1 response = null;
        string connectionString = System.Configuration.ConfigurationManager.AppSettings["strConn"];

        System.Text.Encoding encoding = System.Text.Encoding.UTF8;

        public void Run2()
        {
            var reader = File.OpenText("../../data.json");
            string json = reader.ReadToEnd();
            
            reader.Close();
            var obj = fromJson<deviceGisDTO>(json);

            Console.WriteLine("json:{0}", obj.ToString());
        }

        public void GenJsonMethod(Type t)
        {
           // Type t = entity.GetType();
            var properties = t.GetProperties();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("sb.Append(\"{\");");

            int i = 0;
            foreach (var p in properties)
            {
               var attrs = p.GetCustomAttributes(typeof(DataMemberAttribute), false);
               if (attrs == null || attrs.Length == 0)
               {
                   continue ;
               }
               DataMemberAttribute dm = attrs[0] as DataMemberAttribute;
                string name = p.Name;
                if (!String.IsNullOrEmpty(dm.Name))
                {
                    name = dm.Name;
                }
                if (++i == 1)
                {
                    sb.AppendFormat("sb.AppendFormat(\"\\\"{0}\\\":\\\"{1}\\\"\", this.{2});", name, "{0}",p.Name);
                }
                else
                {
                    sb.AppendFormat("sb.AppendFormat(\",\\\"{0}\\\":\\\"{1}\\\"\", this.{2});", name, "{0}", p.Name);
                }
                sb.AppendLine();
            }
            sb.AppendLine("sb.Append(\"}\");");

            OutputLog("序列化代码:\n" + sb.ToString());
        }
        
        public void Run()
        {
            OutputLog("启动测试......");
            request = new queryDeviceInfoByIdRequest1(new queryDeviceInfoByIdRequest());
            string sbbms = System.Configuration.ConfigurationManager.AppSettings["sbbms"];
            if (string.IsNullOrEmpty(sbbms))
            {
                OutputLog("没有配置设备编码!");
                return;
            }

            var bms = sbbms.Split(',');

            OutputLog(string.Format("共有{0}设备台账信息需要获取,编码为：{1}",bms.Length,sbbms));

            Stopwatch sw = new Stopwatch();
            foreach (var bm in bms)
            {
                sw.Restart();
                try
                {
                    invoke(bm);
                }
                catch (Exception ex)
                {
                    OutputLog(string.Format("获取设备[{0}]的台账信息失败,错误：{1}", bm, ex.Message),ex);
                }
                finally
                {
                    OutputLog(string.Format("完成设备[{0}]的台账信息获取,耗时：{1}ms", bm, sw.ElapsedMilliseconds));
                }
            }
            OutputLog("测试结束");
        }

        public void TestBulkInsert()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            var dtSchema = SqlHelper.ExecuteEmptyDataTable(connectionString, "PRW_Inte_SCADA_EFile1");
            dtSchema.TableName = "PRW_Inte_SCADA_EFile1";
            var dt = dtSchema.Clone();
            int mydate = 20160101;
            
            int j = 100000;
            DataRow r;
            for (int i = 1; i < 100000001; i++)
            {
                if (i % 100000 == 0)
                {
                    Console.WriteLine("to {0} build cost:{1}ms", i, sw.ElapsedMilliseconds);
                    sw.Restart();
                    bulkInsertDT(dt);
                    Console.WriteLine("{0} bulk insert cost:{1}ms", i, sw.ElapsedMilliseconds);
                    sw.Restart();
                    
                    mydate += 1;
                    j = 100000;
                    dt = dtSchema.Clone();
                }
                j++;
                r = dt.NewRow();
                dt.Rows.Add(r); 
                r["Id"] = Guid.NewGuid();
                r["CreateDate"] = mydate;
                r["CreateTime"] = j.ToString();
                r["SiteName"] = "adfsaf";
                r["TagId"] = "sadfas";
                r["TagName"] = "sadfas";
                r["MeasureValue"] = "0";
                r["Status"] = 0;
                r["Type"] = "sadfas";                
            }
        }

        void bulkInsertDT(DataTable dt)
        {
            SqlHelper.BulkInsert(connectionString, dt);
        }


        void invoke(string deviceId)
        {
            request.queryDeviceInfoByIdRequest.deviceId = deviceId;

            response = service.queryDeviceInfoById(request);

            //response = new queryDeviceInfoByIdResponse1(new queryDeviceInfoByIdResponse() { deviceGisDTO = new queryDeviceInfoByIdResponseDeviceGisDTO(), replyCode = "OK" });
            

            switch (response.queryDeviceInfoByIdResponse.replyCode)
            {
                case "OK":
                    success(response.queryDeviceInfoByIdResponse.deviceGisDTO);
                    break;
                default:
                    OutputLog(string.Format("获取设备[{0}]的台账信息失败,replyCode：{1}", deviceId, response.queryDeviceInfoByIdResponse.replyCode), null);
                    break;                    

            }
        }

        void success(deviceGisDTO dto)
        {
            if (logger.IsDebugEnabled)
            {
                logger.DebugFormat("台账信息(XML): {0}", toXml(dto));
                logger.DebugFormat("台账信息(JSON): {0}", toJson(dto));
            }
           
        }

        string toXml(deviceGisDTO dto)
        {
            string result = null;
            XmlSerializer xmlserializer = new XmlSerializer(dto.GetType());
            using (MemoryStream ms = new MemoryStream())
            {
                xmlserializer.Serialize(ms, dto);
                result = encoding.GetString(ms.ToArray());
            }
            return result;
        }

        T fromXml<T>(string xml)
        {
            T result = default(T);
            XmlSerializer xmlserializer = new XmlSerializer(typeof(T));
            using (Stream xmlstream = new MemoryStream(encoding.GetBytes(xml)))
            {
                using (XmlReader reader = XmlReader.Create(xmlstream))
                {
                    object obj = xmlserializer.Deserialize(reader);
                    result = (T)obj;
                }
            }
            return result;
        }


        string toJson(deviceGisDTO dto)
        {
            string result = Newtonsoft.Json.JsonConvert.SerializeObject(dto);           
            return result;
        }

        T fromJson<T>(string json)
        {
            T result = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);           
            return result;
        }


        #region Log
        ILog logger = LogManager.GetLogger("csgii");
        void OutputLog(string message)
        {
            Console.WriteLine("{0:HH:mm:ss} {1}", DateTime.Now, message);
            logger.InfoFormat(message);
        }

        void OutputLog(string message,  Exception ex)
        {
            Console.WriteLine("{0:HH:mm:ss} Error:{1}", DateTime.Now, message);
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
