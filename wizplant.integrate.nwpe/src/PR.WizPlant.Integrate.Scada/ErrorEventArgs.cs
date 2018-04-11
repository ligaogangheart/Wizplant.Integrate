using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR.WizPlant.Integrate.Scada
{
    
    public class ErrorEventArgs:EventArgs
    { 
        public Exception InnerException
        {
            get;
            private set;
        }



        public ErrorEventArgs(Exception exception)
        {
            InnerException = exception;
        }
    }
}
