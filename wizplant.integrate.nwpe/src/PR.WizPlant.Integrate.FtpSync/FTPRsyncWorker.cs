using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;

namespace PR.WizPlant.Integrate.FtpSync
{
    public class FTPRsyncWorker
    {
        public void Run()
        {
            NameValueCollection configSection = (NameValueCollection)ConfigurationManager.GetSection("ftpsync");
            if (configSection == null)
            {
                Console.WriteLine("没有配置要同步的文件");
                return;
            }
            Dictionary<string, string> map = new Dictionary<string, string>();

            int iLen = configSection.Count;
            for (int i = 0;i < iLen;i++)
            {
                map.Add(configSection.GetKey(i), configSection[i]);
            }

            Console.WriteLine("FTPRsyncWorker.Run()");

            string userName = ConfigurationManager.AppSettings["ftpuser"];
            string password = ConfigurationManager.AppSettings["ftppwd"];
            string ftppath = ConfigurationManager.AppSettings["ftpsyncpath"];
            string localpath = ConfigurationManager.AppSettings["localpath"];
            string jsfile = ConfigurationManager.AppSettings["jsfile"];
            string jsfilename = Path.GetFileName(jsfile);

            Console.WriteLine("FTPHelper({0},{1})", userName, password);
            FTPHelper ftp = new FTPHelper(userName, password);
            Console.WriteLine("ListFiles({0})", ftppath);
            bool succ = true;
            List<FileStruct> files = null;
            try
            {
                files = ftp.ListFiles(ftppath);
            }
            catch (Exception ex)
            {
                succ = false;
                Console.WriteLine("error={0}", ex.Message);
                Console.WriteLine("error={0}", ex.StackTrace);
            }
            if (!succ)
            {
                return;
            }

            string targetFile = null;
            string ftpFleUrl = null;
                Console.WriteLine("files.count={0}", files.Count);
                foreach (var file in files)
                {
                    if (map.ContainsKey(file.Name))
                    { 
                        Console.WriteLine("sync filename={0}", file.Name);
                        targetFile = Path.Combine(localpath, map[file.Name]);
                        if (File.Exists(targetFile) && File.GetCreationTime(targetFile).ToFileTimeUtc() == file.CreateTime.ToFileTimeUtc())
                        {
                            Console.WriteLine("file create time is same, skip sync");
                            break;
                        }

                        string targetPath = Path.GetDirectoryName(targetFile);
                        Console.WriteLine("target Path:{0}", targetPath);
                        if (Directory.Exists(targetPath))
                        {

                        }

                        ftpFleUrl = string.Format("{0}/{1}", ftppath.TrimEnd('/', '\\'), System.Web.HttpUtility.UrlEncode(file.Name,Encoding.Default));
                        try
                        {
                            ftp.DownLoadFile(ftpFleUrl, targetFile);
                            Console.WriteLine("file sync success");
                            succ = true;
                        }
                        catch (Exception ex)
                        {
                            succ = false;
                            Console.WriteLine("file sync failed:{0}",ex.Message);                            
                        }

                        if (succ)
                        {
                            if (File.Exists(jsfile))
                            {
                                Console.WriteLine("jsfile 不存在:{0}", jsfile);
                                targetFile = Path.Combine(Path.GetDirectoryName(targetFile), jsfilename);
                                if (!File.Exists(targetFile))
                                {
                                    File.Copy(jsfile, targetFile);
                                    Console.WriteLine("Copy {0} to {1}", jsfile, targetFile);
                                }
                            }
                        }
                    }
                    
                }
           


        }
    }
}
