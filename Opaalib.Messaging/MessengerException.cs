using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opaalib.Messaging
{
    public class MessengerException : Exception
    {
        public string ResponseJson { get; }

        public MessengerException(string message, Exception innerException)
            : base(message, innerException)
        {
            
        }

        public MessengerException(string message, Exception innerException, string responseJson)
            : base(message, innerException)
        {
            ResponseJson = responseJson;
        }
    }
}
