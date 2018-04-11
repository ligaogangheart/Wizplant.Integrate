using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR.WizPlant.Integrate.Scada
{
    
    public class CompletedEventArgs:EventArgs
    {
       
        public PackageKey PackageKey
        {
            get;
            private set;
        }

        public bool HasError
        {
            get;
            private set;
        }


        public CompletedEventArgs( PackageKey packageKey,bool hasError)
        {
            PackageKey = packageKey;
            HasError = hasError;
        }
    }
}
