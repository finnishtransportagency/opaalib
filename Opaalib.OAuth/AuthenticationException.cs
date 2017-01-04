using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opaalib.OAuth
{
    public class AuthenticationException : Exception
    {
        public string ResponseJson { get; }

        public AuthenticationException(string message, Exception innerException)
            : base(message, innerException)
        {

        }

        public AuthenticationException(string message, Exception innerException, string responseJson)
            : base(message, innerException)
        {
            ResponseJson = responseJson;
        }
    }
}
