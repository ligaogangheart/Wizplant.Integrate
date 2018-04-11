using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinqToDB;
using LinqToDB.Mapping;

namespace PR.WizPlant.ScadaDataGeneral
{
    /// <summary>
    /// 项目测点集
    /// </summary>
    [Table("ProjectTag")]
    public partial class ProjectTag
    {
        [PrimaryKey, Identity]
        public long ProjectTagID { get; set; }
        [Column, NotNull]
        public long ProjectID { get; set; }
        [Column, NotNull]
        public string TagID { get; set; }
    }
}
