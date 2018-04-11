using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR.WizPlant.Integrate.Scada.Entities
{
    /// <summary>
    /// Scada映射对象
    /// </summary>
    public partial class ScadaObjectMap
    {
        /// <summary>
        /// 对象Id
        /// </summary>
        public string ObjectId { get; set; }
        /// <summary>
        /// 厂站名
        /// </summary>
        public string SiteName { get; set; }
        /// <summary>
        /// 测点名称
        /// </summary>
        public string TagName { get; set; }
        /// <summary>
        /// 测点描述
        /// </summary>
        public string TagDesc { get; set; }
        /// <summary>
        /// 测点编号
        /// </summary>
        public string TagNo { get; set; }
        /// <summary>
        /// 对应设备名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 对应部件名称
        /// </summary>
        public string Name2 { get; set; }
        /// <summary>
        /// 上下文
        /// </summary>
        public string Context { get; set; }
        /// <summary>
        /// 版本
        /// </summary>
        public string Revision { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public ScadaDataType Type { get; set; }

        /// <summary>
        /// 测量类型
        /// </summary>
        public int MessureType { get; set; }
    }
}
