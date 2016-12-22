using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoneraOMA.OAuth
{
    public class AuthenticatorConfiguration
    {
        public string BaseAddress { get; }
        public int ApiVersion { get; }
        public string CombinedAddress => $"{BaseAddress}/v{ApiVersion}";

        public AuthenticatorConfiguration(string baseAddress = "https://api.sonera.fi/autho4api", int apiVersion = 1)
        {
            BaseAddress = baseAddress;
            ApiVersion = apiVersion;
        }

        public static AuthenticatorConfiguration Default => new AuthenticatorConfiguration();
    }
}
