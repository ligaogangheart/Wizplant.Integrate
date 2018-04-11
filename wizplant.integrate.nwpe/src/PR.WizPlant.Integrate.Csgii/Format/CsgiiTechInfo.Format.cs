using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR.WizPlant.Integrate.Csgii.Entities
{
    public partial class CsgiiTechInfo
    {
        public string ToJsonString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            sb.AppendFormat("\"TechParamName\":\"{0}\",\"TechParamValue\":\"{1}\"", this.TechParamName, this.TechParamValue);
            sb.Append("}");

            return sb.ToString();
        }
        public static string ToJsonString(List<CsgiiTechInfo> list)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            if (list != null && list.Count > 0)
            {
                foreach (var item in list)
                {
                    sb.Append("{");
                    sb.AppendFormat("\"TechParamName\":\"{0}\",\"TechParamValue\":\"{1}\"", item.TechParamName, item.TechParamValue);
                    sb.Append("},");
                }
                sb.Remove(sb.Length - 1, 1);
            }
            sb.Append("]");

            return sb.ToString();
        }
    }

    
}
