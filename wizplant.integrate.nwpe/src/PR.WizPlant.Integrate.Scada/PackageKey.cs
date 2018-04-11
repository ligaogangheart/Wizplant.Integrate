using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR.WizPlant.Integrate.Scada
{
    public enum PackageKey:byte
    {
        /// <summary>
        /// 握手
        /// </summary>
         HANDS = 0xFF,
        /// <summary>
        /// 全遥信
        /// </summary>
         ALLYX = 0xC1,
        /// <summary>
        /// 修改遥信
        /// </summary>
         MODYX = 0xC2,
        /// <summary>
         /// 全遥测
        /// </summary>
         ALLYC = 0xC3,
        /// <summary>
         /// 修改遥测
        /// </summary>
         MODYC = 0xC4
    }
}
