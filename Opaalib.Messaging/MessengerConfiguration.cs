using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opaalib.Messaging
{
    public class MessengerConfiguration
    {
        // TODO: Remove this and pass as parameter?
        public string SenderAddress { get; }
        public string BaseAddress { get; }

        public MessengerConfiguration(string senderAddress, string baseAddress = "https://api.sonera.fi/sandbox/messaging/v1")
        {
            SenderAddress = senderAddress;
            BaseAddress = baseAddress;
        }
    }
}
