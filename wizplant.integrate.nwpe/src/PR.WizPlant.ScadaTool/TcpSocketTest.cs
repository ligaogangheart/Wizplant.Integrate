using PR.WizPlant.ScadaTool.Command;
using SuperSocket.ClientEngine;
using SuperSocket.ProtoBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PR.WizPlant.ScadaTool
{
    public class TcpSocketTest
    {
        string remoteIP = "127.0.0.1";
        int port = 2012;
        Task<bool> conn = null;
        EasyClient<PackageInfo<ScadaPackageHeader, Byte[]>> innerClient = null;
        public void Run()
        {
            innerClient = new EasyClient<PackageInfo<ScadaPackageHeader, Byte[]>>();
            innerClient.Connected += ec_Connected;
            innerClient.Closed += ec_Closed;
            innerClient.Error += ec_Error;
            innerClient.NewPackageReceived += ec_NewPackageReceived;
            conn = innerClient.ConnectAsync(new IPEndPoint(IPAddress.Parse(remoteIP), port));
            var task = conn.ContinueWith(StartHello);
          //  task.Start();
            
        }

        void ec_NewPackageReceived(object sender, PackageEventArgs<PackageInfo<ScadaPackageHeader, byte[]>> e)
        {
            var header = e.Package.Key;
            switch (header.PackageType)
            {
                case PackageType.HELLO:
                    OnHELLO(e.Package.Body);
                    break;
            }
        }

        #region dealResponse
        public void OnHELLO(byte[] data)
        {
            Console.Write("Recieved Hello:");
            foreach (byte b in data)
            {
                Console.Write("{0:X2},", b);
            }
            Console.WriteLine();
        }
        #endregion

        void ec_Error(object sender, ErrorEventArgs e)
        {
            throw new NotImplementedException();
        }

        void ec_Closed(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Hello()
        {
            var HELLO = CommandBuilder.CreateHELLO();
            innerClient.Send(new ArraySegment<byte>(HELLO.Data));
        }

        void StartHello(Task<bool> conn)
        {
            Hello();
        }

        void ec_Connected(object sender, EventArgs e)
        {
            Hello();
        }
    }
}
