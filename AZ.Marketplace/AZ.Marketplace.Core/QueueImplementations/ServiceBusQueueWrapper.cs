using AZ.Marketplace.Core.Interfaces;
using AZ.Marketplace.Core.Model;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace AZ.Marketplace.Core.QueueImplementations
{
	public class ServiceBusQueueWrapper : IQueueWrapper
	{

		private readonly IQueueClient _unsubscribeQueue;
		private readonly IQueueClient _changePlanQueue;
		private readonly IQueueClient _changeQuantityQueue;
		private readonly IQueueClient _suspendQueue;
		private readonly IQueueClient _reinstateQueue;
		private readonly IQueueClient _informationalQueue;

		public ServiceBusQueueWrapper(
			IQueueClient unsubscribeQueue,
			IQueueClient changePlanQueue,
			IQueueClient changeQuantityQueue,
			IQueueClient suspendQueue,
			IQueueClient reinstateQueue,
			IQueueClient informationalQueue)
		{
			_unsubscribeQueue = unsubscribeQueue;
			_changePlanQueue = changePlanQueue;
			_changeQuantityQueue = changeQuantityQueue;
			_suspendQueue = suspendQueue;
			_reinstateQueue = reinstateQueue;
			_informationalQueue = informationalQueue;
		}

		public async Task SendChangePlanMessage(WebhookModel webhookData)
			=> await SendMessage(webhookData, _changePlanQueue);

		public async Task SendChangeQuantityMessage(WebhookModel webhookData)
			=> await SendMessage(webhookData, _changeQuantityQueue);

		public async Task SendInformationalMessage(WebhookModel webhookData)
			=> await SendMessage(webhookData, _informationalQueue);

		public async Task SendReinstateMessage(WebhookModel webhookData)
			=> await SendMessage(webhookData, _reinstateQueue);

		public async Task SendSuspendMessage(WebhookModel webhookData)
			=> await SendMessage(webhookData, _suspendQueue);

		public async Task SendUnsubscribeMessage(WebhookModel webhookData)
			=> await SendMessage(webhookData, _unsubscribeQueue);

		private async Task SendMessage(WebhookModel webhookData, IQueueClient QueueClient)
		{
			var message = new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(webhookData)));
			await QueueClient.SendAsync(message);
		}

	}
}
