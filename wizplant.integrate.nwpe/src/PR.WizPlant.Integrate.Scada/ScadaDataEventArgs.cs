using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR.WizPlant.Integrate.Scada
{
    
    public class ScadaDataEventArgs:EventArgs
    {
        public byte[] Data
        {
            get;
            private set;
        }
        public PackageKey PackageKey
        {
            get;
            private set;
        }



        public ScadaDataEventArgs(byte[] data, PackageKey packageKey)
        {
            Data = data;
            PackageKey = packageKey;
        }
    }
}
