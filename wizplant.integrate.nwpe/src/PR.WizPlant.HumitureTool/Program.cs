using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR.WizPlant.HumitureTool
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("开始执行......");
            ScanWorker worker = new ScanWorker();
            worker.Run();
            //worker.GenJsonMethod(typeof(deviceGisDTO));
            Console.WriteLine("执行完成，按回车键退出");
            Console.ReadLine();
        }
    }
}
