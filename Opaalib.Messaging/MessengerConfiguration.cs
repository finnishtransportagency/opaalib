using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opaalib.Messaging
{
    public class MessengerConfiguration
    {
        public string RegistrationId { get; }
        public string SenderAddress { get; }
        public string BaseAddress { get; }
        public int ApiVersion { get; }
        public string CombinedAddress => $"{BaseAddress}/v{ApiVersion}";

        public MessengerConfiguration(string registrationId, string senderAddress, string baseAddress = "https://api.sonera.fi/production/messaging", int apiVersion = 1)
        {
            RegistrationId = registrationId;
            SenderAddress = senderAddress;
            BaseAddress = baseAddress;
            ApiVersion = apiVersion;
        }
    }
}
