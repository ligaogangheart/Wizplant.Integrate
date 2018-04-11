using SuperSocket.SocketBase.Command;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR.WizPlant.ScadaMono.Command
{
    public class HANDSHAKE : CommandBase<ScadaSession, BinaryRequestInfo>
    {
        static byte[] HANDSHAKEdata = new byte[16] { 0xEB, 0x90, 0xEB, 0x90, 0xEB, 0x90, 0xFF, 0x00, 0x00, 0x00, 0x05, 0x02, 0x9A, 0x03, 0x78, 0x01 };
        public override void ExecuteCommand(ScadaSession session, BinaryRequestInfo requestInfo)
        {
            Console.WriteLine("ShakeHand Recieved");
            session.Send(HANDSHAKEdata, 0, HANDSHAKEdata.Length);
        }
        
    }
}
