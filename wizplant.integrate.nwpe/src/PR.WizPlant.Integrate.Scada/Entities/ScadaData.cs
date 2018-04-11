using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR.WizPlant.Integrate.Scada.Entities
{
    /// <summary>
    /// Scada数据类型
    /// </summary>
    public enum ScadaDataType
    {
        YX =1,
        YC =2
    }
    /// <summary>
    /// Scada数据
    /// </summary>
    public partial class ScadaData
    {
        /// <summary>
        /// 设备编号
        /// </summary>
        public string TagNo { get; set; }
        /// <summary>
        /// 量测类型
        /// </summary>
        public int MessureType { get; set; }
        /// <summary>
        /// 设备名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 厂站名
        /// </summary>
        public string SiteName { get; set; }
        ///// <summary>
        ///// 设备全名
        ///// </summary>
        //public string FullName { get; set; }

        /// <summary>
        /// 数据类型，YX，YC
        /// </summary>
        public ScadaDataType Type { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public float Value { get; set; }
        /// <summary>
        /// 时标
        /// </summary>
        public DateTime TimeStamp { get; set; }
    }
}
