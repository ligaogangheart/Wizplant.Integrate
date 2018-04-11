using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR.WizPlant.Integrate.Scada
{
    
    public class DisConnectedEventArgs:EventArgs
    {

        public DisConnectedReason Reason
        {
            get;
            private set;
        }

        public Exception InnerException
        {
            get;
            private set;
        }

        public string Message
        {
            get;
            private set;
        }

        public DisConnectedEventArgs(DisConnectedReason reason)
        {
            Reason = reason;           
        }

        public DisConnectedEventArgs(DisConnectedReason reason, string message)
        {
            Reason = reason;
            Message = message;
        }

        public DisConnectedEventArgs(DisConnectedReason reason, string message, Exception exception)
        {
            Reason = reason;
            Message = message;
            InnerException = exception;
        }
    }
}
