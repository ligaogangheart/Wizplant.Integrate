
using Common.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PR.WizPlant.Integrate.Scada
{
    public class ScadaClient
    {
        /// <summary>
        /// 握手请求数据
        /// </summary>
        static byte[] HANDSHAKEdata = new byte[16] { 0xEB, 0x90, 0xEB, 0x90, 0xEB, 0x90, 0xFF, 0x00, 0x00, 0x00, 0x05, 0x02, 0x9A, 0x03, 0x78, 0x00 };
        /// <summary>
        /// 全遥信请求数据
        /// </summary>
        static byte[] ALLYXdata = new byte[15] { 0xEB, 0x90, 0xEB, 0x90, 0xEB, 0x90, 0xC1, 0x00, 0x00, 0x00, 0x04, 0x02, 0x9A, 0x03, 0x78 };
        /// <summary>
        /// 全遥测请求数据
        /// </summary>
        static byte[] ALLYCdata = new byte[15] { 0xEB, 0x90, 0xEB, 0x90, 0xEB, 0x90, 0xC3, 0x00, 0x00, 0x00, 0x04, 0x02, 0x9A, 0x03, 0x78 };
        /// <summary>
        /// 变化遥信请求数据
        /// </summary>
        static byte[] MODYXdata = new byte[15] { 0xEB, 0x90, 0xEB, 0x90, 0xEB, 0x90, 0xC2, 0x00, 0x00, 0x00, 0x04, 0x02, 0x9A, 0x03, 0x78 };
        /// <summary>
        /// 变化遥测请求数据
        /// </summary>
        static byte[] MODYCdata = new byte[15] { 0xEB, 0x90, 0xEB, 0x90, 0xEB, 0x90, 0xC4, 0x00, 0x00, 0x00, 0x04, 0x02, 0x9A, 0x03, 0x78 };

        /// <summary>
        /// 握手响应数据
        /// </summary>
        static byte[] HANDSHAKEResponse = new byte[16] { 0xEB, 0x90, 0xEB, 0x90, 0xEB, 0x90, 0xFF, 0x00, 0x00, 0x00, 0x05, 0x02, 0x9A, 0x03, 0x78, 0x01 };

        /// <summary>
        /// 同步字
        /// </summary>
        static byte[] SYNCWORD = new byte[6] { 0xEB, 0x90, 0xEB, 0x90, 0xEB, 0x90 };        

        /// <summary>
        /// 返回头长度
        /// </summary>
        static int responseHeaderLength = 11;
        /// <summary>
        /// 遥信数据长度
        /// </summary>
        int yxDataLength = Consts.YXDataLength;

        /// <summary>
        /// 遥测数据长度
        /// </summary>
        int ycDataLength = Consts.YCDataLength;

        

        // 返回头缓存
        byte[] header = null;
        /// <summary>
        /// 返回遥信数据缓存
        /// </summary>
        byte[] yxBuffer = null;
        /// <summary>
        /// 返回遥测数据缓存
        /// </summary>
        byte[] ycBuffer = null;

        /// <summary>
        /// 日志类
        /// </summary>
        private ILog logger = LogManager.GetLogger<ScadaClient>();

        /// <summary>
        /// 内部客户端
        /// </summary>
        private TcpClient innerClient = null;

        /// <summary>
        /// 连接状态
        /// </summary>
        private ConnectState state = ConnectState.UnConnected;

        private Stopwatch stopwatch = null;

        /// <summary>
        /// 读取连接状态
        /// </summary>
        public ConnectState State
        {
            get
            {
                return state;
            }
        }

        /// <summary>
        /// Scada服务器IP
        /// </summary>
        string scadaServerIP ;
        /// <summary>
        /// Scada服务端口
        /// </summary>
        int scadaServerPort;

        #region event
        /// <summary>
        /// 收到要处理数据事件
        /// </summary>
        public event EventHandler<ScadaDataEventArgs> DataRecieved;
        
        /// <summary>
        /// 当收到处理数据时
        /// </summary>
        /// <param name="data">要处理的数据</param>
        /// <param name="dataType">数据类型</param>
        protected void OnDataRecieved(byte[] data, PackageKey packageKey)
        {
            if (DataRecieved != null)
            {
                DataRecieved(this, new ScadaDataEventArgs(data, packageKey));
            }
        }

        /// <summary>
        /// 连接成功事件
        /// </summary>
        public event EventHandler<EventArgs> Connected;

        /// <summary>
        /// 当连接成功时
        /// </summary>
        protected void OnConnected()
        {
            if (Connected != null)
            {
                Connected(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// 连接断开事件
        /// </summary>
        public event EventHandler<DisConnectedEventArgs> DisConnected;

        /// <summary>
        /// 当连接断开时
        /// </summary>
        protected void OnDisConnected(DisConnectedReason reason)
        {
            if (DisConnected != null)
            {
                DisConnected(this, new DisConnectedEventArgs(reason));
            }
        }
        /// <summary>
        /// 当连接断开时
        /// </summary>
        protected void OnDisConnected(DisConnectedReason reason, string message)
        {
            if (DisConnected != null)
            {
                DisConnected(this, new DisConnectedEventArgs(reason, message));
            }
        }
        /// <summary>
        /// 当连接断开时
        /// </summary>
        protected void OnDisConnected(DisConnectedReason reason,string message,Exception ex)
        {
            if (DisConnected != null)
            {
                DisConnected(this, new DisConnectedEventArgs(reason,message,ex));
            }
        }

        /// <summary>
        /// 出现错误事件
        /// </summary>
        public event EventHandler<ErrorEventArgs> Error;

        /// <summary>
        /// 当出现错误时
        /// </summary>
        protected void OnError(Exception ex)
        {
            if (Error != null)
            {
                Error(this, new ErrorEventArgs(ex));
            }
        }

        /// <summary>
        /// 完成请求事件
        /// </summary>
        public event EventHandler<CompletedEventArgs> Completed;

        /// <summary>
        /// 当完成请求时
        /// </summary>
        protected void OnCompleted(PackageKey packageKey,bool hasError)
        {
            if (Completed != null)
            {
                Completed(this, new CompletedEventArgs(packageKey,hasError));
            }
        }
        #endregion

        /// <summary>
        /// 默认构造方法
        /// </summary>
        public ScadaClient()
        {
            initConfig();
            init();
            
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="host">Scada服务器地址</param>
        /// <param name="port">Scada服务端口</param>
        public ScadaClient(string host, int port)
        {
            scadaServerIP = host;
            scadaServerPort = port;
            init();
        }

        /// <summary>
        /// 初始化缓存
        /// </summary>
        void initBuffer()
        {
             header = new byte[responseHeaderLength];
            yxBuffer = new byte[yxDataLength];
            ycBuffer = new byte[ycDataLength];
        }

        void init()
        {
            initBuffer();
            stopwatch = new Stopwatch();
            stopwatch.Start();
        }

        /// <summary>
        /// 初始化配置
        /// </summary>
        private void initConfig()
        {
            scadaServerIP = System.Configuration.ConfigurationManager.AppSettings["ScadaServerIP"];
            if (string.IsNullOrEmpty(scadaServerIP))
            {
                logger.Warn("没有配置ScadaServerIP");
            }
            var val = System.Configuration.ConfigurationManager.AppSettings["ScadaServerPort"];
            if (string.IsNullOrEmpty(val))
            {
                logger.Warn("没有配置ScadaServerPort");
            }
            else if (!int.TryParse(val, out scadaServerPort))
            {
                logger.WarnFormat("配置ScadaServerPort[{0}]不是合法的端口号", val);
            }
        }

        /// <summary>
        /// 连接Scada服务器
        /// </summary>
        /// <returns></returns>
        public bool Connect()
        {
            bool result = true;
            if (innerClient == null)
            {
                try
                {
                    innerClient = new TcpClient();      
                    innerClient.Connect(scadaServerIP, scadaServerPort);
                    state = ConnectState.Connected;
                    OnConnected();
                    result = true;
                }
                catch (Exception ex)
                {
                    result = false;
                    innerClient = null;
                    if (state == Scada.ConnectState.Connected || state == Scada.ConnectState.HandShaked)
                    {
                        state = Scada.ConnectState.Error;
                        OnDisConnected(DisConnectedReason.Error,ex.Message,ex);
                    }
                    logger.ErrorFormat("连接服务[{0}:{1}]时出现错误:{2}", scadaServerIP, scadaServerPort, ex.Message, ex);
                    Console.Write("连接服务[{0}:{1}]时出现错误:{2}", scadaServerIP, scadaServerPort, ex.Message);
                }
            }
            else if(!innerClient.Connected)
            {
                try
                {
                    innerClient.Connect(scadaServerIP, scadaServerPort);
                    state = ConnectState.Connected;
                    OnConnected();
                }
                catch (Exception ex)
                {
                    result = false;
                    innerClient = null;
                    if (state == Scada.ConnectState.Connected || state == Scada.ConnectState.HandShaked)
                    {
                        state = Scada.ConnectState.Error;
                        OnDisConnected(DisConnectedReason.Error, ex.Message, ex);
                    }
                    logger.ErrorFormat("连接服务[{0}:{1}]时出现错误:{2}", scadaServerIP, scadaServerPort, ex.Message, ex);
                    Console.Write("连接服务[{0}:{1}]时出现错误:{2}", scadaServerIP, scadaServerPort, ex.Message);
                }
            }
            return result;
        }

        public void CloseConnection(DisConnectedReason reason)
        {
            if (innerClient != null)
            {
                logger.DebugFormat("Connection State is Connected: {0}", innerClient.Connected);
            }
            state = ConnectState.UnConnected;
            if (innerClient.Connected)
            {
                innerClient.Close();
                OnDisConnected(reason);
            }

           

        }

      
        /// <summary>
        /// int缓存，用来重用，防止实例化大量小内存
        /// </summary>
        byte[] intBuffer = new byte[4];

       
        /// <summary>
        /// 确保连接可用
        /// </summary>
        /// <returns></returns>
        private bool EnsureConnection()
        {
            bool result = false;
            if (innerClient != null && innerClient.Connected)
            {
                result = true;
            }
            else
            {
                result = Connect();
            }
            return result;           
        }

        /// <summary>
        /// 检查连接是否可用
        /// </summary>
        /// <returns>是否可用</returns>
        private bool CheckConnection()
        {
            bool result = false;
            if (innerClient != null && innerClient.Connected)
            {
                result = true;
            }
            return result;
        }

        /// <summary>
        /// 调用握手协议
        /// </summary>
        /// <returns>是否成功</returns>
        public bool HandShake()
        {
            if (!EnsureConnection())
            {
                return false;
            }

            if (state == Scada.ConnectState.HandShaked)
            {
                logger.Info("Has aready handshaked");
                return true;
            }
            

            writeBytes("HANDSHAKE request", HANDSHAKEdata);

            NetworkStream socketStream = null;
            try
            {
                socketStream = innerClient.GetStream();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("获取网络流异常:{0}", ex.Message, ex);
                OnDisConnected(DisConnectedReason.Error);
                return false;
            }
            int size = 0;
            byte[] buffer = null;
            try
            {
                // 发送握手请求
                socketStream.Write(HANDSHAKEdata, 0, HANDSHAKEdata.Length);
                socketStream.Flush();

                buffer = new byte[HANDSHAKEResponse.Length];

                // 读取返回头信息
                size = socketStream.Read(buffer, 0, HANDSHAKEResponse.Length);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("握手请求异常:{0}", ex.Message, ex);
                OnDisConnected(DisConnectedReason.Error);
                return false;
            }

            if (size != HANDSHAKEResponse.Length)
            {
                logger.ErrorFormat("HANDSHAKE responseHeaderSize Error:{0}", size);
                Console.WriteLine("ERROR: the count is " + size);
            }
            else
            {
                writeBytes("HANDSHAKE response", buffer);
            }

            bool succ = BufferHelper.IsEqual(buffer, HANDSHAKEResponse, 0, HANDSHAKEResponse.Length);
            if (!succ)
            {
                Console.WriteLine("ERROR: HANDSHAKE Response ERROR ");
                logger.DebugFormat("ERROR: HANDSHAKE Response ERROR");
                OnDisConnected(DisConnectedReason.Default);
            }
            else
            {
                state = Scada.ConnectState.HandShaked;                
            }
            return succ;
        }

        /// <summary>
        /// 每次调用处理的数据数
        /// </summary>
        int circleDoneCount { get; set; }

        /// <summary>
        /// 每次调用需要保存的数据数
        /// </summary>
        int circleToSaveCount { get; set; }

        /// <summary>
        /// 是否启用记录每次调用需保存对象数
        /// </summary>
        bool toSaveCountEnabled = false;
        /// <summary>
        /// 启用记录每次调用需保存对象数
        /// </summary>
        public void EnableToSaveCount()
        {
            toSaveCountEnabled = true;
        }

        /// <summary>
        /// 需要保存对象数+1
        /// </summary>
        public void AddToSaveObject()
        {
            if (!toSaveCountEnabled)
            {
                return;
            }
            circleToSaveCount++;
        }

        /// <summary>
        /// 调用Scada数据服务，此调用必须单线程进行
        /// </summary>
        /// <param name="packageKey">指令Key</param>
        /// <returns>是否成功</returns>
        public bool Do(PackageKey packageKey)
        {
            long startTime = stopwatch.ElapsedMilliseconds;
            //logger.DebugFormat("Enter Do({0})", packageKey);

            // 重置为0，由于读取网络只用一个线程，因此不用加锁
            circleDoneCount = 0;
            circleToSaveCount = 0;
            bool succ = CheckConnection();
            if (!succ)
            {
                succ = EnsureConnection();
                if (succ)
                {
                    succ = HandShake();
                }
            }
            if (succ)
            {
                NetworkStream socketStream = null;
                try
                {
                    socketStream = innerClient.GetStream();
                }
                catch (Exception ex)
                {
                    OnDisConnected(DisConnectedReason.Error,ex.Message,ex);
                    return false;
                }
            // 处理Scada
            succ = doScada(socketStream, packageKey);
            }

            
            OnCompleted(packageKey,!succ);

            //logger.DebugFormat("Exit Do({0})", packageKey);
            bool needDebug = false;
            if (toSaveCountEnabled)
            {
                needDebug = circleToSaveCount > 0;
            }
            else
            {
                needDebug = circleDoneCount > 0;
            }

            string key = null;

            if (circleDoneCount > 0)
            {                
                key = string.Format("{0}_InvokeTimes_HasValue", packageKey);
                PoolManager.UnlockCounterAddOne(key);
                if (circleToSaveCount > 0)
                {
                    key = string.Format("{0}_InvokeTimes_HasSaveValue", packageKey);
                    PoolManager.UnlockCounterAddOne(key);
                }
            }
            else
            {
                key = string.Format("{0}_InvokeTimes_Empty", packageKey);
                PoolManager.UnlockCounterAddOne(key);
            }
           
            //if (needDebug)
            //{
            //    logger.DebugFormat("{4} Success:{0},ReceivedCount:{1},ToSaveCount:{2}, Total Cost:{3}ms", succ, circleDoneCount, circleToSaveCount,stopwatch.ElapsedMilliseconds - startTime, packageKey);
            //}
            return succ;
        }


        
     
        /// <summary>
        /// 处理Scada数据
        /// </summary>
        /// <param name="socketStream">Socket网络流</param>
        /// <param name="packageKey">请求Key</param>
        /// <returns></returns>
        private bool doScada(NetworkStream socketStream, PackageKey packageKey)
        {           
            // 请求指令
            byte[] requestData = null;
            // 数据长度
            int dataLength = 0;

            switch (packageKey)
            {
                case PackageKey.ALLYX:
                    requestData = ALLYXdata;
                    dataLength = yxDataLength;
                    break;
                case PackageKey.ALLYC:
                    requestData = ALLYCdata;
                    dataLength = ycDataLength;
                    break;
                case PackageKey.MODYX:
                    requestData = MODYXdata;
                    dataLength = yxDataLength;
                    break;
                case PackageKey.MODYC:
                    requestData = MODYCdata;
                    dataLength = ycDataLength;
                    break;
            }
            try
            {
                socketStream.Write(requestData, 0, requestData.Length);
                socketStream.Flush();

                // 读取头部信息
                int size = socketStream.Read(header, 0, responseHeaderLength);
                if (size != responseHeaderLength)
                {
                    logger.ErrorFormat("{0}返回头部长度不对:{1}", packageKey, size);
                    Console.WriteLine("ERROR: the header count is " + size);
                    return false;
                }
            }
            catch (Exception ex)
            {
                OnDisConnected(DisConnectedReason.Error, ex.Message, ex);
                return false;
            }

            
            // 判断同步字
            var succ = BufferHelper.IsEqual(header, SYNCWORD, 0, SYNCWORD.Length);
            // 判断请求Key
            if (!succ || header[SYNCWORD.Length] != (byte)packageKey)
            {
                writeBytes(string.Format("{0} header response", packageKey), header);
                logger.DebugFormat("{0}返回头部信息:{1}", packageKey, BufferHelper.GetByteStringWith0x(header));
                logger.ErrorFormat("{0}返回头部指令不对:{1}",packageKey, header);
                Console.WriteLine("ERROR: the header SYNCWORD ERROR ");
                return false;
            }

            // 读取本次请求返回数据总长度
            int bodyLength = BufferHelper.ReadInt32(header, SYNCWORD.Length + 1);
            
            // 计算返回总长度最后余数，是否还有更多标志位，长度为1
            int mod = bodyLength % dataLength;

            if (bodyLength < 0 || mod != 1)
            {
                writeBytes(string.Format("{0} header response", packageKey), header);
                logger.DebugFormat("{0}返回头部信息:{1}", packageKey, BufferHelper.GetByteStringWith0x(header));
                logger.ErrorFormat("{2}返回头部返回数据长度不对:length[{0}],MOD[{1}]", bodyLength, mod,packageKey);
                Console.WriteLine("ERROR: the header dataLength ERROR ：{0}", bodyLength);
            }
            else
            {
                // 处理数累加
                circleDoneCount += bodyLength / dataLength;
            }

            bool result = doScadaData(socketStream,packageKey,bodyLength);
           
            return result;
        }

      



        

     
        /// <summary>
        /// 读取Scada返回数据
        /// </summary>
        /// <param name="socketStream">socket网络流</param>
        /// <param name="packageKey">方法Key</param>
        /// <param name="bodyLength">头部指示数据总长度</param>
        /// <returns>是否成功，只有全部读取成功才算成功</returns>
        private bool doScadaData(NetworkStream socketStream, PackageKey packageKey, int bodyLength)
        {

            // 数据长度
            int dataLength = 0;
            // 缓冲
            byte[] buffer = null;

            switch (packageKey)
            {
                case PackageKey.ALLYX:
                case PackageKey.MODYX:
                    dataLength = yxDataLength;
                    buffer = yxBuffer;
                    break;
                case PackageKey.ALLYC:
                case PackageKey.MODYC:
                    dataLength = ycDataLength;
                    buffer = ycBuffer;
                    break;
            }

            // 已读取数据长度
            int readsize = 0;
            // 已处理数据记录数
            int idx = 0;

            // 循环读取数据，一直到读完为止
            while (readsize + 1 < bodyLength)
            {
                ++idx;

                int offset = 0;
                try
                {
                    // 读取数据到缓冲
                    int size = socketStream.Read(buffer, offset, dataLength - offset);
                    // 读满一条记录
                    while (size < dataLength - offset)
                    {
                        long startTime = stopwatch.ElapsedMilliseconds;
                        Thread.Sleep(5);
                        offset += size;
                        size = socketStream.Read(buffer, offset, dataLength - offset);
                        PoolManager.CounterAdd(Consts.CounterWait4NetStreamReadTime, stopwatch.ElapsedMilliseconds - startTime);
                    }
                }
                catch (Exception ex)
                {
                    logger.ErrorFormat("{2}返回第{0}条数据时出现错误:{1}", idx, ex.Message, packageKey,ex);
                    Console.WriteLine("{2}返回第{0}条数据时出现错误:{1}", idx, ex.Message, packageKey);
                    return false;
                }

                readsize += dataLength;
                ///TODO: 暂时用同步方法
                // 通知处理数据
                OnDataRecieved(buffer, packageKey);                
            }
            // 获取最后一个标志位
            socketStream.Read(buffer, 0, 1);
            int hasMore = (int)buffer[0];
           // Console.Write("has more:{0}", hasMore);
            if (hasMore == 1)
            {
                return doScada(socketStream, packageKey);
            }
            else
            {
                return true;
            }   
        }

        

        


     /// <summary>
     /// 写字节流
     /// </summary>
     /// <param name="msg">前置消息</param>
     /// <param name="data"></param>
        void writeBytes(string msg, byte[] data)
        {
            return;
            //int len = data.Length;

            if (!string.IsNullOrEmpty(msg))
            {
                Console.Write("{0}: ", msg);
            }
            //Console.Write("{");
            //Console.Write("0x{0:X2}", (int)data[0]);
            //for (int i = 1; i < len; i++)
            //{
            //    Console.Write(", 0x{0:X2}", (int)data[i]);
            //}
           // Console.WriteLine("}");
            Console.Write(BufferHelper.GetByteStringWith0x(data));

        }

       
    }
}
