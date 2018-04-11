using MongoDB.Driver;
using PR.WizPlant.Integrate.Scada.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR.WizPlant.Integrate.Scada
{
    public class MongDbCurrentYXScadaService : MongDbYXScadaService
    { 
        protected override string Name
        {
            get { return "CurrentYXData"; }
        }      
    }
}
