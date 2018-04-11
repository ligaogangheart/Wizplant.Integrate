using SuperSocket.SocketBase.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR.WizPlant.ScadaMono
{
    public interface IScadaSetup
    {
        void Setup(IRootConfig rootConfig, IServerConfig serverConfig);
    }
}
