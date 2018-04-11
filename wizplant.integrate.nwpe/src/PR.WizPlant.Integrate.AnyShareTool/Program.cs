using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PR.WizPlant.Integrate.AnyShareTool
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("开始执行......");
            CreatWorker worker = new CreatWorker();
            worker.Run();
            Console.WriteLine("执行完成，按回车键退出");
            Console.ReadLine();
        }
    }
}
