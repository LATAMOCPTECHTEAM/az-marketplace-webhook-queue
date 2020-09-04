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
	public class ServiceBusQueueWrapperTest
	{
		private Mock<IQueueClient> _QueueClientUnsubscribeMock;
		private Mock<IQueueClient> _QueueClientChangePlanMock;
		private Mock<IQueueClient> _QueueClientChangeQuantityMock;
		private Mock<IQueueClient> _QueueClientReinstateMock;
		private Mock<IQueueClient> _QueueClientSuspendMock;
		private Mock<IQueueClient> _QueueClientInformationalMock;

		private ServiceBusQueueWrapper _serviceBusWrapper;

		[TestInitialize]
		public void TestInitialize()
		{
			_QueueClientUnsubscribeMock = new Mock<IQueueClient>();
			_QueueClientChangePlanMock = new Mock<IQueueClient>();
			_QueueClientChangeQuantityMock = new Mock<IQueueClient>();
			_QueueClientReinstateMock = new Mock<IQueueClient>();
			_QueueClientSuspendMock = new Mock<IQueueClient>();
			_QueueClientInformationalMock = new Mock<IQueueClient>();

			_serviceBusWrapper = new ServiceBusQueueWrapper(
				_QueueClientUnsubscribeMock.Object,
				_QueueClientChangePlanMock.Object,
				_QueueClientChangeQuantityMock.Object,
				_QueueClientSuspendMock.Object,
				_QueueClientReinstateMock.Object,
				_QueueClientInformationalMock.Object);
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
			_QueueClientUnsubscribeMock
				.Verify(x => x.SendAsync(It.Is<Message>(x => Encoding.UTF8.GetString(x.Body) == expected)), Times.Once);
		}

		[TestMethod]
		public async Task ShouldDispatchChangePlanMessageToQueue()
		{
			var mockModel = GetWebHookSucceededMock(WebhookActionType.ChangePlan);

			await _serviceBusWrapper.SendChangePlanMessage(mockModel);

			var expected = JsonConvert.SerializeObject(mockModel);
			_QueueClientChangePlanMock
				.Verify(x => x.SendAsync(It.Is<Message>(x => Encoding.UTF8.GetString(x.Body) == expected)), Times.Once);
		}

		[TestMethod]
		public async Task ShouldDispatchChangeQuantityMessageToQueue()
		{
			var mockModel = GetWebHookSucceededMock(WebhookActionType.ChangeQuantity);

			await _serviceBusWrapper.SendChangeQuantityMessage(mockModel);

			var expected = JsonConvert.SerializeObject(mockModel);
			_QueueClientChangeQuantityMock
				.Verify(x => x.SendAsync(It.Is<Message>(x => Encoding.UTF8.GetString(x.Body) == expected)), Times.Once);
		}

		[TestMethod]
		public async Task ShouldDispatchSuspendMessageToQueue()
		{
			var mockModel = GetWebHookSucceededMock(WebhookActionType.Suspend);

			await _serviceBusWrapper.SendSuspendMessage(mockModel);

			var expected = JsonConvert.SerializeObject(mockModel);
			_QueueClientSuspendMock
				.Verify(x => x.SendAsync(It.Is<Message>(x => Encoding.UTF8.GetString(x.Body) == expected)), Times.Once);
		}

		[TestMethod]
		public async Task ShouldDispatchReinstateMessageToQueue()
		{
			var mockModel = GetWebHookSucceededMock(WebhookActionType.Reinstate);

			await _serviceBusWrapper.SendReinstateMessage(mockModel);

			var expected = JsonConvert.SerializeObject(mockModel);
			_QueueClientReinstateMock
				.Verify(x => x.SendAsync(It.Is<Message>(x => Encoding.UTF8.GetString(x.Body) == expected)), Times.Once);
		}


		[TestMethod]
		public async Task ShouldDispatchInformationalMessageToQueue()
		{
			var mockModel = GetWebHookSucceededMock("anything");

			await _serviceBusWrapper.SendInformationalMessage(mockModel);

			var expected = JsonConvert.SerializeObject(mockModel);
			_QueueClientInformationalMock
				.Verify(x => x.SendAsync(It.Is<Message>(x => Encoding.UTF8.GetString(x.Body) == expected)), Times.Once);
		}
	}
}
