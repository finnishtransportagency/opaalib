using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoneraOMA.Messaging
{
    public class DeliveryInfoListContainer
    {
        [JsonProperty(PropertyName = "deliveryInfoList", Required = Required.Always)]
        public DeliveryInfoList DeliveryInfoList { get; set; }
    }

    public class DeliveryInfoList
    {
        [JsonProperty(PropertyName = "resourceURL", Required = Required.Always)]
        public string ResourceUrl { get; set; }

        [JsonProperty(PropertyName = "deliveryInfo", Required = Required.Always)]
        public DeliveryInfo DeliveryInfo { get; set; }
    }

    public class DeliveryInfo
    {
        [JsonProperty(PropertyName = "address", Required = Required.Always)]
        public string Address { get; set; }

        // TODO: Add conversion
        [JsonProperty(PropertyName = "deliveryStatus", Required = Required.Always)]
        public DeliveryStatus DeliveryStatus { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }
    }

    public enum DeliveryStatus
    {
        DeliveredToTerminal,
        DeliveryUncertain,
        DeliveryImpossible,
        MessageWaiting,
        DeliveredToNetwork,
        DeliveryNotificationNotSupported,
    }
}
