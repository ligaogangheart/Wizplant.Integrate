
using Common.Logging;
using MongoDB.Bson;
using PR.WizPlant.Integrate.Scada;
using PR.WizPlant.Integrate.Scada.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PR.WizPlant.ScadaTool
{
    /// <summary>
    /// ScadaTcp扫描工作器
    /// </summary>
    public class TcpScanWorker : IWorker
    {
        /// <summary>
        /// Scada报文客户端
        /// </summary>
        private ScadaClient client = null;
        
        /// <summary>
        /// 耗时性能监控
        /// </summary>
        Stopwatch stopWatch = null;

        /// <summary>
        /// 保存间隔时间，0表示立即执行
        /// </summary>
        int saveInterval = 0;

        #region Service
        /// <summary>
        /// Scada服务
        /// </summary>
        IScadaService<ScadaData> scadaService = null;
        MongDbCurrentScadaService currentScadaService = null;
        #endregion

        
        ILog logger = LogManager.GetLogger<TcpScanWorker>();

        /// <summary>
        /// 是否取消
        /// </summary>
        private bool cancel = false;
        /// <summary>
        /// 是否取消，用来退出接收Scada数据，默认为不取消
        /// </summary>
        public bool Cancel
        {
            get { return cancel; }
            set { cancel = value; }
        }

        /// <summary>
        /// 需要集成的厂站名称
        /// </summary>
        string[] inteSites = null;
        

        /// <summary>
        /// 构造方法
        /// </summary>
        public TcpScanWorker()
        {
            client = new ScadaClient();
            client.DataRecieved += client_DataRecieved;
            client.DisConnected += client_DisConnected;
            stopWatch = new Stopwatch();
            PoolManager.CreatePool<ScadaData>();
            PoolManager.CreatePool<byte[]>(Consts.PoolYXBufferName);
            PoolManager.CreatePool<byte[]>(Consts.PoolYCBufferName);
            PoolManager.CreateCounter(Consts.CounterParsedYXObject);
            PoolManager.CreateCounter(Consts.CounterParsedYCObject);
            PoolManager.CreateCounter(Consts.CounterDataBufferRecieved);

            scadaService = new MongDbScadaService();
            currentScadaService = new MongDbCurrentScadaService();

            string val = System.Configuration.ConfigurationManager.AppSettings["InteSites"];
            if (!string.IsNullOrEmpty(val))
            {
                inteSites = val.Split(',');
            }
        }

        /// <summary>
        /// 输出计数器信息
        /// </summary>
        /// <param name="counter"></param>
        private void OutputCounter(Dictionary<string, long> counter)
        {
            var orderedDic = counter.OrderBy(p => p.Key);
            logger.Debug("记数器信息:");
            foreach (var item in orderedDic)
            {
                logger.DebugFormat("记数器类别：{0} 计数:{1}", item.Key, item.Value);
            }

            // 输出时间更新
            preOutputCounterTime = stopWatch.ElapsedMilliseconds;
        }

        /// <summary>
        /// 启动
        /// </summary>
        public void Run()
        {
            // 计时品重启
            stopWatch.Restart();
            long beginTime = stopWatch.ElapsedMilliseconds;

            // 启动报文数据接收解析
            start();

            // 如果启用异步保存，则等等保存完成
            if (doSaveTask != null)
            {
                doneAll = true;
                long startTime = stopWatch.ElapsedMilliseconds;
                doSaveTask.Wait();
                logger.DebugFormat("wait for doSaveTask complete:{0}ms", stopWatch.ElapsedMilliseconds - startTime);
            }

            Console.WriteLine("完成，共耗时:{0}ms", stopWatch.ElapsedMilliseconds - beginTime);
            logger.DebugFormat("任务完成，共耗时:{0}ms", stopWatch.ElapsedMilliseconds - beginTime);

            if (logger.IsDebugEnabled)
            {               
                var counter = PoolManager.GetCounter();
                OutputCounter(counter);
            }

            // 释放池对象
            PoolManager.ReleasePool();
            // 关闭客户端连接
            client.CloseConnection();
            // 停止计时器
            stopWatch.Stop();
        }

        /// <summary>
        /// 保存任务
        /// </summary>
        private Task doSaveTask = null;
        /// <summary>
        /// 是否全部完成，用来退出保存任务
        /// </summary>
        private bool doneAll = false;

        /// <summary>
        /// 是否在处理全数据
        /// </summary>
        bool isAllData = true;

        /// <summary>
        /// 上次输出计数器时间
        /// </summary>
        long preOutputCounterTime = 0;

        /// <summary>
        /// 输出计数器间隔时间（毫秒）
        /// </summary>
        long intervalOutputCounterTime = 5 * 60 * 1000;

        /// <summary>
        /// 调用变化数据间隔时间（毫秒）
        /// </summary>
        long intervalModInvoke = 1000;

        /// <summary>
        /// 开始
        /// </summary>
        void start()
        {
            Console.WriteLine("Begin HandShake");
            // 与服务器握手
            bool succ = client.HandShake();
            Console.WriteLine("HandShake Result:{0}", succ);
            if (!succ)
            {
                RetryConnection();
                return;
            }

            
            if (doSaveTask == null)
            {
                logger.Debug("开启异步保存任务");
                doSaveTask = DoSave();
            }

            logger.Debug("开始全数据处理");

            // 启动要保存对象计数
            client.EnableToSaveCount();

            // 先取一次全数据
            allData();

            logger.DebugFormat("全数据处理完毕:共耗时{0}ms", stopWatch.ElapsedMilliseconds);

            var counter = PoolManager.GetAndResetCounter();
            // 打印全数据相关性能计数器
            OutputCounter(counter);

            // 处理变化数据
            isAllData = false;

            // 是否继续条件
            bool condition = true;

            // 需要休眠毫秒数
            int sleepMilliseconds = 0;

            // 持续取变化数据
            while (condition)
            {
                // 是否被取消，用来跳出循环
                if (Cancel)
                {
                    break;
                }

                if (sleepMilliseconds > 0)
                {
                    // 记录休眠时间
                    PoolManager.CounterAdd(Consts.CounterTimeForSleep, sleepMilliseconds);
                   Thread.Sleep(sleepMilliseconds);
                }

                long startModData = stopWatch.ElapsedMilliseconds;
                // 接收报文变化数据并处理
                modData();               

                // 每间隔 intervalOutputCounterTime 毫秒 输出一下性能计数情况，以监控运行状态
                if (stopWatch.ElapsedMilliseconds - preOutputCounterTime > intervalOutputCounterTime)
                {
                    // 获取计数器
                    counter = PoolManager.GetAndResetCounter();  
                    // 输出
                    OutputCounter(counter);

                    // 自动优化内存，如果池中对象超过5000个，则只保留2000个
                    if (counter[Consts.CounterPoolCurPrefix + typeof(ScadaData).FullName]>5000)
                    {
                        PoolManager.ReleasePool<ScadaData>(2000);
                    }
                }

                // 需要休眠时间 = 间隔时间 - 本轮处理耗时
                sleepMilliseconds = (int)(intervalModInvoke - (stopWatch.ElapsedMilliseconds - startModData));
            }

        }

        /// <summary>
        /// 处理全数据
        /// </summary>
        void allData()
        {
            long begin = stopWatch.ElapsedMilliseconds;

            PackageKey key = PackageKey.ALLYX;
            Console.WriteLine("Begin {0}", key);
            var succ = client.Do(key);
            var scadaData = PoolManager.Get<ScadaData>();
            if (scadaData != null)
            {
                logger.DebugFormat("ScadaData:{0}", Newtonsoft.Json.JsonConvert.SerializeObject(scadaData));
            }
            Console.WriteLine("{2} Result:{0},total cost:{1}ms", succ, stopWatch.ElapsedMilliseconds - begin, key);


            begin = stopWatch.ElapsedMilliseconds;
            key = PackageKey.ALLYC;
            Console.WriteLine("Begin {0}", key);
            succ = client.Do(key);
            scadaData = PoolManager.Get<ScadaData>();
            if (scadaData != null)
            {
                logger.DebugFormat("ScadaData:{0}", Newtonsoft.Json.JsonConvert.SerializeObject(scadaData));
            }
            Console.WriteLine("{2} Result:{0},total cost:{1}ms", succ, stopWatch.ElapsedMilliseconds - begin, key);
        }

        /// <summary>
        /// 处理变化数据
        /// </summary>
        void modData()
        {
            PackageKey key = PackageKey.MODYX;
            long begin = stopWatch.ElapsedMilliseconds;
            Console.WriteLine("Begin {0}", key);
            var succ = client.Do(key);                 
            Console.WriteLine("{2} Result:{0},total cost:{1}ms", succ, stopWatch.ElapsedMilliseconds - begin, key);

            key = PackageKey.MODYC;
            begin = stopWatch.ElapsedMilliseconds;
            Console.WriteLine("Begin {0}", key);
            succ = client.Do(key);
            
            Console.WriteLine("{2} Result:{0},total cost:{1}ms", succ, stopWatch.ElapsedMilliseconds - begin, key);
        }

        void client_DisConnected(object sender, DisConnectedEventArgs e)
        {            
            if (e.Reason != DisConnectedReason.Mannul)
            {
                RetryConnection();
            }
        }

        //private YXData yxData = null;
        //private YCData ycData =  null;

        /// <summary>
        /// 待处理列表锁，用来锁定读网络数据和本地保存数据分别在不同的待处理列表上工作
        /// </summary>
        private object lockListObj = new object();
        private bool workAtFirst = true;

        /// <summary>
        /// 待处理列表1
        /// </summary>
        List<ScadaData> scadaList = new List<ScadaData>();

        /// <summary>
        /// 待处理列表2
        /// </summary>
        List<ScadaData> scadaList2 = new List<ScadaData>();

        public async Task DoSave()
        {            
            while (!doneAll)
            {
                // 是否有数据保存
                bool hasDoSave = false;
                long begin = stopWatch.ElapsedMilliseconds;
                bool curWork;
                lock (lockListObj)
                {
                    curWork = workAtFirst;
                    workAtFirst = !workAtFirst;
                }

                IList<ScadaData> curScadaList = null;

                if (curWork)
                {
                    curScadaList = scadaList;
                }
                else
                {
                    curScadaList = scadaList2;
                }

                int count = curScadaList.Count;
                if (count > 0)
                {

                    try
                    {
                        hasDoSave = true;
                        var startTime = stopWatch.ElapsedMilliseconds;
                        await scadaService.InsertListAsync(curScadaList);
                        PoolManager.CounterAdd(Consts.CounterTimeForSave, stopWatch.ElapsedMilliseconds - startTime);
                        PoolManager.CounterAdd(Consts.CounterSaveObject, count);
                    }
                    catch (Exception ex)
                    {
                        logger.ErrorFormat("Service.SaveAsync ERROR:{0}", ex.Message, ex);
                    }

                    // 通过lockListObj锁已经确保列表不会被别的线程处理，所以不需要加锁
                    foreach (var item in curScadaList)
                    {
                        PoolManager.EnPool(item);
                    }
                    curScadaList.Clear();
                   
                }
              
                if (hasDoSave)
                {
                    // 执行保存次数
                    PoolManager.CounterAddOne(Consts.CounterExecuteSaveTimes);
                }
                long cost = stopWatch.ElapsedMilliseconds - begin;
                int sleep = 1;
                if (cost < saveInterval)
                {
                    sleep = saveInterval - (int)cost;                    
                }
                await Task.Delay(sleep);
            }

            int count2 = scadaList.Count;
            // 处理其他
            if (count2 > 0)
            {
                var startTime = stopWatch.ElapsedMilliseconds;
                try
                {
                await scadaService.InsertListAsync(scadaList);
                PoolManager.CounterAdd(Consts.CounterTimeForSave, stopWatch.ElapsedMilliseconds - startTime);
                PoolManager.CounterAdd(Consts.CounterSaveObject, count2);
                }
                catch (Exception ex)
                {
                    logger.ErrorFormat("when exit the Service.SaveAsync ERROR:{0}", ex.Message, ex);
                } 
            }          
           
            count2 = scadaList2.Count;
            if (count2 > 0)
            {
                var startTime = stopWatch.ElapsedMilliseconds;
                try
                {
                await scadaService.InsertListAsync(scadaList2);
                PoolManager.CounterAdd(Consts.CounterTimeForSave, stopWatch.ElapsedMilliseconds - startTime);
                PoolManager.CounterAdd(Consts.CounterSaveObject, count2);
                }
                catch (Exception ex)
                {
                    logger.ErrorFormat("when exit the Service.SaveAsync ERROR:{0}", ex.Message, ex);
                }
            }
        }

        Dictionary<string, long> debugTimeErrorCount = new Dictionary<string, long>();

        /// <summary>
        /// 临时对象，用来存放当前值
        /// </summary>
        ScadaData tempSD = new ScadaData();

        /// <summary>
        /// 复制对象内容，不含_id
        /// </summary>
        /// <param name="src">源对象</param>
        /// <param name="tgt">目标对象</param>
        void copy(ScadaData src, ScadaData tgt)
        {
            tgt.Name = src.Name;
            tgt.MessureType = src.MessureType;
            tgt.SiteName = src.SiteName;
            tgt.TagNo = src.TagNo;
            tgt.TimeStamp = src.TimeStamp;
            tgt.Type = src.Type;
            tgt.Value = src.Value;
        }

        /// <summary>
        /// 收到数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void client_DataRecieved(object sender, ScadaDataEventArgs e)
        {
            // 临时缓存数据
            byte[] buffer = null;
            PoolManager.CounterAddOne(Consts.CounterDataBufferRecieved);
            Task task = null;
            Task innerTask = null;
            long start = 0;
            switch (e.PackageKey)
            {
                case PackageKey.ALLYX:
                case PackageKey.MODYX:
                    // 从池中拿缓存
                    buffer = PoolManager.Get<byte[]>(Consts.PoolYXBufferName);
                    if (buffer == null)
                    {
                        buffer = new byte[Consts.YXDataLength];
                        // 新实例加1
                        PoolManager.CounterAddOne(Consts.CounterBufferInstance);
                    }
                    // 复制数据到缓存区，不影响网络处理数据
                    BufferHelper.FullCopy(e.Data, buffer);
                    // 异步执行解释和持久操作，让网络不阻塞
                    task = Task.Run(() =>
                    {
                        // 从池中取对象
                        var scadaData = PoolManager.Get<ScadaData>();
                        if (scadaData == null)
                        {
                            scadaData = new ScadaData();
                            // 实例数据加1
                            PoolManager.CounterAddOne(Consts.CounterObjectInstance);
                        }
                        else
                        {
                            scadaData._id = ObjectId.Empty;
                        }
                        long startTime = stopWatch.ElapsedMilliseconds;
                        // 解释数据
                        bool succ = BufferHelper.ParseYXData(buffer, scadaData);
                        // 解释数据计时累加
                        PoolManager.CounterAdd(Consts.CounterTimeForParse, stopWatch.ElapsedMilliseconds - startTime);

                        if (succ)
                        {
                            // 是否需要保存
                            bool needSave = true;
                            if (inteSites != null)
                            {
                                if (!inteSites.Contains(scadaData.SiteName))
                                {
                                    needSave = false;
                                    // 入池
                                    PoolManager.EnPool(scadaData);                                  
                                }
                            }

                            if (needSave)
                            {
                                copy(scadaData, tempSD);
                                tempSD._id = ObjectId.GenerateNewId();
                                innerTask = currentScadaService.UpdateOneAsync(tempSD);

                                PoolManager.CounterAddOne(Consts.CounterParsedNeedToSaveYXObject);
                                client.AddToSaveObject();
                                lock (lockListObj)
                                {
                                    // 当前工作列表
                                    if (workAtFirst)
                                    {
                                        scadaList.Add(scadaData);
                                    }
                                    else
                                    {
                                        scadaList2.Add(scadaData);
                                    }
                                }

                                var ts = DateTime.Now.Subtract(scadaData.TimeStamp);
                                // 如果超过时差相差20秒以上
                                if (ts.TotalSeconds > 20)
                                {

                                    //logger.InfoFormat("变化数据时标异常，与当前时间相差:{1:c}，请确认数据是否正常；{0}", Newtonsoft.Json.JsonConvert.SerializeObject(scadaData), ts);
                                    //logger.InfoFormat("原始值：{0}", BufferHelper.GetByteString(buffer));
                                    PoolManager.UnlockCounterAddOne("DebugTimeErrorYXCount");

                                    if (e.PackageKey == PackageKey.MODYX)
                                    {
                                        //if (PoolManager.UnlockGetCounterValue("DebugTimeErrorYXCount") % 100 == 0)
                                        //{
                                            logger.WarnFormat("变化数据时标异常，与当前时间相差:{1:c}，请确认数据是否正常；{0}", Newtonsoft.Json.JsonConvert.SerializeObject(scadaData), ts);
                                            logger.WarnFormat("原始值：{0}", BufferHelper.GetByteString(buffer));
                                        //}
                                        PoolManager.UnlockCounterAddOne(string.Format("{0}_YX_{1}", scadaData.SiteName, scadaData.TagNo));
                                    }

                                    

                                }
                                else
                                {
                                    PoolManager.UnlockCounterAddOne("DebugTimeCorrectYXCount");
                                }
                            }
                            // 成功解析对象数加1
                            PoolManager.CounterAddOne(Consts.CounterParsedYXObject);
                            if (needSave)
                            {
                                innerTask.Wait();
                            }
                        }
                        
                            // 回收缓存对象入池
                        PoolManager.EnPool<byte[]>(Consts.PoolYXBufferName, buffer);

                    });
                    start = stopWatch.ElapsedMilliseconds;
                    task.Wait();
                    PoolManager.CounterAdd(Consts.CounterWaitForParse, stopWatch.ElapsedMilliseconds - start);
                    break;
                case PackageKey.ALLYC:
                case PackageKey.MODYC:
                    // 缓存数据放入池中，防止大量创建缓存对象
                    buffer = PoolManager.Get<byte[]>(Consts.PoolYCBufferName);
                    if (buffer == null)
                    {
                        buffer = new byte[Consts.YCDataLength];
                        PoolManager.CounterAddOne(Consts.CounterBufferInstance);
                    }
                    // 复制数据到缓存区，不影响网络处理数据
                    BufferHelper.FullCopy(e.Data, buffer);
                    // 异步执行解释和持久操作，让网络不阻塞
                    task = Task.Run(() =>
                    {
                        //从池中获取对象
                        var scadaData = PoolManager.Get<ScadaData>();
                        if (scadaData == null)
                        {
                            scadaData = new ScadaData();
                            PoolManager.CounterAddOne(Consts.CounterObjectInstance);
                        }
                        else
                        {
                            scadaData._id = ObjectId.Empty;
                        }
                       long startTime = stopWatch.ElapsedMilliseconds;
                        // 解释数据
                        bool succ = BufferHelper.ParseYCData(buffer, scadaData);
                        PoolManager.CounterAdd(Consts.CounterTimeForParse, stopWatch.ElapsedMilliseconds - startTime);
                        if (succ)
                        {
                            // 是否需要保存
                            bool needSave = true;
                            if (inteSites != null)
                            {
                                if (!inteSites.Contains(scadaData.SiteName))
                                {
                                    needSave = false;
                                    // 入池
                                    PoolManager.EnPool(scadaData);
                                }
                            }

                            if (needSave)
                            {
                                copy(scadaData, tempSD);
                                tempSD._id = ObjectId.GenerateNewId();
                                innerTask = currentScadaService.UpdateOneAsync(tempSD);

                                PoolManager.CounterAddOne(Consts.CounterParsedNeedToSaveYCObject);
                                client.AddToSaveObject();
                                lock (lockListObj)
                                {
                                    if (workAtFirst)
                                    {
                                        scadaList.Add(scadaData);
                                    }
                                    else
                                    {
                                        scadaList2.Add(scadaData);
                                    }
                                }
                                var ts = DateTime.Now.Subtract(scadaData.TimeStamp);
                                // 如果超过时差相差20秒以上
                                if (ts.TotalSeconds > 20)
                                {
                                    //logger.InfoFormat("变化时标异常，请确认数据是否正常；{0}", Newtonsoft.Json.JsonConvert.SerializeObject(scadaData));
                                    //logger.InfoFormat("原始值：{0}", BufferHelper.GetByteString(buffer));
                                    PoolManager.UnlockCounterAddOne("DebugTimeErrorYCCount");
                                    if (e.PackageKey == PackageKey.MODYC)
                                    {
                                        //if (PoolManager.UnlockGetCounterValue("DebugTimeErrorYCCount") % 100 == 0)
                                        //{
                                            logger.InfoFormat("变化数据时标异常，与当前时间相差:{1:c}，请确认数据是否正常；{0}", Newtonsoft.Json.JsonConvert.SerializeObject(scadaData), ts);
                                            logger.InfoFormat("原始值：{0}", BufferHelper.GetByteString(buffer));
                                        //}
                                        PoolManager.UnlockCounterAddOne(string.Format("{0}_YC_{1}", scadaData.SiteName, scadaData.TagNo)); 
                                    }
                                    
                                }
                                else
                                {
                                    PoolManager.UnlockCounterAddOne("DebugTimeCorrectYCCount");
                                }                                
                            }
                            PoolManager.CounterAddOne(Consts.CounterParsedYCObject);
                            if (needSave)
                            {
                                innerTask.Wait();
                            }
                        }
                            // 回收缓存对象入池
                        PoolManager.EnPool<byte[]>(Consts.PoolYCBufferName, buffer);
                        
                    });
                    start = stopWatch.ElapsedMilliseconds;
                    task.Wait();
                    PoolManager.CounterAdd(Consts.CounterWaitForParse, stopWatch.ElapsedMilliseconds - start);
                    break;
            }
        }


        int retryTimes = 0;
        void RetryConnection()
        {
            logger.InfoFormat("第{0}重试连接");
            if (++retryTimes > 5)
            {
                logger.ErrorFormat("重试次数超过5次，停止重试");
                return;
            }
            Thread.Sleep(5000);
            bool succ = client.HandShake();
            if (succ)
            {
                retryTimes = 0;
                if (isAllData == false)
                {
                    allData();
                }
            }            
        }
    }
}
