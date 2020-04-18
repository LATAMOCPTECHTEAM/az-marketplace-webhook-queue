using AZ.Marketplace.Core.Interfaces;
using AZ.Marketplace.Core.Model;
using AZ.Marketplace.Functions;
using AZ.Marketplace.Test.Functions;
using AZ.Marketplace.Test.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AZ.Marketplace.AZ.Marketplace.Functions
{
	[TestClass]
	public class WebhookFunctionTest
	{

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

		private IDictionary<string, string> GetBodyFromMock(WebhookModel model)
		{
			var dictionary = new Dictionary<string, string>();
			dictionary.Add("id", model.Id);
			dictionary.Add("action", model.Action);
			dictionary.Add("activityId", model.ActivityId);
			dictionary.Add("offerId", model.OfferId);
			dictionary.Add("planId", model.PlanId);
			dictionary.Add("publisherId", model.PublisherId);
			dictionary.Add("quantity", model.Quantity?.ToString());
			dictionary.Add("status", model.Status);
			dictionary.Add("subscriptionId", model.SubscriptionId);
			dictionary.Add("timestamp", model.Timestamp.ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ"));
			return dictionary;
		}

		private Mock<IQueueWrapper> _queueWrapperMock;

		[TestInitialize]
		public void TestInitialize()
		{
			_queueWrapperMock = new Mock<IQueueWrapper>();
		}

		#region [ Unsubscribe ]

		[TestMethod]
		public async Task Should_Dispatch_Unsubscribe_Message_When_Action_Is_Unsubscribe_And_Status_Is_Valid()
		{
			var mockData = GetWebHookSucceededMock(WebhookActionType.Unsubscribe);

			var request = FunctionRequestHelper.CreateHttpRequest(null, null, GetBodyFromMock(mockData));

			var http = new WebhookFunction(_queueWrapperMock.Object);
			var response = (StatusCodeResult)await http.Run(request, LoggerHelper.CreateLogger());

			_queueWrapperMock.Verify(x => x.SendUnsubscribeMessage(It.Is<WebhookModel>(x => JsonConvert.SerializeObject(x) == JsonConvert.SerializeObject(mockData))), Times.Once);
			Assert.AreEqual(response.StatusCode, 200);
		}

		[TestMethod]
		public async Task Should_Not_Dispatch_Unsubscribe_Message_When_Action_Is_Not_Unsubscribe()
		{
			var mockData = GetWebHookSucceededMock("anythingelse");

			var request = FunctionRequestHelper.CreateHttpRequest(null, null, GetBodyFromMock(mockData));

			var http = new WebhookFunction(_queueWrapperMock.Object);
			var response = (StatusCodeResult)await http.Run(request, LoggerHelper.CreateLogger());

			_queueWrapperMock.Verify(x => x.SendUnsubscribeMessage(It.IsAny<WebhookModel>()), Times.Never());
			Assert.AreEqual(response.StatusCode, 200);
		}

		[TestMethod]
		public async Task Should_Not_Dispatch_Unsubscribe_Message_When_Status_Is_Invalid()
		{
			var mockData = GetWebHookSucceededMock(WebhookActionType.Unsubscribe);
			mockData.Status = "Failed";

			var request = FunctionRequestHelper.CreateHttpRequest(null, null, GetBodyFromMock(mockData));

			var http = new WebhookFunction(_queueWrapperMock.Object);
			var response = (StatusCodeResult)await http.Run(request, LoggerHelper.CreateLogger());

			_queueWrapperMock.Verify(x => x.SendUnsubscribeMessage(It.IsAny<WebhookModel>()), Times.Never());
			Assert.AreEqual(response.StatusCode, 200);
		}

		#endregion

		#region [ Change Plan ]

		[TestMethod]
		public async Task Should_Dispatch_ChangePlan_Message_When_Action_Is_ChangePlan_And_Status_Is_Valids()
		{
			var mockData = GetWebHookSucceededMock(WebhookActionType.ChangePlan);

			var body = GetBodyFromMock(mockData);
			var request = FunctionRequestHelper.CreateHttpRequest(null, null, body);

			var http = new WebhookFunction(_queueWrapperMock.Object);
			var response = (StatusCodeResult)await http.Run(request, LoggerHelper.CreateLogger());

			_queueWrapperMock.Verify(x => x.SendChangePlanMessage(It.Is<WebhookModel>(x => JsonConvert.SerializeObject(x) == JsonConvert.SerializeObject(mockData))), Times.Once);
			Assert.AreEqual(response.StatusCode, 200);
		}

		[TestMethod]
		public async Task Should_Not_Dispatch_ChangePlan_Message_When_Action_Is_Not_ChangePlan()
		{
			var mockData = GetWebHookSucceededMock("anythingelse");

			var request = FunctionRequestHelper.CreateHttpRequest(null, null, GetBodyFromMock(mockData));

			var http = new WebhookFunction(_queueWrapperMock.Object);
			var response = (StatusCodeResult)await http.Run(request, LoggerHelper.CreateLogger());

			_queueWrapperMock.Verify(x => x.SendChangePlanMessage(It.IsAny<WebhookModel>()), Times.Never());
			Assert.AreEqual(response.StatusCode, 200);
		}

		[TestMethod]
		public async Task Should_Not_Dispatch_ChangePlan_Message_When_Status_Is_Invalid()
		{
			var mockData = GetWebHookSucceededMock(WebhookActionType.ChangePlan);
			mockData.Status = "Failed";

			var request = FunctionRequestHelper.CreateHttpRequest(null, null, GetBodyFromMock(mockData));

			var http = new WebhookFunction(_queueWrapperMock.Object);
			var response = (StatusCodeResult)await http.Run(request, LoggerHelper.CreateLogger());

			_queueWrapperMock.Verify(x => x.SendChangePlanMessage(It.IsAny<WebhookModel>()), Times.Never());
			Assert.AreEqual(response.StatusCode, 200);
		}

		#endregion

		#region [ Change Quantity ]

		[TestMethod]
		public async Task Should_Dispatch_ChangeQuantity_Message_When_Action_Is_ChangeQuantity_And_Status_Is_Valids()
		{
			var mockData = GetWebHookSucceededMock(WebhookActionType.ChangeQuantity);

			var request = FunctionRequestHelper.CreateHttpRequest(null, null, GetBodyFromMock(mockData));

			var http = new WebhookFunction(_queueWrapperMock.Object);
			var response = (StatusCodeResult)await http.Run(request, LoggerHelper.CreateLogger());

			_queueWrapperMock.Verify(x => x.SendChangeQuantityMessage(It.Is<WebhookModel>(x => JsonConvert.SerializeObject(x) == JsonConvert.SerializeObject(mockData))), Times.Once);
			Assert.AreEqual(response.StatusCode, 200);
		}

		[TestMethod]
		public async Task Should_Not_Dispatch_ChangeQuantity_Message_When_Action_Is_Not_ChangeQuantity()
		{
			var mockData = GetWebHookSucceededMock("anythingelse");

			var request = FunctionRequestHelper.CreateHttpRequest(null, null, GetBodyFromMock(mockData));

			var http = new WebhookFunction(_queueWrapperMock.Object);
			var response = (StatusCodeResult)await http.Run(request, LoggerHelper.CreateLogger());

			_queueWrapperMock.Verify(x => x.SendChangeQuantityMessage(It.IsAny<WebhookModel>()), Times.Never());
			Assert.AreEqual(response.StatusCode, 200);
		}

		[TestMethod]
		public async Task Should_Not_Dispatch_ChangeQuantity_Message_When_Status_Is_Invalid()
		{
			var mockData = GetWebHookSucceededMock(WebhookActionType.ChangeQuantity);
			mockData.Status = "Failed";

			var request = FunctionRequestHelper.CreateHttpRequest(null, null, GetBodyFromMock(mockData));

			var http = new WebhookFunction(_queueWrapperMock.Object);
			var response = (StatusCodeResult)await http.Run(request, LoggerHelper.CreateLogger());

			_queueWrapperMock.Verify(x => x.SendChangeQuantityMessage(It.IsAny<WebhookModel>()), Times.Never());
			Assert.AreEqual(response.StatusCode, 200);
		}

		#endregion

		#region [ Suspend ]

		[TestMethod]
		public async Task Should_Dispatch_Suspend_Message_When_Action_Is_Suspend_And_Status_Is_Valid()
		{
			var mockData = GetWebHookSucceededMock(WebhookActionType.Suspend);

			var request = FunctionRequestHelper.CreateHttpRequest(null, null, GetBodyFromMock(mockData));

			var http = new WebhookFunction(_queueWrapperMock.Object);
			var response = (StatusCodeResult)await http.Run(request, LoggerHelper.CreateLogger());

			_queueWrapperMock.Verify(x => x.SendSuspendMessage(It.Is<WebhookModel>(x => JsonConvert.SerializeObject(x) == JsonConvert.SerializeObject(mockData))), Times.Once);
			Assert.AreEqual(response.StatusCode, 200);
		}

		[TestMethod]
		public async Task Should_Not_Dispatch_Suspend_Message_When_Action_Is_Not_Suspend()
		{
			var mockData = GetWebHookSucceededMock("anythingelse");

			var request = FunctionRequestHelper.CreateHttpRequest(null, null, GetBodyFromMock(mockData));

			var http = new WebhookFunction(_queueWrapperMock.Object);
			var response = (StatusCodeResult)await http.Run(request, LoggerHelper.CreateLogger());

			_queueWrapperMock.Verify(x => x.SendSuspendMessage(It.IsAny<WebhookModel>()), Times.Never());
			Assert.AreEqual(response.StatusCode, 200);
		}

		[TestMethod]
		public async Task Should_Not_Dispatch_Suspend_Message_When_Status_Is_Invalid()
		{
			var mockData = GetWebHookSucceededMock(WebhookActionType.Suspend);
			mockData.Status = "Failed";

			var request = FunctionRequestHelper.CreateHttpRequest(null, null, GetBodyFromMock(mockData));

			var http = new WebhookFunction(_queueWrapperMock.Object);
			var response = (StatusCodeResult)await http.Run(request, LoggerHelper.CreateLogger());

			_queueWrapperMock.Verify(x => x.SendSuspendMessage(It.IsAny<WebhookModel>()), Times.Never());
			Assert.AreEqual(response.StatusCode, 200);
		}

		#endregion

		#region [ Reinstate ]

		[TestMethod]
		public async Task Should_Dispatch_Reinstate_Message_When_Action_Is_Reinstate_And_Status_Is_Valid()
		{
			var mockData = GetWebHookSucceededMock(WebhookActionType.Reinstate);

			var request = FunctionRequestHelper.CreateHttpRequest(null, null, GetBodyFromMock(mockData));

			var http = new WebhookFunction(_queueWrapperMock.Object);
			var response = (StatusCodeResult)await http.Run(request, LoggerHelper.CreateLogger());

			_queueWrapperMock.Verify(x => x.SendReinstateMessage(It.Is<WebhookModel>(x => JsonConvert.SerializeObject(x) == JsonConvert.SerializeObject(mockData))), Times.Once);
			Assert.AreEqual(response.StatusCode, 200);
		}

		[TestMethod]
		public async Task Should_Not_Dispatch_Reinstate_Message_When_Action_Is_Not_Reinstate()
		{
			var mockData = GetWebHookSucceededMock("anythingelse");

			var request = FunctionRequestHelper.CreateHttpRequest(null, null, GetBodyFromMock(mockData));

			var http = new WebhookFunction(_queueWrapperMock.Object);
			var response = (StatusCodeResult)await http.Run(request, LoggerHelper.CreateLogger());

			_queueWrapperMock.Verify(x => x.SendReinstateMessage(It.IsAny<WebhookModel>()), Times.Never());
			Assert.AreEqual(response.StatusCode, 200);
		}

		[TestMethod]
		public async Task Should_Not_Dispatch_Reinstate_Message_When_Status_Is_Invalid()
		{
			var mockData = GetWebHookSucceededMock(WebhookActionType.Reinstate);
			mockData.Status = "Failed";

			var request = FunctionRequestHelper.CreateHttpRequest(null, null, GetBodyFromMock(mockData));

			var http = new WebhookFunction(_queueWrapperMock.Object);
			var response = (StatusCodeResult)await http.Run(request, LoggerHelper.CreateLogger());

			_queueWrapperMock.Verify(x => x.SendReinstateMessage(It.IsAny<WebhookModel>()), Times.Never());
			Assert.AreEqual(response.StatusCode, 200);
		}

		#endregion

		#region [ Informational ]

		[TestMethod]
		public async Task Should_Dispatch_Informational_Message_When_Status_Is_Invalid()
		{
			var mockData = GetWebHookSucceededMock("anything");
			mockData.Status = "anything";

			var request = FunctionRequestHelper.CreateHttpRequest(null, null, GetBodyFromMock(mockData));

			var http = new WebhookFunction(_queueWrapperMock.Object);
			var response = (StatusCodeResult)await http.Run(request, LoggerHelper.CreateLogger());

			_queueWrapperMock.Verify(x => x.SendInformationalMessage(It.Is<WebhookModel>(x => JsonConvert.SerializeObject(x) == JsonConvert.SerializeObject(mockData))), Times.Once);
			Assert.AreEqual(response.StatusCode, 200);
		}

		[TestMethod]
		public async Task Should_Not_Dispatch_Informational_Message_When_Status_Is_Valid()
		{
			var mockData = GetWebHookSucceededMock(WebhookActionType.Reinstate);
			mockData.Status = "Succeeded";

			var request = FunctionRequestHelper.CreateHttpRequest(null, null, GetBodyFromMock(mockData));

			var http = new WebhookFunction(_queueWrapperMock.Object);
			var response = (StatusCodeResult)await http.Run(request, LoggerHelper.CreateLogger());

			_queueWrapperMock.Verify(x => x.SendInformationalMessage(It.IsAny<WebhookModel>()), Times.Never());
			Assert.AreEqual(response.StatusCode, 200);
		}

		#endregion

	}
}