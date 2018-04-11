using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace PR.WizPlant.Integrate.FtpSync
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>      
        static void Main()
        {
            FTPRsyncWorker worker = new FTPRsyncWorker();
            worker.Run();
            Console.WriteLine("任务完成");
            Console.ReadLine();
        }
    }
}
