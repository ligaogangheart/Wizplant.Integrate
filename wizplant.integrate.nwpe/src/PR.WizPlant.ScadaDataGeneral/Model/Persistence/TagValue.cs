using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinqToDB;
using LinqToDB.Mapping;
using PR.WizPlant.Integrate.Scada.Entities;

namespace PR.WizPlant.ScadaDataGeneral
{
    /// <summary>
    /// 项目测点值
    /// </summary>
    [Table("ProjectTagValue")]
    public partial class ProjectTagValue
    {
        [PrimaryKey, Identity]
        public long ProjectTagID { get; set; }
        [Column, Nullable]
        public string TagId { get; set; }
        [Column, Nullable]
        public string TagNo { get; set; }
        [Column, Nullable]
        public int MessureType { get; set; }
        [Column, Nullable]
        public string Name { get; set; }
        [Column, Nullable]
        public string SiteName { get; set; }
        [Column, Nullable]
        public ScadaDataType Type { get; set; }
        [Column, Nullable]
        public float Value { get; set; }
        [Column, Nullable]
        public DateTime TimeStamp { get; set; }
        [Column, NotNull]
        public long ProjectID { get; set; }
    }
}
