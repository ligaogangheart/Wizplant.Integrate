using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR.WizPlant.Integrate.Csgii.Entities
{
    public partial class CsgiiZTPJ
    {
        public Guid Id { get; set; }
        public string DeviceId { get; set; }
        public DateTime PJRQ { get; set; }
        public string SBJKD { get; set; }
        public string SBZYD { get; set; }
        public string SBYWJB { get; set; }        
    }
}
