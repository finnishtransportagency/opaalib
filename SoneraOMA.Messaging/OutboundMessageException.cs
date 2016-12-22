using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoneraOMA.Messaging
{
    public class OutboundMessageException : Exception
    {
        public OutboundMessageException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
