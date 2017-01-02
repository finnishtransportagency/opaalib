using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opaalib.Messaging
{
    public class DeliveryInfoNotificationContainer
    {
        [JsonProperty(PropertyName = "deliveryInfoNotification", Required = Required.Always)]
        public DeliveryInfoNotification DeliveryInfoNotification { get; set; }
    }

    public class DeliveryInfoNotification
    {
        [JsonProperty(PropertyName = "deliveryInfo", Required = Required.Always)]
        public DeliveryInfo[] DeliveryInfo { get; set; }

        [JsonProperty(PropertyName = "callbackData")]
        public string CallbackData { get; set; }
    }
}
