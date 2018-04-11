using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketBase.Logging;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PR.WizPlant.ScadaMono
{
    public class ScadaServer : AppServer<ScadaSession, BinaryRequestInfo>
    {        
         public ScadaServer()
             : base(new DefaultReceiveFilterFactory<ScadaRecieveFilter, BinaryRequestInfo>())
        {
            Console.WriteLine("new ScadaServer()");
        }

      
    }
}
