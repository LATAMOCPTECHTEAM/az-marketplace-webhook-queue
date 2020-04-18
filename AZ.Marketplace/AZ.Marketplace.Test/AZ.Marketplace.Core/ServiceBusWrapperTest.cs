using AZ.Marketplace.Core.Model;
using AZ.Marketplace.Core.QueueImplementations;
using Microsoft.Azure.ServiceBus;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace AZ.Marketplace.Test.AZ.Marketplace.Core
{
	[TestClass]
	public class ServiceBusWrapperTest
	{
		private Mock<ITopicClient> _topicClientUnsubscribeMock;
		private Mock<ITopicClient> _topicClientChangePlanMock;
		private Mock<ITopicClient> _topicClientChangeQuantityMock;
		private Mock<ITopicClient> _topicClientReinstateMock;
		private Mock<ITopicClient> _topicClientSuspendMock;
		private Mock<ITopicClient> _topicClientInformationalMock;

		private ServiceBusTopicWrapper _serviceBusWrapper;

		[TestInitialize]
		public void TestInitialize()
		{
			_topicClientUnsubscribeMock = new Mock<ITopicClient>();
			_topicClientChangePlanMock = new Mock<ITopicClient>();
			_topicClientChangeQuantityMock = new Mock<ITopicClient>();
			_topicClientReinstateMock = new Mock<ITopicClient>();
			_topicClientSuspendMock = new Mock<ITopicClient>();
			_topicClientInformationalMock = new Mock<ITopicClient>();

			_serviceBusWrapper = new ServiceBusTopicWrapper(
				_topicClientUnsubscribeMock.Object,
				_topicClientChangePlanMock.Object,
				_topicClientChangeQuantityMock.Object,
				_topicClientSuspendMock.Object,
				_topicClientReinstateMock.Object,
				_topicClientInformationalMock.Object);
		}

		private WebhookModel GetWebHookSucceededMock(string action)
		{
			var mockData = new WebhookModel()
			{
				Id = "id",
				Action = action,
				ActivityId = "activityId",
				OfferId = "offerId",
				PlanId = "planId",
				PublisherId = "publisherId",
				Quantity = 1,
				Status = "Succeeded",
				SubscriptionId = "subscriptionId",
				Timestamp = DateTime.UtcNow
			};

			return mockData;
		}


		[TestMethod]
		public async Task ShouldDispatchUnsubscribeMessageToQueue()
		{
			var mockModel = GetWebHookSucceededMock(WebhookActionType.Unsubscribe);

			await _serviceBusWrapper.SendUnsubscribeMessage(mockModel);

			var expected = JsonConvert.SerializeObject(mockModel);
			_topicClientUnsubscribeMock
				.Verify(x => x.SendAsync(It.Is<Message>(x => Encoding.UTF8.GetString(x.Body) == expected)), Times.Once);
		}

		[TestMethod]
		public async Task ShouldDispatchChangePlanMessageToQueue()
		{
			var mockModel = GetWebHookSucceededMock(WebhookActionType.ChangePlan);

			await _serviceBusWrapper.SendChangePlanMessage(mockModel);

			var expected = JsonConvert.SerializeObject(mockModel);
			_topicClientChangePlanMock
				.Verify(x => x.SendAsync(It.Is<Message>(x => Encoding.UTF8.GetString(x.Body) == expected)), Times.Once);
		}

		[TestMethod]
		public async Task ShouldDispatchChangeQuantityMessageToQueue()
		{
			var mockModel = GetWebHookSucceededMock(WebhookActionType.ChangeQuantity);

			await _serviceBusWrapper.SendChangeQuantityMessage(mockModel);

			var expected = JsonConvert.SerializeObject(mockModel);
			_topicClientChangeQuantityMock
				.Verify(x => x.SendAsync(It.Is<Message>(x => Encoding.UTF8.GetString(x.Body) == expected)), Times.Once);
		}

		[TestMethod]
		public async Task ShouldDispatchSuspendMessageToQueue()
		{
			var mockModel = GetWebHookSucceededMock(WebhookActionType.Suspend);

			await _serviceBusWrapper.SendSuspendMessage(mockModel);

			var expected = JsonConvert.SerializeObject(mockModel);
			_topicClientSuspendMock
				.Verify(x => x.SendAsync(It.Is<Message>(x => Encoding.UTF8.GetString(x.Body) == expected)), Times.Once);
		}

		[TestMethod]
		public async Task ShouldDispatchReinstateMessageToQueue()
		{
			var mockModel = GetWebHookSucceededMock(WebhookActionType.Reinstate);

			await _serviceBusWrapper.SendReinstateMessage(mockModel);

			var expected = JsonConvert.SerializeObject(mockModel);
			_topicClientReinstateMock
				.Verify(x => x.SendAsync(It.Is<Message>(x => Encoding.UTF8.GetString(x.Body) == expected)), Times.Once);
		}


		[TestMethod]
		public async Task ShouldDispatchInformationalMessageToQueue()
		{
			var mockModel = GetWebHookSucceededMock("anything");

			await _serviceBusWrapper.SendInformationalMessage(mockModel);

			var expected = JsonConvert.SerializeObject(mockModel);
			_topicClientInformationalMock
				.Verify(x => x.SendAsync(It.Is<Message>(x => Encoding.UTF8.GetString(x.Body) == expected)), Times.Once);
		}
	}
}
