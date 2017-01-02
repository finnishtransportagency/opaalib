using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opaalib.Messaging
{
    public class MessengerException : Exception
    {
        public MessengerException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
