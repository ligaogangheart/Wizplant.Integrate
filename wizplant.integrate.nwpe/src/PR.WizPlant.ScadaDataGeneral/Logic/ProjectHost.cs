using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinqToDB;
using System.Threading;

namespace PR.WizPlant.ScadaDataGeneral
{
    /// <summary>
    /// 数据生成器项目主机
    /// </summary>
    public class ProjectHost : DiposableClass,IProjectHost
    {
        #region nested class

        public class TagValueChangeEventArg
        {
            public string TagNo { get; set; }
            public float NewValue { get; set; }
            public float OldValue { get; set; }
        }

        #endregion

        private readonly Project _project;
        private GeneralDataEngineer _generalDataEngineer = new GeneralDataEngineer();
        private ChooseTagEngineer _chooseTagEngineer = new ChooseTagEngineer();
        private CancellationTokenSource _tokenSource;
        private Task _cyclicGeneralDataTask;
        private List<string> _curGeneralDataTagList;

        //初始化数据完成事件
        public event EventHandler InitDataFinishedEvent;
        public event EventHandler OpenCyclicChangeDataEvent;

        public event EventHandler<TagValueChangeEventArg> OnTagValueChangeEvent;

        public ProjectHost(Project project)
        {
            _project = project;
        }

        public bool ExecuteInit()
        {
           bool result = true;
            if (_project.IsNeedInitData)
            {
                try
                {
                    InitTagData();
                }
                catch(Exception e)
                {
                    result = false;
                }
            }

            if (InitDataFinishedEvent != null)
            {
                InitDataFinishedEvent.Invoke(this, null);
            }
            return result;
        }

        //开启循环随机变化测点数据任务
        public void ExecuteGeneralData()
        {
            _tokenSource = new CancellationTokenSource();
            _cyclicGeneralDataTask = new Task(CyclicGeneralDataTask, _tokenSource.Token);
            _cyclicGeneralDataTask.Start();
            if (OpenCyclicChangeDataEvent != null)
            {
                OpenCyclicChangeDataEvent.Invoke(this, null);
            }
        }

        //循环变化测点数据
        public void CyclicGeneralDataTask()
        {
            List<ProjectTagValue> tagValueList;
            while (true)
            {
                if (_tokenSource.IsCancellationRequested)
                {
                    throw new OperationCanceledException(_tokenSource.Token);
                }

                try
                {
                    _curGeneralDataTagList = _chooseTagEngineer.ExecuteChooseTagList(_project.ProjectTagList.Select(x => x.TagID).ToList(), _project.NumOneWork);


                    using (var db = new DataModels.TagValueDB())
                    {
                        var tagListQuery = from a in db.ProjectTagValue
                                           where a.ProjectID == _project.ProjectID && _curGeneralDataTagList.Contains(a.TagId)
                                           select a;
                        tagValueList = tagListQuery.ToList();
                        float oldValue;
                        foreach (var tagValue in tagValueList)
                        {
                            oldValue = tagValue.Value;
                            tagValue.Value = _generalDataEngineer.ExecuteGeneralData(tagValue.TagId, oldValue);
                            tagValue.TimeStamp = DateTime.Now;
                            db.UpdateAsync(tagValue);
                            if (OnTagValueChangeEvent != null)
                            {
                                OnTagValueChangeEvent.BeginInvoke(this, new TagValueChangeEventArg() { TagNo = tagValue.TagNo, NewValue = tagValue.Value, OldValue = oldValue },null,null);
                            }
                        }
                    }
                }
                catch (Exception e)
                { 
                }
                
                Thread.Sleep(500);
            }
        }

        //产生初始化数据
        private void InitTagData()
        {
            List<ProjectTagValue> tagValueList = new List<ProjectTagValue>();
            foreach (var tag in _project.ProjectTagList)
            {
                var curTagRule = TagValueRuleManager.TagValueRuleDict[tag.TagID];
                float newValue = _generalDataEngineer.ExecuteGeneralData(tag.TagID, curTagRule.MinValue);
                ProjectTagValue tagValue = new ProjectTagValue() {TagId=curTagRule.TagId ,TagNo=curTagRule.TagNo,MessureType=curTagRule.MessureType,Name=curTagRule.Name,
                        SiteName=curTagRule.SiteName,Type=curTagRule.Type,TimeStamp=DateTime.Now,ProjectID=_project.ProjectID,Value=newValue
                    };
                tagValueList.Add(tagValue);
            }

            using (var db = new DataModels.TagValueDB())
            {
                foreach (var tagValue in tagValueList)
                {
                    db.InsertAsync<ProjectTagValue>(tagValue);
                }
            }
        }

        #region dispose

        private bool _disposed;

        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_tokenSource != null)
                    {
                        _tokenSource.Cancel();
                    }
                }
                _disposed = true;
            }
            base.Dispose(disposing);
        }

        #endregion
    }
}
