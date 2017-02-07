using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opaalib.Messaging
{
    /// <summary>
    /// Uses sandbox api instead of production by default
    /// </summary>
    public class MessengerConfiguration
    {
        public string BaseAddress { get; }

        public MessengerConfiguration(string baseAddress = "https://api.sonera.fi/sandbox/messaging/v1")
        {
            BaseAddress = baseAddress;
        }

        public static MessengerConfiguration Default => new MessengerConfiguration();
    }
}
