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
        /// <summary>
        /// Contains delivery information. One instance for each
        /// destination address.
        /// </summary>
        [JsonProperty(PropertyName = "deliveryInfo", Required = Required.Always)]
        public DeliveryInfo[] DeliveryInfo { get; set; }

        /// <summary>
        /// CallbackData for the messaging session, matching the
        /// one in the outbound message request call, if this was
        /// provided.
        /// </summary>
        [JsonProperty(PropertyName = "callbackData")]
        public string CallbackData { get; set; }
    }
}
