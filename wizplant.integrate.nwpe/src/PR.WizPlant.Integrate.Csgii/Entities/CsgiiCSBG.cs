using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR.WizPlant.Integrate.Csgii.Entities
{
    public partial class CsgiiCSBG
    {
        public Guid Id { get; set; }
        public string DeviceId { get; set; }
        public string CSBGMC { get; set; }
        public DateTime RQ { get; set; }
        public string ZY { get; set; }
        public string XZ { get; set; }
        public string JL { get; set; }
    }
}
