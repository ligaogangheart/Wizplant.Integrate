using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace PR.WizPlant.Integrate.Scada.Entities
{
    [DataContract]
    public class SegregateSwitchData
    {
        [DataMember]
        public string OriginObjId { get; set; }
        [DataMember]
        public string AliasObjCode { get; set; }
        [DataMember]
        public string TagNo { get; set; }
        [DataMember]
        public int TagValue { get; set; }
    }
}
