
using Common.Logging;
using PR.WizPlant.Integrate.Scada;
using PR.WizPlant.Integrate.Scada.Entities;
using PR.WizPlant.Integrate.Sql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR.WizPlant.ScadaTool
{
    public class TcpGetMetaDataWorker : IWorker
    {
        Dictionary<string, string> tagDescs = null;
        /// <summary>
        /// Scada报文客户端
        /// </summary>
        private ScadaClient client = null;

        /// <summary>
        /// 需要集成的厂站名称
        /// </summary>
        string[] inteSites = null;

        string connectionString = System.Configuration.ConfigurationManager.AppSettings["strConn"];

        #region Service
        /// <summary>
        /// Scada服务
        /// </summary>
        IScadaService<ScadaData> scadaService = null;
        #endregion


        ILog logger = LogManager.GetLogger<TcpGetMetaDataWorker>();

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
        /// 构造方法
        /// </summary>
        public TcpGetMetaDataWorker()
        {
            client = new ScadaClient();
            client.DataRecieved += client_DataRecieved;
            client.DisConnected += client_DisConnected;


            scadaService = new MongDbScadaService();

            string val = System.Configuration.ConfigurationManager.AppSettings["InteSites"];
            if (!string.IsNullOrEmpty(val))
            {
                inteSites = val.Split(',');
            }
           

            var smpWorker = new ScadaModelParseWorker();
            smpWorker.Run();
            tagDescs = smpWorker.TagDescs;
        }

        


        /// <summary>
        /// 启动
        /// </summary>
        public void Run()
        {  
            // 启动报文数据接收解析
            start();

            string selectSql = "select TagNo from PRW_Inte_SCADA_Map";
            if (inteSites != null)
            {
                selectSql += " where SiteName in ('" + string.Join("','", inteSites) + "')";
            }

            List<string> existsTagNos = new List<string>();
            using (var reader = SqlHelper.ExecuteDataReader(connectionString, selectSql, null))
            {
                while (reader.Read())
                {
                    existsTagNos.Add(reader.GetString(0));
                }
            }

            StringBuilder sqlBuilder = new StringBuilder();

            // 此段用来提取设备测点数据字典
            //StreamWriter sw = new StreamWriter(metaDataFile, false, Encoding.UTF8);
            //sw.WriteLine("TagNo,SiteName,Name,Type,MessureType");

            int insertCount = 0;
            int updateCount = 0;
            if (scadaList.Count > 0)
            {
                logger.DebugFormat("scadaList.Count={0}", scadaList.Count);
                string desc = null;
                foreach (var item in scadaList)
                {
                    desc = null;
                    tagDescs.TryGetValue(item.TagNo, out desc);

                    if (desc == null)
                    {
                        logger.DebugFormat("测点[{0}_{1}_{2}] 在模型文件中没有找到对应的描述", item.TagNo,item.Name,item.Type);
                    }

                    if (!existsTagNos.Contains(item.TagNo))
                    {
                        insertCount++;
                        sqlBuilder.AppendFormat("insert into PRW_Inte_SCADA_Map(Id,SiteName,TagNo,TagName,TagType,MessureType,TagDesc) values(newid(),'{0}','{1}','{2}','{3}',{4},{5});",
                            item.SiteName,
                            item.TagNo,
                            item.Name,
                            item.Type,
                            item.MessureType,
                            desc == null ? "NULL" : string.Format("'{0}'", desc));
                        sqlBuilder.AppendLine();
                    }
                    else
                    {
                        updateCount++;
                        sqlBuilder.AppendFormat("update PRW_Inte_SCADA_Map set SiteName='{1}',TagName='{2}',TagType='{3}',MessureType={4},TagDesc={5} where TagNo='{0}';",
                             item.TagNo, 
                             item.SiteName,                           
                            item.Name,
                            item.Type,
                            item.MessureType,
                            desc == null ? "TagDesc" : string.Format("'{0}'", desc));
                        sqlBuilder.AppendLine();
                    }

                    if ((updateCount + insertCount) % 2000 == 0)
                    {
                        string updateSql = sqlBuilder.ToString();
                        sqlBuilder.Clear();
                       // logger.DebugFormat("updateSql = {0}", updateSql);
                        DateTime startTime = DateTime.Now;
                        int result = SqlHelper.ExecuteNonQuery(connectionString, updateSql, null);
                        logger.DebugFormat("本次保存耗时{0}ms,影响的记录数:{1}", DateTime.Now.Subtract(startTime).TotalMilliseconds, result);
                        
                    }
                    //sw.WriteLine("{0},{1},{2},{3},{4}", item.TagNo, item.SiteName, item.Name, item.Type,item.MessureType);
                }

                if (sqlBuilder.Length > 0)
                {
                    string updateSql = sqlBuilder.ToString();
                   // logger.DebugFormat("updateSql = {0}", updateSql);
                    DateTime startTime = DateTime.Now;
                    int result = SqlHelper.ExecuteNonQuery(connectionString, updateSql, null);
                    logger.DebugFormat("本次保存耗时{0}ms,影响的记录数:{1}", DateTime.Now.Subtract(startTime).TotalMilliseconds, result);
                }
            }

            logger.DebugFormat("本次新增了{0}个测点信息,修改了{1}个测点信息", insertCount,updateCount);
          
            Console.WriteLine("完成");
        }



        /// <summary>
        /// 开始
        /// </summary>
        void start()
        {
            Console.WriteLine("Begin HandShake");
            // 与服务器握手
            bool succ = client.HandShake();
            Console.WriteLine("HandShake Result:{0}", succ);

            //logger.Debug("开启异步保存任务");
            //doSaveTask = DoSave();
            //doSaveTask.Start();

            logger.Debug("开始全数据处理");

            // 先取一次全数据
            allData();

            logger.DebugFormat("全数据处理完毕");



        }

        /// <summary>
        /// 处理全数据
        /// </summary>
        void allData()
        {

            PackageKey key = PackageKey.ALLYX;
            Console.WriteLine("Begin {0}", key);
            var succ = client.Do(key);
            Console.WriteLine("End {0}", key);



            key = PackageKey.ALLYC;
            Console.WriteLine("Begin {0}", key);
            succ = client.Do(key);
            Console.WriteLine("End {0}", key);
        }


        void client_DisConnected(object sender, DisConnectedEventArgs e)
        {
            // throw new NotImplementedException();
        }



        /// <summary>
        /// 待处理列表
        /// </summary>
        List<ScadaData> scadaList = new List<ScadaData>();





        /// <summary>
        /// 收到数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void client_DataRecieved(object sender, ScadaDataEventArgs e)
        {

            Task task = null;
            switch (e.PackageKey)
            {
                case PackageKey.ALLYX:
                    // 异步执行解析和持久操作，让网络不阻塞
                    task = Task.Run(() =>
                    {
                        var scadaData = new ScadaData();

                        // 解析数据
                        bool succ = BufferHelper.ParseYXData(e.Data, scadaData);

                        if (succ)
                        {
                            bool needSave = true;
                            if (inteSites != null)
                            {
                                if (!inteSites.Contains(scadaData.SiteName))
                                {
                                    needSave = false;
                                }
                            }
                            if (needSave)
                            {
                                scadaList.Add(scadaData);
                            }
                        }



                    });

                    task.Wait();

                    break;
                case PackageKey.ALLYC:

                    // 异步执行解析和持久操作，让网络不阻塞
                    task = Task.Run(() =>
                    {
                        var scadaData = new ScadaData();

                        // 解析数据
                        bool succ = BufferHelper.ParseYCData(e.Data, scadaData);
                        if (succ)
                        {
                            bool needSave = true;
                            if (inteSites != null)
                            {
                                if (!inteSites.Contains(scadaData.SiteName))
                                {
                                    needSave = false;
                                }
                            }
                            if (needSave)
                            {
                                scadaList.Add(scadaData);
                            }
                        }

                    });
                    task.Wait();
                    break;
            }
        }
    }
}
