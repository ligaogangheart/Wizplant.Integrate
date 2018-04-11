using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LinqToDB;
using DataModels;
using PR.WizPlant.Integrate.Sql;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using PR.WizPlant.Integrate.Scada.Entities;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading;

namespace PR.WizPlant.ScadaDataGeneral
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            conn = new SqlConnection(connectionString);
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            Closed += MainWindow_Closed;
        }

        void MainWindow_Closed(object sender, EventArgs e)
        {
            if (host != null)
            {
                host.Dispose();
            }
            if (selfHost != null)
            {
                selfHost.Close();
            }
        }

        ProjectHost host;
        ServiceHost selfHost;
        string connectionString = System.Configuration.ConfigurationManager.AppSettings["strConn"];
        string hostIpStr = System.Configuration.ConfigurationManager.AppSettings["hostIP"];
        string hostPortStr = System.Configuration.ConfigurationManager.AppSettings["hostPort"];

        SqlConnection conn = null;

        private readonly TaskScheduler _syncContextTaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();

        private void AddActionInfo(string info)
        {
            if (string.IsNullOrEmpty(info))
            {
                infoBox.AppendText("\n");
            }
            else
            {
                info = string.Format("***{0}***{1}",DateTime.Now.ToString(),info);
                infoBox.AppendText(info);
            }
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //异步取测点及测点值字典数据
            AddActionInfo("开始取测点及测点值字典\n");
            Task taskGetDict = new Task(getTagValueRuleList);

            Task task2 = taskGetDict.ContinueWith((t) =>{
                AddActionInfo("已取到测点及测点值字典\n");
                btnStart.IsEnabled = true;
            }, new CancellationToken(), TaskContinuationOptions.OnlyOnRanToCompletion, _syncContextTaskScheduler);

            Task task3 = taskGetDict.ContinueWith((t) =>
            {
                AddActionInfo("取测点及测点值字典失败\n");
                MessageBox.Show("取测点及测点值字典失败\n");
            }, new CancellationToken(), TaskContinuationOptions.OnlyOnFaulted, _syncContextTaskScheduler);

            taskGetDict.Start();
        }

        private void OpenServiceHost()
        {
            AddActionInfo("正在开启服务节点\n");
            selfHost = new ServiceHost(typeof(Service.VirtualScadaService));
            WebHttpBinding webBind = new WebHttpBinding();
            webBind.CrossDomainScriptAccessEnabled = true;

            var endPointBehavior = new WebHttpBehavior();

            string address = string.Format("http://{0}:{1}/VirtualScadaService/",hostIpStr,hostPortStr);
            var endPoint = selfHost.AddServiceEndpoint(typeof(Service.VirtualScadaService), webBind, address);
            endPoint.EndpointBehaviors.Add(endPointBehavior);
            selfHost.Open();
            AddActionInfo("开启服务节点成功\n");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Project project;;

            AddActionInfo("正在初始化数据\n");
            Task taskInitData=new Task(()=>{
                using (var db = new DataModels.TagValueDB())
                {
                    var projectQuery = from c in db.Project
                                       select c;

                    if (projectQuery.Count() == 0)
                    {
                        project = new Project() { ProjectName = "test",IsNeedInitData = true };
                        project.ProjectTagList = new List<ProjectTag>();
                        project.ProjectID = (long)db.InsertWithIdentity(project);
                        project.ProjectTagList = new List<ProjectTag>();
                        foreach (var rule in TagValueRuleManager.TagValueRuleDict)
                        {
                            var projectTag = new ProjectTag() { ProjectID = project.ProjectID, TagID = rule.Key };
                            db.Insert(projectTag);
                            project.ProjectTagList.Add(projectTag);
                        }
                    }
                    else
                    {
                        project = projectQuery.ToList()[0];
                        var projectTagQuery = from c in db.ProjectTag
                                              where c.ProjectID == project.ProjectID
                                              select c;
                        project.ProjectTagList = new List<ProjectTag>();
                        foreach (var projectTag in projectTagQuery)
                        {
                            project.ProjectTagList.Add(projectTag);
                        }
                    }
                }

                host = new ProjectHost(project);
                host.InitDataFinishedEvent += host_InitDataFinishedEvent;
                host.OpenCyclicChangeDataEvent += host_OpenCyclicChangeDataEvent;
                host.OnTagValueChangeEvent += host_OnTagValueChangeEvent;
                host.ExecuteInit();
                host.ExecuteGeneralData();
            });

            taskInitData.ContinueWith((t) => {
                OpenServiceHost();
                btnStop.IsEnabled = true;
                btnStart.IsEnabled = false;
            },new CancellationToken(),TaskContinuationOptions.OnlyOnRanToCompletion,_syncContextTaskScheduler);

            taskInitData.Start();
        }

        void host_OnTagValueChangeEvent(object sender, ProjectHost.TagValueChangeEventArg e)
        {
            ProjectHost.TagValueChangeEventArg data = e as ProjectHost.TagValueChangeEventArg;
            Task.Factory.StartNew(() =>
            {
                AddActionInfo(string.Format("测点{0}发生变化，新值：{1}，旧值：{2} \n",data.TagNo,data.NewValue,data.OldValue));
            }, new CancellationToken(), TaskCreationOptions.None, _syncContextTaskScheduler);
        }

        void host_OpenCyclicChangeDataEvent(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() => {
                AddActionInfo("开启循环变化数据任务\n");
            },new CancellationToken(),TaskCreationOptions.None,_syncContextTaskScheduler);
        }

        void host_InitDataFinishedEvent(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                AddActionInfo("初始化数据完成\n");
            }, new CancellationToken(), TaskCreationOptions.None, _syncContextTaskScheduler);
        }

        private void getTagValueRuleList()
        {
            string selectSql = @"select a.TagId,a.TagNo,b.IsNumeric,b.MaxValue,b.MinValue,b.MinChangeValue,
                                    c.MessureType,c.Name,c.SiteName,(case c.TagType when 'YX' then 1 else 2 end) ScadaDataType
                                 from PRW_Inte_VirtualTagConfig a
                                 inner join PRW_Inte_VirtualTagValueClass b on b.Id=a.ValueClassId
                                 inner join PRW_Inte_SCADA_Map c on c.TagNo=a.TagNo
                                 where a.isenable=1";
            List<TagValueRule> result = new List<TagValueRule>();
            try
            {
                using (var reader = SqlHelper.ExecuteDataReader(conn, CommandType.Text, selectSql, null))
                {
                    while (reader.Read())
                    {
                        var tagValueRule = new TagValueRule();
                        tagValueRule.TagId = reader[0].ToString().ToUpper();
                        tagValueRule.TagNo = reader[1].ToString();
                        tagValueRule.IsNumeric = (int)reader[2] == 1 ? true : false;
                        tagValueRule.MaxValue = (float)reader[3];
                        tagValueRule.MinValue = (float)reader[4];
                        tagValueRule.MinChangeValue = (float)reader[5];
                        tagValueRule.MessureType = (int)reader[6];
                        if ((object)reader[7] != System.DBNull.Value)
                        {
                            tagValueRule.Name = reader[7].ToString();
                        }
                        tagValueRule.SiteName = reader[8].ToString();
                        tagValueRule.Type = (ScadaDataType)reader[9];
                        result.Add(tagValueRule);
                    }
                }
                var dict = new Dictionary<string, TagValueRule>();
                foreach (var rule in result)
                {
                    if (dict.ContainsKey(rule.TagId))
                    {
                        continue;
                    }
                    dict.Add(rule.TagId, rule);
                }
                TagValueRuleManager.TagValueRuleDict = dict;
            }
            catch(Exception e)
            { 
            }
        }

        private void infoBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            infoBox.ScrollToEnd();
            TextRange textRange = new TextRange(infoBox.Document.ContentStart,infoBox.Document.ContentEnd);
            if (textRange.Text.Length > 10000)
            {
                infoBox.Document.Blocks.Clear();
            }
        }

        private void btnStop_Click_1(object sender, RoutedEventArgs e)
        {
            if (host != null)
            {
                host.Dispose();
            }
            if (selfHost != null)
            {
                selfHost.Close();
            }
            btnStop.IsEnabled = false;
            btnStart.IsEnabled = true;
        }
    }
}
