using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR.WizPlant.Integrate.Csgii.Entities
{
    public partial class CsgiiCSBG
    {
        public string ToJsonString()
        {
            return JsonConvert.SerializeObject(this);
            StringBuilder sb = new StringBuilder();           
            sb.AppendLine("{");
            sb.AppendFormat("\"Id\":\"{0}\"", this.Id);
            sb.AppendFormat(",\"DeviceId\":\"{0}\"", this.DeviceId);
            sb.AppendFormat(",\"CSBGMC\":\"{0}\"", this.CSBGMC);
            sb.AppendFormat(",\"RQ\":\"{0}\"", this.RQ);
            sb.AppendFormat(",\"ZY\":\"{0}\"", this.ZY);
            sb.AppendFormat(",\"XZ\":\"{0}\"", this.XZ);
            sb.AppendFormat(",\"JL\":\"{0}\"", this.JL);            
            sb.AppendLine("}");          

            return sb.ToString();
        }

        public static string ToJsonString(List<CsgiiCSBG> list)
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
