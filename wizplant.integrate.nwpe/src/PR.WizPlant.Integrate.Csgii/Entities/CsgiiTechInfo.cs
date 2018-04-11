using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR.WizPlant.Integrate.Csgii.Entities
{
    public partial class CsgiiTechInfo
    {
        public Guid Id { get; set; }
        public string DeviceId { get; set; }
        public string ClassifyId { get; set; }
        public string TechParamId { get; set; }
        public string ColumnName { get; set; }
        public string TechParamName { get; set; }
        public string TechParamValue { get; set; }
        public string Units { get; set; }
        public bool? IsMandatory { get; set; }
        public bool? IsShow { get; set; }
        public int? SortNo { get; set; }
        public int DataType { get; set; }
        public bool? IsVendorFill { get; set; }
    }
}
