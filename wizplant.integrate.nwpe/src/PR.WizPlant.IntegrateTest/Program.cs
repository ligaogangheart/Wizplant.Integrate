using Common.Logging;
using PR.WizPlant.Integrate.Csgii.SBTZ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR.WizPlannt.IntegrateTest
{
    class Program
    {
        static ILog logger = LogManager.GetLogger("IntegrateTest");
        static void Main(string[] args)
        {
            Console.WriteLine("开始执行......");
            var isSoa = System.Configuration.ConfigurationManager.AppSettings["isSoa"];
            IWorker worker = null;
            //if (isSoa == "true")
            //{
            //    worker = new TestSoaWorker();
            //}
            //else
            //{
            //    worker = new TestWorker();
            //}
            logger.Info("开始执行......");
            worker = new MongoDbTest();

            worker.Run();
            //worker.GenJsonMethod(typeof(deviceGisDTO));
            Console.WriteLine("执行完成，按回车键退出");
            Console.ReadLine();
        }
    }
}
