using PR.WizPlant.Integrate.Scada.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace PR.WizPlant.Integrate.Scada
{
    public class MongDbCurrentYCScadaService : MongDbYCScadaService
    {  
        protected override string Name
        {
            get { return "CurrentYCData"; }
        }        
    }
}
