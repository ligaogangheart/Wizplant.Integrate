using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR.WizPlant.Integrate.Scada.Entities
{
    public class YXData
    {
        /// <summary>
        /// 设备编号
        /// </summary>
        public int TagNo { get; set; }
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
        /// <summary>
        /// 设备全名
        /// </summary>
        public string FullName { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public byte Value { get; set; }
        /// <summary>
        /// 时标
        /// </summary>
        public DateTime TimeStamp { get; set; }
    }
}
