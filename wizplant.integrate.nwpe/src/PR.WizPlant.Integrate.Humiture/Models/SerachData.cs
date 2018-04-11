using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR.WizPlant.Integrate.Humiture.Models
{
  public  class SerachData
    {
        public string pageNo { get; set; }
        public string pageSize { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public string token { get; set; }
    }
}
