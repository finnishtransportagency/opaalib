using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opaalib.Messaging
{
    public class DeliveryInfoListContainer
    {
        [JsonProperty(PropertyName = "deliveryInfoList", Required = Required.Always)]
        public DeliveryInfoList DeliveryInfoList { get; set; }
    }

    public class DeliveryInfoList
    {
        /// <summary>
        /// URI of the created message, as the reference to this 
        /// response.
        /// </summary>
        [JsonProperty(PropertyName = "resourceURL", Required = Required.Always)]
        public string ResourceUrl { get; set; }

        /// <summary>
        /// Contains delivery information.
        /// </summary>
        [JsonProperty(PropertyName = "deliveryInfo", Required = Required.Always)]
        public DeliveryInfo[] DeliveryInfo { get; set; }
    }

    public class DeliveryInfo
    {
        /// <summary>
        /// Outbound message destination address. 
        /// </summary>
        [JsonProperty(PropertyName = "address", Required = Required.Always)]
        public string Address { get; set; }

        // TODO: Add conversion
        /// <summary>
        /// Contains the delivery result for the destination address.
        /// </summary>
        [JsonProperty(PropertyName = "deliveryStatus", Required = Required.Always)]
        public DeliveryStatus DeliveryStatus { get; set; }

        /// <summary>
        /// Used together with DeliveryStatus to provide additional
        /// information.
        /// </summary>
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }
    }

    public enum DeliveryStatus
    {
        /// <summary>
        /// Successful delivery to the terminal.
        /// </summary>
        DeliveredToTerminal,
        /// <summary>
        /// Delivery status unknown. For example, because it was handed off to
        /// another network.
        /// </summary>
        DeliveryUncertain,
        /// <summary>
        /// Unsuccessful delivery; the message is not delivered before it
        /// expired.
        /// </summary>
        DeliveryImpossible,
        /// <summary>
        /// The message is still queued for delivery. This is a temporary state,
        /// pending transition to one of the preceding states.
        /// </summary>
        MessageWaiting,
        /// <summary>
        /// Successful delivery to the network enabler responsible for routing
        /// the SMS.
        /// </summary>
        DeliveredToNetwork,
        /// <summary>
        /// Unable to provide delivery receipt notification.
        /// </summary>
        DeliveryNotificationNotSupported,
    }
}
