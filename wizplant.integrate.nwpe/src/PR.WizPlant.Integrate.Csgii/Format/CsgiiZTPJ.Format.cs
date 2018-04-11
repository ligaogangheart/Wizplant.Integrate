using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR.WizPlant.Integrate.Csgii.Entities
{
    public partial class CsgiiZTPJ
    {
        
        public string ToJsonString()
        {
            return JsonConvert.SerializeObject(this);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("{");
            sb.AppendFormat("\"Id\":\"{0}\"", this.Id);
            sb.AppendFormat(",\"DeviceId\":\"{0}\"", this.DeviceId);
            sb.AppendFormat(",\"PJRQ\":\"{0}\"", this.PJRQ);
            sb.AppendFormat(",\"SBJKD\":\"{0}\"", this.SBJKD);
            sb.AppendFormat(",\"SBZYD\":\"{0}\"", this.SBZYD);
            sb.AppendFormat(",\"SBYWJB\":\"{0}\"", this.SBYWJB);
            
            sb.AppendLine("}");

            return sb.ToString();
        }

        public static string ToJsonString(List<CsgiiZTPJ> list)
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
