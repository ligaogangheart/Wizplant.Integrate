using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR.WizPlant.Integrate.Humiture.Models
{
    /// <summary>
    /// 温湿度数据
    /// </summary>
    public class HumitureData
    {
        /// <summary>
        /// 设备编码
        /// </summary>
        public string mac { get; set; }
        /// <summary>
        /// 设备名称
        /// </summary>
        public string boxname { get; set; }
        /// <summary>
        /// 温度
        /// </summary>
        public string temp { get; set; }
        /// <summary>
        /// 湿度
        /// </summary>
        public string hum { get; set; }
        /// <summary>
        /// 时间
        /// </summary>
        public string updatetime { get; set; }
        /// <summary>
        /// 状态
        /// 00   温湿度数据正常
        ///10   温度黄色警告，湿度正常
        ///11   温度黄色警告，湿度黄色警告
        ///12   温度黄色警告，湿度红色警告
        ///20   温度红色警告，湿度正常
        ///21   温度红色警告，湿度黄色警告
        ///22   温度红色警告，湿度红色警告
        /// </summary>
        public string deviceStatus { get; set; }
    }
}
