using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoneraOMA.Messaging
{
    public class InboundMessageNotificationContainer
    {
        [JsonProperty(PropertyName = "inboundMessageNotification", Required = Required.Always)]
        public InboundMessageNotification InboundMessageNotification { get; set; }
    }

    public class InboundMessageNotification
    {
        [JsonProperty(PropertyName = "inboundMessage", Required = Required.Always)]
        public InboundMessage InboundMessage { get; set; }
    }
}
