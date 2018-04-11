using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR.WizPlant.DataImport
{
    public class LogHelper
    {
        /// <summary>
        /// 写错误日志
        /// </summary>
        /// <param name="message"></param>
        public static void WriteErrorLog(string message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "Logs\\ErrorLogs.txt";
            WriteFile(path, "错误信息：" + message);
        }

        /// <summary>
        /// 写错误日志
        /// </summary>
        /// <param name="ex"></param>
        public static void WriteErrorLog(Exception ex)
        {
            string message = string.Empty;
            if (ex.InnerException != null)
            {
                message = ex.InnerException.Message;

            }
            else
            {
                message = ex.Message;
            }
            if (ex.StackTrace != null)
            {
                message += ex.StackTrace;
            }
            WriteErrorLog(message);
        }

        /// <summary>
        /// 写错误日志
        /// </summary>
        /// <param name="ex"></param>
        public static void WriteErrorLog(Exception ex, string messageout)
        {

            string message = string.Empty;

            if (ex.InnerException != null)
            {
                message = ex.InnerException.Message;

            }
            else
            {
                message = ex.Message;
            }
            message += messageout;
            if (ex.StackTrace != null)
            {
                message += ex.StackTrace;
            }
            WriteErrorLog(message);
        }

        /// <summary>
        /// 写操作日志
        /// </summary>
        /// <param name="message"></param>
        public static void WriteOperateLog(string message)
        {
            bool isNeedOperateLog = false;
            string value = System.Configuration.ConfigurationManager.AppSettings["NeedOperateLog"];
            if (!string.IsNullOrEmpty(value))
            {
                isNeedOperateLog = bool.Parse(value);
            }
            if (isNeedOperateLog)
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "Logs\\OperateLogs.txt";
                WriteFile(path, "操作信息：" + message);
            }
        }


        private static void WriteFile(string path, string message)
        {
            try
            {
                StreamWriter write = null;
                if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "Logs"))
                {
                    Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "Logs");
                }
                if (!File.Exists(path))
                {
                    FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    write = new StreamWriter(fs);
                }
                else
                {
                    write = File.AppendText(path);
                }
                write.WriteLine(DateTime.Now + message);
                write.Flush();
                write.Close();
            }
            catch (Exception ex)
            {


            }

        }
    }
}
