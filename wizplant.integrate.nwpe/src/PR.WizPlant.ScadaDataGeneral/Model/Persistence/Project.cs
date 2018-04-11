using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinqToDB;
using LinqToDB.Mapping;
using System.Configuration;

namespace PR.WizPlant.ScadaDataGeneral
{
    /// <summary>
    /// 产生数据项目
    /// </summary>
    [Table("Project")]
    public partial class Project
    {
        [PrimaryKey, Identity]
        public long ProjectID { get; set; }

        [Column, Nullable]
        public string ProjectName { get; set; }

        public bool IsNeedInitData { get; set; }

        public List<ProjectTag> ProjectTagList { get; set; }

        public int NumOneWork {
            get
            {
                return int.Parse(System.Configuration.ConfigurationManager.AppSettings["NumOneWork"]); 
            }
        }

        public Project()
        {
        }
    }
}
