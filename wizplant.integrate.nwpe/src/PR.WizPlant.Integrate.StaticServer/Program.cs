using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR.WizPlant.Integrate.StaticServer
{
    class Program
    {
        static void Main(string[] args)
        {
            MonthWorker worker = new MonthWorker();
            worker.Run();
            Console.Read();
        }
    }
}
