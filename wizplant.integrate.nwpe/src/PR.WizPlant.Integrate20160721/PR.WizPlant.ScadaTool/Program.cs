using Common.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PR.WizPlant.ScadaTool
{
    class Program
    {
        // logger ....
        private static ILog logger = LogManager.GetLogger<Program>();
        static void Main(string[] args)
        {
            bool needSelectMode = true;
            string command = null;

            if (args!=null && args.Length>0)
            {
                command = args[0];
                if (command == "1" || command == "2" || command == "3")
                {
                    needSelectMode = false;
                }
            }

            if (needSelectMode)
            {
                Console.WriteLine("###################################");
                Console.WriteLine("运行模式:");
                Console.WriteLine("___________________________________");
                Console.WriteLine("1. 接收Scada报文");
                Console.WriteLine("2. 接收Scada测点表");
                Console.WriteLine("3. 更新Scada对照表");
                Console.WriteLine("0. 退出");
                Console.WriteLine("___________________________________");

                Console.Write("请选择运行模式[1]:");
                command = Console.ReadLine();
                while (true)
                {
                    if (command == "")
                    {
                        command = "1";
                    }
                    if (command == "0")
                    {
                        return;
                    }
                    if (command == "1" || command == "2" || command == "3")
                    {
                        break;
                    }
                    Console.Write("请选择运行模式[1]:");
                    command = Console.ReadLine();
                }
            }

            IWorker worker = null;

            switch (command)
            {
                case "1":
                    worker = new TcpScanWorker();
                    break;
                case "2":
                    worker = new TcpGetMetaDataWorker();
                    break;
                case "3":
                    worker = new ScadaMetaDataInitWorker();
                    break;
            } 
            worker.Run();

            logger.InfoFormat("任务完成，5秒后退出");
            Console.WriteLine("任务结束，5秒后退出");
            Thread.Sleep(5000);
          //  Console.Read();
            return;

            //Stopwatch sw = new Stopwatch();
            //sw.Start();

            int interval = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["interval"]);

            worker = new ScanWorker();
            while (true)
            {
                try
                {
                    //ScalaEFileHelper.ScanEFileToDb(@"D:\svn\PR.WizPlant.Integrate\trunk\doc\data\普洱_20160416_182100_SCADA.DT", DateTime.Now);
                    //Console.WriteLine("done,cost:{0}ms", sw.ElapsedMilliseconds);
                    worker.Run();
                }
                catch (Exception ex)
                {
                    //log
                    logger.ErrorFormat("错误:{0} 扫描程序已终止，待下次扫描", ex.Message + "|" + ex.TargetSite.ToString());
                    Console.WriteLine("{0:HH:mm:ss}  错误:{1} 扫描程序已终止，待下次扫描", DateTime.Now, ex.Message + "|" + ex.TargetSite.ToString());

                }
                finally
                {
                    string message = string.Format("{0:HH:mm:ss}  下一次扫描启动时间:{1:yyyy-MM-hh HH:mm:ss}", DateTime.Now, DateTime.Now.AddMilliseconds(interval));
                    logger.Debug(message);
                    Console.WriteLine(message);
                    Thread.Sleep(interval);// from config 

                    Console.WriteLine();
                }
            }
        }

       
    }
}
