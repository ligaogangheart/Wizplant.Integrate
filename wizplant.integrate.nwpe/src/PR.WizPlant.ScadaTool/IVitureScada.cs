using PR.WizPlant.Integrate.Scada.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR.WizPlant.ScadaTool
{
   public interface IVitureScada
    {
       Task GetAllData();


       Task GetLastData();
    }
}
