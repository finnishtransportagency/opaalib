using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opaalib.OAuth
{
    public class AuthenticatorConfiguration
    {
        public string BaseAddress { get; }

        public AuthenticatorConfiguration(string baseAddress = "https://api.sonera.fi/autho4api/v1")
        {
            BaseAddress = baseAddress;
        }

        public static AuthenticatorConfiguration Default => new AuthenticatorConfiguration();
    }
}
