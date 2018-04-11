using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR.WizPlant.Integrate.Csgii.Entities
{
    public partial class CsgiiDevice
    {
        public string ToJsonString()
        {
            return JsonConvert.SerializeObject(this);
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            sb.AppendFormat("\"Id\":\"{0}\"", this.Id);
            sb.AppendFormat(",\"DeviceId\":\"{0}\"", this.DeviceId);
            sb.AppendFormat(",\"Station\":\"{0}\"", this.Station);
            sb.AppendFormat(",\"DeviceName\":\"{0}\"", this.DeviceName);
            sb.AppendFormat(",\"FullPath\":\"{0}\"", this.FullPath);
            sb.AppendFormat(",\"ClassPath\":\"{0}\"", this.ClassPath);
            sb.AppendFormat(",\"LevelType\":\"{0}\"", this.LevelType);
            sb.AppendFormat(",\"Status\":\"{0}\"", this.Status);
            sb.Append("}");

            return sb.ToString();
        }

        public static string ToJsonString(List<CsgiiDevice> list)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            if (list != null && list.Count > 0)
            {
                foreach (var item in list)
                {
                    sb.AppendFormat("{0},", item.ToJsonString());
                }
                sb.Remove(sb.Length - 1, 1);
            }
            sb.Append("]");

            return sb.ToString();
        }
    }
}
