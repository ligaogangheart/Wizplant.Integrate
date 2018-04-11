using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR.WizPlant.ScadaMono
{
    public class ScadaSession : AppSession<ScadaSession, BinaryRequestInfo>
    {
        public const string WelcomeMessageFormat = "Welcome to {0}";
        public const string UnknownCommandMessageFormat = "Unknown command: {0}";

        public new ScadaServer AppServer
        {
            get { return (ScadaServer)base.AppServer; }
        }

       

        protected override void HandleException(Exception e)
        {
            Console.WriteLine("HandleException: " + e.Message);
        }

        protected override void HandleUnknownRequest(BinaryRequestInfo cmdInfo)
        {
            Console.WriteLine("HandleUnknownRequest:" + cmdInfo.Key);
            string response = string.Format(UnknownCommandMessageFormat, cmdInfo.Key);
            Send(response);
        }
    }
}
