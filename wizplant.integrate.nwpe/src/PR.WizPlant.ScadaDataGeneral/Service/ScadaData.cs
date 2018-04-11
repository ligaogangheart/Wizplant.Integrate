using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Runtime.Serialization;

namespace PR.WizPlant.ScadaDataGeneral.Service
{
    [DataContract]
    public enum ScadaDataType
    {
        [EnumMember]
        YX = 1,
        [EnumMember]
        YC = 2
    }

    [DataContract]
    public class ScadaData
    {
        /// <summary>
        /// 设备编号
        /// </summary>
        [DataMember(Name="TagNo")]
        public string TagNo { get; set; }
        /// <summary>
        /// 量测类型
        /// </summary>
        [DataMember(Name = "MessureType")]
        public int MessureType { get; set; }
        /// <summary>
        /// 设备名
        /// </summary>
        [DataMember(Name = "Name")]
        public string Name { get; set; }
        /// <summary>
        /// 厂站名
        /// </summary>
        [DataMember(Name = "SiteName")]
        public string SiteName { get; set; }
        /// <summary>
        /// 数据类型，YX，YC
        /// </summary>
        [DataMember(Name = "Type")]
        public ScadaDataType Type { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        [DataMember(Name = "Value")]
        public float Value { get; set; }
        /// <summary>
        /// 时标
        /// </summary>
        [DataMember(Name = "TimeStamp")]
        public DateTime TimeStamp { get; set; }
    }
}
