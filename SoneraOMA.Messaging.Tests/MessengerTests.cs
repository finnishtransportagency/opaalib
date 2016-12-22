using Newtonsoft.Json;
using System;
using System.Linq;
using Xunit;

namespace SoneraOMA.Messaging.Tests
{
    public class MessengerTests
    {
        [Fact]
        public void AddressConversion()
        {
            var omrD = JsonConvert.DeserializeObject<OutboundMessageRequest>(@"{ address: ""test1"", senderAddress: ""asd"" }");
            Assert.Equal("test1", omrD.Address[0]);

            var omrS = JsonConvert.SerializeObject(omrD);

            omrD = JsonConvert.DeserializeObject<OutboundMessageRequest>(omrS);
            Assert.Equal("test1", omrD.Address[0]);

            omrD = JsonConvert.DeserializeObject<OutboundMessageRequest>(@"{ address: [""test1"", ""test2"", ""test3""], senderAddress: ""asd"" }");
            Assert.Collection(omrD.Address,
                v => Assert.Equal("test1", v),
                v => Assert.Equal("test2", v),
                v => Assert.Equal("test3", v));
            omrS = JsonConvert.SerializeObject(omrD);
        }

        [Fact]
        public void OutboundMessageRequestJson1()
        {
            var json = @"{
	""outboundMessageRequest"": {

        ""address"": [
			""tel:+358405005387"",
			""tel:+358405005987"",
			""tel:+358405005988"",
			""tel:+358405005989""
		],
		""senderAddress"": ""tel:+358405005900"",
		""outboundSMSBinaryMessage"": {
			""message"": ""BgUEAAAASGVsbG8gdGhlcmU=""
		},
		""senderName"": ""Sonera"",
		""receiptRequest"": {
			""notifyURL"": ""https://hostname:port/application notification endpoint"",
			""notificationFormat"": ""JSON""
		}
	}
}";

            var d = JsonConvert.DeserializeObject<OutboundMessageRequestContainer>(json);
            var req = d.OutboundMessageRequest;

            Assert.Collection(req.Address,
                v => Assert.Equal("tel:+358405005387", v),
                v => Assert.Equal("tel:+358405005987", v),
                v => Assert.Equal("tel:+358405005988", v),
                v => Assert.Equal("tel:+358405005989", v));
            Assert.Equal(req.SenderAddress, "tel:+358405005900");
            Assert.Equal(req.OutboundSmsBinaryMessage.Message, "BgUEAAAASGVsbG8gdGhlcmU=");
            Assert.Equal(req.SenderName, "Sonera");
            Assert.Equal(req.ReceiptRequest.NotifyUrl, "https://hostname:port/application notification endpoint");
            Assert.Equal(req.ReceiptRequest.NotificationFormat, "JSON");
        }

        [Fact]
        public void OutboundMessageRequestJson2()
        {
            var json = @"{
""outboundMessageRequest"": {

    ""address"": [
		""tel:+35842349023""
	],
	""senderAddress"": ""15590"",
	""outboundSMSFlashMessage"": {
		""flashMessage"": ""Flash message""
	},
	""charging"": {
		""description"": ""Charge for FN-PUSH-23 Prepaid"",
		""currency"": ""EUR"",
		""amount"": ""2.99""
	},
	""senderName"": ""Sonera"",
	""receiptRequest"": {
		""notifyURL"": ""https://hostname:port/application notification endpoint"",
		""notificationFormat"": ""JSON"",
		""callbackData"": ""test callback data""
	}
}
}";
            var d = JsonConvert.DeserializeObject<OutboundMessageRequestContainer>(json);
            var req = d.OutboundMessageRequest;

            Assert.Collection(req.Address,
                v => Assert.Equal("tel:+35842349023", v));
            Assert.Equal(req.SenderAddress, "15590");
            Assert.Equal(req.OutboundSmsFlashMessage.FlashMessage, "Flash message");
            Assert.Equal(req.Charging.Description, "Charge for FN-PUSH-23 Prepaid");
            Assert.Equal(req.Charging.Currency, "EUR");
            Assert.Equal(req.Charging.Amount, 2.99M);
            Assert.Equal(req.SenderName, "Sonera");
            Assert.Equal(req.ReceiptRequest.NotifyUrl, "https://hostname:port/application notification endpoint");
            Assert.Equal(req.ReceiptRequest.NotificationFormat, "JSON");
            Assert.Equal(req.ReceiptRequest.CallbackData, "test callback data");
        }

        [Fact]
        public void OutboundMessageRequestJson3()
        {
            var json = @"{
""outboundMessageRequest"": {

    ""address"": [
		""tel:+35842349023""
	],
	""senderAddress"": ""15590"",
	""outboundSMSTextMessage"": {
		""message"": ""Text message""
	},
	""charging"": {
		""description"": ""Charge for FN-PUSH-23 Prepaid"",
		""currency"": ""EUR"",
		""amount"": ""2.99""
	},
	""senderName"": ""Sonera"",
	""receiptRequest"": {
		""notifyURL"": ""https://hostname:port/application notification endpoint"",
		""notificationFormat"": ""JSON"",
		""callbackData"": ""test callback data""
	}
}
}";

            var d = JsonConvert.DeserializeObject<OutboundMessageRequestContainer>(json);
            var req = d.OutboundMessageRequest;

            Assert.Collection(req.Address,
                v => Assert.Equal("tel:+35842349023", v));
            Assert.Equal(req.SenderAddress, "15590");
            Assert.Equal(req.OutboundSmsTextMessage.Message, "Text message");
            Assert.Equal(req.Charging.Description, "Charge for FN-PUSH-23 Prepaid");
            Assert.Equal(req.Charging.Currency, "EUR");
            Assert.Equal(req.Charging.Amount, 2.99M);
            Assert.Equal(req.SenderName, "Sonera");
            Assert.Equal(req.ReceiptRequest.NotifyUrl, "https://hostname:port/application notification endpoint");
            Assert.Equal(req.ReceiptRequest.NotificationFormat, "JSON");
            Assert.Equal(req.ReceiptRequest.CallbackData, "test callback data");
        }
    }
}
