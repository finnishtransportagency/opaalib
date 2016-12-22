using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoneraOMA.Messaging
{
    public class OutboundMessageRequestContainer
    {
        [JsonProperty(PropertyName = "outboundMessageRequest", Required = Required.Always)]
        public OutboundMessageRequest OutboundMessageRequest { get; set; }
    }

    public class OutboundMessageRequest
    {
        [JsonProperty(PropertyName = "address", Required = Required.Always)]
        [JsonConverter(typeof(SingleStringToListConverter))]
        public List<string> Address { get; set; }

        [JsonProperty(PropertyName = "senderAddress", Required = Required.Always)]
        public string SenderAddress { get; set; }

        [JsonProperty(PropertyName = "senderName")]
        public string SenderName { get; set; }

        [JsonProperty(PropertyName = "receiptRequest")]
        public ReceiptRequest ReceiptRequest { get; set; }

        [JsonProperty(PropertyName = "clientCorrelator")]
        public string ClientCorrelator { get; set; }

        [JsonProperty(PropertyName = "charging")]
        public ChargingInformation Charging { get; set; }

        // TODO: Maybe do some kind of check so that only one kind of message is present
        [JsonProperty(PropertyName = "outboundSMSBinaryMessage")]
        public OutboundSmsBinaryMessage OutboundSmsBinaryMessage { get; set; }

        [JsonProperty(PropertyName = "outboundSMSFlashMessage")]
        public OutboundSmsFlashMessage OutboundSmsFlashMessage { get; set; }

        [JsonProperty(PropertyName = "outboundSMSTextMessage")]
        public OutboundSmsTextMessage OutboundSmsTextMessage { get; set; }
    }

    public class ReceiptRequest
    {
        [JsonProperty(PropertyName = "notifyURL", Required = Required.Always)]
        public string NotifyUrl { get; set; }

        // TODO: Maybe do enum conversion here (JSON or XML)
        [JsonProperty(PropertyName = "notificationFormat")]
        public string NotificationFormat { get; set; }

        [JsonProperty(PropertyName = "callbackData")]
        public string CallbackData { get; set; }
    }

    public class ChargingInformation
    {
        // TODO: Is this array or string?
        [JsonProperty(PropertyName = "description", Required = Required.Always)]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "currency", Required = Required.Always)]
        public string Currency { get; set; }

        [JsonProperty(PropertyName = "amount", Required = Required.Always)]
        public decimal Amount { get; set; }

        [JsonProperty(PropertyName = "code")]
        public string Code { get; set; }
    }

    public class OutboundSmsBinaryMessage
    {
        // This is in base64 binary
        // TODO: Maybe do some kind of conversion here
        [JsonProperty(PropertyName = "message", Required = Required.Always)]
        public string Message { get; set; }
    }

    public class OutboundSmsFlashMessage
    {
        // TODO: Is this "flashMessage" or "message"? The spec uses both
        [JsonProperty(PropertyName = "flashMessage", Required = Required.Always)]
        public string FlashMessage { get; set; }
    }

    public class OutboundSmsTextMessage
    {
        [JsonProperty(PropertyName = "message", Required = Required.Always)]
        public string Message { get; set; }
    }

    public enum OutboundMessageError
    {
        AuthenticationFailed,
        RequestFailed,
        InvalidRequest,
        InvalidResponse,
    }
}
