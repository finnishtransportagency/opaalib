using Newtonsoft.Json;
using System;
using System.Linq;
using Xunit;

namespace Opaalib.Messaging.Tests
{
    public class MessengerTests
    {
        [Fact(Skip = "Not needed because this conversion doesn't happen anymore")]
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
            Assert.Equal(req.ReceiptRequest.NotificationFormat, NotificationFormat.JSON);
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
            Assert.Equal(req.ReceiptRequest.NotificationFormat, NotificationFormat.JSON);
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
		""notificationFormat"": ""XML"",
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
            Assert.Equal(req.ReceiptRequest.NotificationFormat, NotificationFormat.XML);
            Assert.Equal(req.ReceiptRequest.CallbackData, "test callback data");
        }

        [Fact]
        public void OutboundMessageResponseJson1()
        {
            var json = @"{
    ""resourceReference"" : {
    ""resourceURL"" : ""https://api.sonera.fi/production/messaging/v1/outbound/tel%3A%2B358405005900/requests/2f57108f-cdb4-4501-9f02-b6f5399596e2""
    }
}";

            var d = JsonConvert.DeserializeObject<OutboundMessageResponse>(json);
            var res = d.ResourceReference;

            Assert.Equal(res.ResourceUrl, "https://api.sonera.fi/production/messaging/v1/outbound/tel%3A%2B358405005900/requests/2f57108f-cdb4-4501-9f02-b6f5399596e2");
        }

        [Fact]
        public void OutboundMessageDeliveryStatusResponseJson1()
        {
            var json = @"{
    ""deliveryInfoList"": {

        ""resourceURL"": ""https://api.sonera.fi/production/messaging/v1/outbound/tel%3A%2B358405005900/requests/cb513ae8-c630-409e-abcb-6bb55cfd7873/deliveryInfos"",
	    ""deliveryInfo"": [
		    {
			    ""address"": ""tel:+358405007000"",
			    ""deliveryStatus"": ""DeliveryImpossible""

            },
		    {
			    ""address"": ""tel:+358405007001"",
			    ""deliveryStatus"": ""DeliveredToNetwork""
		    },
		    {
			    ""address"": ""tel:+358405007002"",
			    ""deliveryStatus"": ""DeliveryImpossible""
		    }
	    ]
    }
}";

            var d = JsonConvert.DeserializeObject<DeliveryInfoListContainer>(json);
            var res = d.DeliveryInfoList;

            Assert.Equal("https://api.sonera.fi/production/messaging/v1/outbound/tel%3A%2B358405005900/requests/cb513ae8-c630-409e-abcb-6bb55cfd7873/deliveryInfos", res.ResourceUrl);
            Assert.Collection(res.DeliveryInfo,
                v =>
                {
                    Assert.Equal("tel:+358405007000", v.Address);
                    Assert.Equal(DeliveryStatus.DeliveryImpossible, v.DeliveryStatus);
                },
                v =>
                {
                    Assert.Equal("tel:+358405007001", v.Address);
                    Assert.Equal(DeliveryStatus.DeliveredToNetwork, v.DeliveryStatus);
                },
                v =>
                {
                    Assert.Equal("tel:+358405007002", v.Address);
                    Assert.Equal(DeliveryStatus.DeliveryImpossible, v.DeliveryStatus);
                });
        }

        [Fact]
        public void InboundMessageRetrieveAndDeleteRequestJson1()
        {
            var json = @"{
""inboundMessageRetrieveAndDeleteRequest"":{
    ""retrievalOrder"":""OldestFirst"",
    ""useAttachmentURLs"":""true""}
    }";

            var d = JsonConvert.DeserializeObject<InboundMessageRetrieveAndDeleteRequestContainer>(json);
            var req = d.InboundMessageRetrieveAndDeleteRequest;

            Assert.Equal(req.RetrievalOrder, "OldestFirst");
            Assert.Equal(req.UseAttachmentUrls, true);
        }

        [Fact]
        public void InboundMessageRetrieveAndDeleteResponseJson1()
        {
            var json = @"{
    ""inboundMessageList"": {
        ""inboundMessage"": [
            {
                ""destinationAddress"": ""70200"",
                ""senderAddress"": ""tel:+35842347002"",
                ""dateTime"": ""2015-04-14T07:35:31.000+0000"",
                ""resourceURL"": ""https://api.sonera.fi/production/messaging/v1/inbound/registrations/822c82991bd145e493a3690e871800e2/messages/retrieveAndDeleteMessages/4883911"",
                ""messageId"": ""4883911"",
                ""inboundSMSTextMessage"": {
                    ""message"": ""Nokeyword FN-REC-ONLY-02 - Autoframing""
                }
            },
            {
                ""destinationAddress"": ""70200"",
                ""senderAddress"": ""tel:+35842347002"",
                ""dateTime"": ""2015-04-14T07:36:31.000+0000"",
                ""resourceURL"": ""https://api.sonera.fi/production/messaging/v1/inbound/registrations/822c82991bd145e493a3690e871800e2/messages/retrieveAndDeleteMessages/4883912"",
                ""messageId"": ""4883912"",
                ""inboundSMSTextMessage"": {
                    ""message"": ""Nokeyword FN-REC-ONLY-02 - Autoframing""
                }
            }
        ],
        ""totalNumberOfPendingMessages"": 0,
        ""numberOfMessagesInThisBatch"": 2,
        ""resourceURL"": ""https://api.sonera.fi/production/messaging/v1/inbound/registrations/822c82991bd145e493a3690e871800e2/messages/retrieveAndDeleteMessages""
    }
}";

            var d = JsonConvert.DeserializeObject<InboundMessageListContainer>(json);
            var req = d.InboundMessageList;

            Assert.Collection(req.InboundMessage,
                v =>
                {
                    Assert.Equal(DateTime.Parse("2015-04-14T07:35:31.000+0000"), v.DateTime);
                    Assert.Equal("Nokeyword FN-REC-ONLY-02 - Autoframing", v.InboundSmsTextMessage.Message);
                },
                v =>
                {
                    Assert.Equal(DateTime.Parse("2015-04-14T07:36:31.000+0000"), v.DateTime);
                    Assert.Equal("Nokeyword FN-REC-ONLY-02 - Autoframing", v.InboundSmsTextMessage.Message);
                });
            Assert.Equal(0, req.TotalNumberOfPendingMessages);
            Assert.Equal(2, req.NumberOfMessagesInThisBatch);
        }

        [Fact]
        public void ServiceException()
        {
            var json = @"{
    ""requestError"": {
        ""serviceException"": {
            ""messageId"": ""SVC0002"",
            ""text"": ""Invalid input value for message part %1 with value %2. Reason %3"",
            ""variables"": [
                ""address"",
                ""447919891111"",
                ""Invalid address element""
            ]
        }
    }
}";
            var re = JsonConvert.DeserializeObject<RequestErrorContainer>(json).RequestError;

            Assert.Null(re.PolicyException);
            Assert.Equal("Invalid input value for message part address with value 447919891111. Reason Invalid address element", 
                re.ServiceException.FormattedText);
            Assert.Collection(re.ServiceException.Variables,
                v => Assert.Equal("address", v),
                v => Assert.Equal("447919891111", v),
                v => Assert.Equal("Invalid address element", v));
        }

        [Fact]
        public void PolicyException()
        {
            var json = @"{
    ""requestError"": {
        ""policyException"": {
            ""messageId"": ""POL2000"",
            ""text"": ""The following policy error occurred: %1. Error code is %2."",
            ""variables"": [
                ""Destination Black List is enforced and address is in Destination Black List."",
                ""POL-028""
            ]
        }
    }
}";
            var re = JsonConvert.DeserializeObject<RequestErrorContainer>(json).RequestError;

            Assert.Null(re.ServiceException);
            Assert.Equal("The following policy error occurred: Destination Black List is enforced and address is in Destination Black List.. Error code is POL-028.",
                re.PolicyException.FormattedText);
            Assert.Collection(re.PolicyException.Variables,
                v => Assert.Equal("Destination Black List is enforced and address is in Destination Black List.", v),
                v => Assert.Equal("POL-028", v));
        }
    }
}
