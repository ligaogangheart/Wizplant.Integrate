using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR.WizPlant.Integrate.Csgii.Entities
{
    public partial class CsgiiYXQXJL
    {
        public string ToJsonString()
        {
            return JsonConvert.SerializeObject(this);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("{");
            sb.AppendFormat("\"Id\":\"{0}\"", this.Id);
            sb.AppendFormat(",\"DeviceId\":\"{0}\"", this.DeviceId);
            sb.AppendFormat(",\"QXBH\":\"{0}\"", this.QXBH);
            sb.AppendFormat(",\"QXLY\":\"{0}\"", this.QXLY);
            sb.AppendFormat(",\"QXDJ\":\"{0}\"", this.QXDJ);
            sb.AppendFormat(",\"QXBX\":\"{0}\"", this.QXBX);
            sb.AppendFormat(",\"QXBW\":\"{0}\"", this.QXBW);
            sb.AppendFormat(",\"QXLX\":\"{0}\"", this.QXLX);
            sb.AppendFormat(",\"QXYY\":\"{0}\"", this.QXYY);
            sb.AppendFormat(",\"QXZT\":\"{0}\"", this.QXZT);
            sb.AppendFormat(",\"FXSJ\":\"{0}\"", this.FXSJ);
            sb.AppendFormat(",\"XQSJ\":\"{0}\"", this.XQSJ);
            sb.AppendLine("}");

            return sb.ToString();
        }

        public static string ToJsonString(List<CsgiiYXQXJL> list)
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
