using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opaalib.OAuth
{
    public class AuthenticationException : Exception
    {
        public AuthenticationException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
