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
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PR.WizPlant.ScadaTool
{
    public class VitureScadacsWorker:IVitureScada,IWorker
    {

        public static string strConn = System.Configuration.ConfigurationManager.AppSettings["strConn"];//数据库连接字段
        public static string AllDataPath = System.Configuration.ConfigurationManager.AppSettings["AllDataPath"];//e文件存放位置
        public static string LastestDataPath = System.Configuration.ConfigurationManager.AppSettings["LastestDataPath"];//处理成功的e文件保存位置
        /// <summary>
        /// 是否取消
        /// </summary>
        private bool cancel=false;

        public bool Cancel {
            get { return cancel; }
            set { cancel = value; }
        }
        /// <summary>
        /// 耗时性能监控
        /// </summary>
        Stopwatch stopWatch = null;
        ScadaData tempSD = null;
        /// <summary>
        /// 保存间隔时间，0表示立即执行
        /// </summary>
        int saveInterval = 0;
        #region Service
       private MongDbScadaService scadaService = null;
       private MongDbCurrentScadaService currentScadaService = null;
        #endregion

        ILog logger = LogManager.GetLogger<TcpScanWorker>();
        public static T GetObjFromJson<T>(string jsonStr)
        {
            T obj = Activator.CreateInstance<T>();
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonStr)))
            {
                System.Runtime.Serialization.Json.DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(obj.GetType());
                return (T)jsonSerializer.ReadObject(ms);
            }
        }


        public VitureScadacsWorker() {
          
            stopWatch = new Stopwatch();
            //PoolManager.CreatePool<ScadaData>();
            //PoolManager.CreatePool<byte[]>(Consts.PoolYXBufferName);
            //PoolManager.CreatePool<byte[]>(Consts.PoolYCBufferName);
            //PoolManager.CreateCounter(Consts.CounterParsedYXObject);
            //PoolManager.CreateCounter(Consts.CounterParsedYCObject);
            //PoolManager.CreateCounter(Consts.CounterDataBufferRecieved);
            currentScadaService = new MongDbCurrentScadaService();

            scadaService = new MongDbScadaService();
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
        /// httpclient获取生成的json数据
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public List<ScadaData> GetDataFromUrl(string url) {
            try
            {
                WebRequest webRequest = WebRequest.Create(url);
                WebResponse webResponse = webRequest.GetResponse();

                Stream stream = webResponse.GetResponseStream();

                StreamReader reader = new StreamReader(stream);
                string result = reader.ReadToEnd();

                List<ScadaData> RecivedDataModels = GetObjFromJson<List<ScadaData>>(result);
                return RecivedDataModels;
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        /// <summary>
        /// 获取全遥信，全遥测数据
        /// </summary>
        /// <returns></returns>
        public async Task GetAllData()
        {
            //scadaService.DeleteManyAsync().Wait();
            //currentScadaService.DeleteAllAsync().Wait();
            string allDataUrl = AllDataPath;
            List<ScadaData> allData = GetDataFromUrl(allDataUrl);
            Console.WriteLine("已从数据生成器获取{0}条全遥信、全遥测数据，正在向Mongodb数据库插入....",allData.Count);
          await  scadaService.InsertListAsync(allData);
          currentScadaService.InsertListAsync(allData).Wait();
        }
        /// <summary>
        /// copy数据到临时变量
        /// </summary>
        /// <param name="src"></param>
        /// <param name="tgt"></param>
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
        /// 获取变遥测、遥信数据
        /// </summary>
        /// <returns></returns>
        public async Task  GetLastData() {
            string LastestDataUrl = LastestDataPath;
            List<ScadaData> lastestData = GetDataFromUrl(LastestDataUrl);
            Console.WriteLine("已从数据生成器获取{0}条变化遥信、变化遥测数据，正在向Mongodb数据库插入....",lastestData.Count);
            for (int i = 0; i < lastestData.Count; i++) {
                tempSD = new ScadaData();
                copy(lastestData[i],tempSD);
                tempSD._id = ObjectId.GenerateNewId();
                await currentScadaService.UpdateOneAsync(tempSD);
            }
             await scadaService.InsertListAsync(lastestData);
             count++;
        }
        /// <summary>
        /// 保存任务
        /// </summary>
        private Task doSaveAllTask = null;
        private Task doSaveLastestTask = null;
        /// <summary>
        /// 是否全部完成，用来退出保存任务
        /// </summary>
        private bool doneAll = false;

        /// <summary>
        /// 上次输出计数器时间
        /// </summary>
        long preOutputCounterTime = 0;

        /// <summary>
        /// 输出计数器间隔时间（毫秒）
        /// </summary>
        long intervalOutputCounterTime = 5 * 60 * 1000;
        int count = 0;
        /// <summary>
        /// 调用变化数据间隔时间（毫秒）
        /// </summary>
        long intervalModInvoke = 5000;

        public void start() {

            doSaveAllTask = GetAllData();   //先获取一次全遥信、遥测数据
           
            var counter = PoolManager.GetAndResetCounter();
            OutputCounter(counter);
            bool condition = true;
            int sleepMilliseconds = 0;
            string countKey = string.Format("{0}{1}", Consts.CounterPoolCurPrefix, typeof(ScadaData).FullName);
            bool IsFirstSave = true;
          
            while (condition)   //后每5秒获取一次变遥信、遥测数据
            {
                if (doSaveAllTask.IsCompleted && IsFirstSave)
                {
                    Console.WriteLine("已向mongodb中插入全遥信、全遥测数据");
                    IsFirstSave = false;
                }
                if (Cancel)
                    break;

                if (sleepMilliseconds > 0)
                {
                    PoolManager.CounterAdd(Consts.CounterTimeForSleep, sleepMilliseconds);
                    Thread.Sleep(sleepMilliseconds);
                }
                if (doSaveLastestTask!=null&&doSaveLastestTask.IsCompleted)
                    Console.WriteLine("已第{0}次成功向mongodb中插入变化的数据",count);
                long startData = stopWatch.ElapsedMilliseconds;
                doSaveLastestTask = GetLastData();
                if (stopWatch.ElapsedMilliseconds - preOutputCounterTime > intervalOutputCounterTime)
                {
                    // 获取计数器
                    counter = PoolManager.GetAndResetCounter();
                    // 输出
                    OutputCounter(counter);

                    // 自动优化内存，如果池中对象超过5000个，则只保留2000个
                    if (counter.ContainsKey(countKey) && counter[countKey] > 5000)
                    {
                        PoolManager.ReleasePool<ScadaData>(2000);
                    }
                }

                sleepMilliseconds = (int)(intervalModInvoke - (stopWatch.ElapsedMilliseconds - startData));
            }
        }
     
        public void Run()
        {
            start();
        }

        
    }
}
