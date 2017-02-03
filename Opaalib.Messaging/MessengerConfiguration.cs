using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opaalib.Messaging
{
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
