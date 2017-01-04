using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opaalib.Messaging
{
    public class OutboundMessageRequestContainer
    {
        [JsonProperty(PropertyName = "outboundMessageRequest", Required = Required.Always)]
        public OutboundMessageRequest OutboundMessageRequest { get; set; }
    }

    public class OutboundMessageRequest
    {
        /// <summary>
        /// Destination addressses This must be an international number of at
        /// least 9 digits prefixed with “tel:+”.
        /// </summary>
        [JsonProperty(PropertyName = "address", Required = Required.Always)]
        ///[JsonConverter(typeof(SingleStringToListConverter))]
        public List<string> Address { get; set; }

        /// <summary>
        /// The address of the sender to whom a responding
        /// message may be sent. This must match the
        /// senderAddress value in the URI.<para />
        /// The number must be prefixed with tel: or short: as
        /// appropriate.<para />
        /// •  tel: must be followed by + and an
        /// international number containing a minimum
        /// of 9 digits<para />
        /// •  You may use short codes either without the
        /// short: prefix or with prefix.
        /// </summary>
        [JsonProperty(PropertyName = "senderAddress", Required = Required.Always)]
        public string SenderAddress { get; set; }

        /// <summary>
        /// The name of the sender to appear on the user’s
        /// terminal as the originator of the message.This
        /// parameter supports maximum 11 characters.
        /// </summary>
        [JsonProperty(PropertyName = "senderName")]
        public string SenderName { get; set; }

        /// <summary>
        /// Use if delivery notification is required, to include the notifyURL
        /// </summary>
        [JsonProperty(PropertyName = "receiptRequest")]
        public ReceiptRequest ReceiptRequest { get; set; }

        /// <summary>
        /// A correlator that the client can use to tag this particular
        /// resource representation during a request to create a
        /// resource on the server.
        /// </summary>
        [JsonProperty(PropertyName = "clientCorrelator")]
        public string ClientCorrelator { get; set; }

        /// <summary>
        /// Included for premium SMS, when charging data is
        /// passed in the message.
        /// </summary>
        [JsonProperty(PropertyName = "charging")]
        public ChargingInformation Charging { get; set; }

        /// <summary>
        /// Choose one message type to use. Leave others null.
        /// </summary>
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
        /// <summary>
        /// The endpoint that is used to notify the application when
        /// the message is delivered to a terminal or delivery is
        /// impossible. This parameter supports maximum 255
        /// characters.<para />
        /// Delivery notifications is sent when this value is present.
        /// </summary>
        [JsonProperty(PropertyName = "notifyURL", Required = Required.Always)]
        public string NotifyUrl { get; set; }

        /// <summary>
        /// JSON or XML
        /// </summary>
        // TODO: Maybe do enum conversion here (JSON or XML)
        [JsonProperty(PropertyName = "notificationFormat")]
        public string NotificationFormat { get; set; }

        /// <summary>
        /// Useful data to be passed back in the notification to
        /// identify the message. This parameter supports
        /// maximum 255 characters.
        /// </summary>
        [JsonProperty(PropertyName = "callbackData")]
        public string CallbackData { get; set; }
    }

    public class ChargingInformation
    {
        /// <summary>
        /// Any text describing or related to the charge, such as
        /// the billing text.This parameter supports maximum
        /// 11 characters as billing text, if your short code is five
        /// digits long. If your short code is six digits long, then
        /// only 10 characters are shown as billing text.
        /// </summary>
        // TODO: Is this array or string?
        [JsonProperty(PropertyName = "description", Required = Required.Always)]
        public string Description { get; set; }

        /// <summary>
        /// The currency applied to the charge, in ISO 4217
        /// conformant code such as EUR.This parameter
        /// supports maximum 255 characters.
        /// </summary>
        [JsonProperty(PropertyName = "currency", Required = Required.Always)]
        public string Currency { get; set; }

        /// <summary>
        /// The amount to be charged.<para />
        /// NOTE: The amount defined here for service
        /// price is excluding VAT.
        /// </summary>
        [JsonProperty(PropertyName = "amount", Required = Required.Always)]
        public decimal Amount { get; set; }

        /// <summary>
        /// The charging code.
        /// </summary>
        [JsonProperty(PropertyName = "code")]
        public string Code { get; set; }
    }

    public class OutboundSmsBinaryMessage
    {
        /// <summary>
        /// Format should be base64Binary
        /// </summary>
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
}
