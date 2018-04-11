using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace PR.WizPlant.Integrate.WcfHost.utilities
{
    public static class SerializeHelper
    {
        static System.Text.Encoding encoding = System.Text.Encoding.UTF8;
        public static string ToXml(object dto)
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

        public static T FromXml<T>(string xml)
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


        public static string ToJson(object dto)
        {
            string result = Newtonsoft.Json.JsonConvert.SerializeObject(dto);
            return result;
        }

        public static T FromJson<T>(string json)
        {
            T result = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
            return result;
        }
    }
}