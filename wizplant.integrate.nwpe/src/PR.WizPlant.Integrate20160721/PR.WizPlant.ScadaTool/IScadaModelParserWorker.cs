using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR.WizPlant.ScadaTool
{
    public interface IScadaModelParserWorker:IWorker
    {
        Dictionary<string, string> TagDescs { get; }
    }
}
