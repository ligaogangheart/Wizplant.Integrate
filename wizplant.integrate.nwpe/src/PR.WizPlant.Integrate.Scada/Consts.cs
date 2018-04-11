using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR.WizPlant.Integrate.Scada
{
    public static class Consts
    {
        /// <summary>
        /// 遥信数据长度
        /// </summary>
        public const int YXDataLength = 77;

        /// <summary>
        /// 遥测数据长度
        /// </summary>
        public const int YCDataLength = 80;
        

      

        #region 计数器名称
        /// <summary>
        /// 池最大计数前缀
        /// </summary>
        public const string CounterPoolMaxPrefix = "Counter.Pool.Max.";
        /// <summary>
        /// 池当前计数前缀
        /// </summary>
        public const string CounterPoolCurPrefix = "Counter.Pool.Cur.";

        /// <summary>
        /// 解释成功遥信对象数
        /// </summary>
        public const string CounterParsedYXObject = "Counter.Count.ParsedYXObject";        

        /// <summary>
        /// 解析成功遥测对象数
        /// </summary>
        public const string CounterParsedYCObject = "Counter.Count.ParsedYCObject";

        /// <summary>
        /// 需要保存的遥信对象数
        /// </summary>
        public const string CounterParsedNeedToSaveYXObject = "Counter.Count.NeedToSaveYXObject";
        /// <summary>
        /// 需要保存的遥测对象数
        /// </summary>
        public const string CounterParsedNeedToSaveYCObject = "Counter.Count.NeedToSaveYCObject";

        /// <summary>
        /// 保存的对象数
        /// </summary>
        public const  string CounterSaveObject = "Counter.Count.ObjectSaved";
        /// <summary>
        /// 等待解析字节流及保存到缓存中的时间,单位ms
        /// </summary>
        public const  string CounterWaitForParse = "Counter.Cost.WaitForParse";
        /// <summary>
        /// 解析字节流的时间，单位ms
        /// </summary>
        public const  string CounterTimeForParse = "Counter.Cost.TimeForParse";
        /// <summary>
        /// 所有保存时间汇总
        /// </summary>
        public const  string CounterTimeForSave = "Counter.Cost.TimeForSave";

         /// <summary>
        /// 所有休眠时间汇总
        /// </summary>
        public const string CounterTimeForSleep = "Counter.Cost.TimeForSleep";

        
        /// <summary>
        /// 接收网络数据数量
        /// </summary>
        public const  string CounterDataBufferRecieved = "Counter.Count.DataBufferRecieved";

        /// <summary>
        /// 实例化对象数
        /// </summary>
        public const  string CounterObjectInstance = "Counter.Count.ObjectInstance";

        /// <summary>
        /// 实例化缓存个数
        /// </summary>
        public const  string CounterBufferInstance = "Counter.Count.BufferInstance";

        /// <summary>
        /// 所有保存次数汇总
        /// </summary>
        public const  string CounterExecuteSaveTimes = "Counter.Count.ExecuteSaveTimes";

        /// <summary>
        /// 等待读网络数据时间
        /// </summary>
        public const string CounterWait4NetStreamReadTime = "Counter.Cost.Wait4NetStreamReadTime";
        #endregion
        #region 池名称
        /// <summary>
        /// 遥信字节缓存池名称
        /// </summary>
        public const string PoolYXBufferName = "Pool.YXBuffer";

        /// <summary>
        /// 遥测字节缓存池名称
        /// </summary>
        public const string PoolYCBufferName = "Pool.YCBuffer";
        #endregion
    }
}
