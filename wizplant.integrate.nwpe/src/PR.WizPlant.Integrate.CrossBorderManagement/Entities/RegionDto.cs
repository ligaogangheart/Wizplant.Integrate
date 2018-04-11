using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR.WizPlant.Integrate.CrossBorderManagement
{
    public class RegionDto
    {
        public string RegionId { get; set; }
        public string RegionName { get; set; }
        public string SiteId { get; set; }
        public List<BorderPointDto> BorderPointList { get; set; }
    }
}
