using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PR.WizPlant.ScadaMono
{
    class Program
    {

        static byte[] HANDSHAKEdata = new byte[16] { 0xEB, 0x90, 0xEB, 0x90, 0xEB, 0x90, 0xFF, 0x00, 0x00, 0x00, 0x05, 0x02, 0x9A, 0x03, 0x78, 0x00 };
        static byte[] ALLYXdata = new byte[15] { 0xEB, 0x90, 0xEB, 0x90, 0xEB, 0x90, 0xC1, 0x00, 0x00, 0x00, 0x04, 0x02, 0x9A, 0x03, 0x78 };
        static byte[] ALLYCdata = new byte[15] { 0xEB, 0x90, 0xEB, 0x90, 0xEB, 0x90, 0xC3, 0x00, 0x00, 0x00, 0x04, 0x02, 0x9A, 0x03, 0x78 };
        static byte[] MODYXdata = new byte[15] { 0xEB, 0x90, 0xEB, 0x90, 0xEB, 0x90, 0xC2, 0x00, 0x00, 0x00, 0x04, 0x02, 0x9A, 0x03, 0x78 };
        static byte[] MODYCdata = new byte[15] { 0xEB, 0x90, 0xEB, 0x90, 0xEB, 0x90, 0xC4, 0x00, 0x00, 0x00, 0x04, 0x02, 0x9A, 0x03, 0x78 };

        static byte[] SYNCWORD = new byte[6] { 0xEB, 0x90, 0xEB, 0x90, 0xEB, 0x90 };

        private Socket innerClient;
        static void Main(string[] args)
        {
            Program p = new Program();
            string scadaServerIP = System.Configuration.ConfigurationManager.AppSettings["ScadaServerIP"];
            int scadaServerPort = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ScadaServerPort"]);
            EndPoint serverAddress = new IPEndPoint(IPAddress.Parse(scadaServerIP), scadaServerPort);
            NetworkStream socketStream = null;
            try
            {
                p.innerClient = new Socket(serverAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                p.innerClient.Connect(serverAddress);
                socketStream = new NetworkStream(p.innerClient);
            }
            catch (Exception ex)
            {
                Console.Write("连接服务器时发生错误：{0} | ", ex.Message);
                Console.WriteLine(ex.StackTrace);
                if (p.innerClient != null)
                {
                    p.innerClient.Close();
                }
                return;
            }

            //p.Setup();
            try
            {

                // 测试握手协议
                Console.WriteLine("测试握手协议");
                p.TestHANDSHAKE(socketStream);
                Console.WriteLine("测试全遥信协议");
                p.TestALLYX(socketStream);
                Console.WriteLine("测试全遥测协议");
                p.TestALLYC(socketStream);
            }
            catch (Exception ex)
            {
                Console.Write("发生错误：{0} | ", ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
            finally
            {
                p.innerClient.Close();
            }
            Console.Read();
        }
        private IServerConfig m_Config;



        public void Setup()
        {
            m_Config = new ServerConfig
            {
                Port = 2012,
                Ip = "Any",
                MaxConnectionNumber = 1000,
                Mode = SocketMode.Tcp,
                Name = "ScadaServer"
            };
        }



        public string getString(byte[] buffer, int offset, int length)
        {
            string result = Encoding.UTF8.GetString(buffer, offset, length).Trim();
            return result;
        }

        byte[] intBuffer = new byte[4];

        public int readInt32(byte[] buffer, int start)
        {
            intBuffer[0] = buffer[start + 3];
            intBuffer[1] = buffer[start + 2];
            intBuffer[2] = buffer[start + 1];
            intBuffer[3] = buffer[start];
            return BitConverter.ToInt32(intBuffer, 0);
        }

        /// <summary>
        /// 测试握手协议
        /// </summary>
        public void TestHANDSHAKE(NetworkStream socketStream)
        {
            writeBytes("HANDSHAKE request", HANDSHAKEdata);




            byte[] buffer = new byte[16];



            socketStream.Write(HANDSHAKEdata, 0, HANDSHAKEdata.Length);
            socketStream.Flush();

            // Console.WriteLine("Sent: HELLO " + (i+1));


            int size = socketStream.Read(buffer, 0, 16);
            if (size != 16)
            {
                Console.WriteLine("ERROR: the count is " + size);
            }
            else
            {
                writeBytes("HANDSHAKE response", buffer);
            }
        }

        /// <summary>
        /// 测试全遥信协议
        /// </summary>
        public void TestALLYX(NetworkStream socketStream)
        {
            writeBytes("ALLYX request", ALLYXdata);


            byte[] header = new byte[11];




            socketStream.Write(ALLYXdata, 0, ALLYXdata.Length);
            socketStream.Flush();



            int size = socketStream.Read(header, 0, header.Length);
            if (size != header.Length)
            {
                Console.WriteLine("ERROR: the header count is " + size);
                return;
            }
            writeBytes("ALLYX header response", header);

            bool succ = IsEqual(header, SYNCWORD, 0, 6);
            if (!succ || header[6] != (byte)PackageKey.ALLYX)
            {
                Console.WriteLine("ERROR: the header SYNCWORD ERROR ");
                return;
            }

            int dataLength = 77;

            int bodyLength = readInt32(header, 7);
            int mod = bodyLength % dataLength;
            if (bodyLength < 0)
            {
                Console.WriteLine("ERROR: the header dataLength ERROR ：{0}", bodyLength);
                return;
            }


            byte[] buffer = new byte[dataLength];
            int readsize = 0;
            int idx = 0;
            while (readsize + 1 < bodyLength)
            {
                size = socketStream.Read(buffer, 0, dataLength);
                if (size != dataLength)
                {
                    Console.WriteLine("ERROR: the body readSize[{0}] != dataLength[{1}]", size, bodyLength);
                    return;
                }
                readsize += size;
                Console.Write("{2} 遥信全名:{0},值：{1}", getString(buffer, 6, 64), (int)buffer[70], ++idx);
            }
            socketStream.Read(buffer, 0, 1);
            int hasMore = (int)buffer[0];
            Console.Write("has more:{0}", hasMore);

        }

        /// <summary>
        /// 测试全遥信协议
        /// </summary>
        public void TestALLYC(NetworkStream socketStream)
        {
            writeBytes("ALLYC request", ALLYCdata);


            byte[] header = new byte[11];




            socketStream.Write(ALLYCdata, 0, ALLYCdata.Length);
            socketStream.Flush();



            int size = socketStream.Read(header, 0, header.Length);
            if (size != header.Length)
            {
                Console.WriteLine("ERROR: the header count is " + size);
                return;
            }
            writeBytes("ALLYC header response", header);

            bool succ = IsEqual(header, SYNCWORD, 0, 6);
            if (!succ || header[6] != (byte)PackageKey.ALLYC)
            {
                Console.WriteLine("ERROR: the header SYNCWORD ERROR ");
                return;
            }

            int dataLength = 80;

            int bodyLength = readInt32(header, 7);
            int mod = bodyLength % dataLength;
            if (bodyLength < 0 || mod != 1)
            {
                Console.WriteLine("ERROR: the header dataLength ERROR ：{0}", bodyLength);
                return;
            }


            byte[] buffer = new byte[dataLength];
            int readsize = 0;
            int idx = 0;
            while (readsize + 1 < bodyLength)
            {
                size = socketStream.Read(buffer, 0, dataLength);
                if (size != dataLength)
                {
                    Console.WriteLine("ERROR: the body readSize[{0}] != dataLength[{1}]", size, bodyLength);
                    return;
                }
                readsize += size;
                Console.Write("{2} 遥测全名:{0},值：{1}", getString(buffer, 6, 64), BitConverter.ToSingle(buffer, 70), ++idx);
            }
            socketStream.Read(buffer, 0, 1);
            int hasMore = (int)buffer[0];
            Console.Write("has more:{0}", hasMore);

        }


        /// <summary>
        /// 判断buffer中从offset开始长度为length的字节是否与flag相等
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="flag"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        private bool IsEqual(byte[] buffer, byte[] flag, int offset, int length)
        {
            if (buffer == null || flag == null)
            {
                return false;
            }

            if (buffer.Length < offset + length || flag.Length != length)
            {
                return false;
            }
            for (int idx = 0; idx < length; idx++)
            {
                if (buffer[offset + idx] != flag[idx])
                {
                    return false;
                }
            }
            return true;
        }

        void writeBytes(string msg, byte[] data)
        {
            int len = data.Length;

            if (!string.IsNullOrEmpty(msg))
            {
                Console.Write("{0}: ", msg);
            }
            Console.Write("{");
            Console.Write("0x{0:X2}", (int)data[0]);
            for (int i = 1; i < len; i++)
            {
                Console.Write(", 0x{0:X2}", (int)data[i]);
            }

            Console.WriteLine("}");

        }
    }
}
