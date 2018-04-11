using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR.WizPlant.Integrate.Scada
{
    /// <summary>
    /// 连接状态
    /// </summary>
    public enum ConnectState
    {
        /// <summary>
        /// 未连接
        /// </summary>
        UnConnected,
        /// <summary>
        /// 已连接
        /// </summary>
        Connected,
        /// <summary>
        /// 已握手
        /// </summary>
        HandShaked,
        /// <summary>
        /// 异常
        /// </summary>
        Error
    }
}
