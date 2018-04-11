using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR.WizPlant.Integrate.CrossBorderManagement
{
    /// <summary>
    /// 地理位置坐标
    /// </summary>
    public class PositionGeog
    {
        //经度
        public double Longitude { get; set; }
        //纬度
        public double Latitude { get; set; }
        //海拔
        public double Altitude { get; set; }
    }
}
