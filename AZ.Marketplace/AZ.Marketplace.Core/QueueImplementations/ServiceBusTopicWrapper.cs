using AZ.Marketplace.Core.Interfaces;
using AZ.Marketplace.Core.Model;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace AZ.Marketplace.Core.QueueImplementations
{
	public class ServiceBusTopicWrapper : IQueueWrapper
	{

		private readonly ITopicClient _unsubscribeTopic;
		private readonly ITopicClient _changePlanTopic;
		private readonly ITopicClient _changeQuantityTopic;
		private readonly ITopicClient _suspendTopic;
		private readonly ITopicClient _reinstateTopic;
		private readonly ITopicClient _informationalTopic;

		public ServiceBusTopicWrapper(
			ITopicClient unsubscribeTopic,
			ITopicClient changePlanTopic,
			ITopicClient changeQuantityTopic,
			ITopicClient suspendTopic,
			ITopicClient reinstateTopic,
			ITopicClient informationalTopic)
		{
			_unsubscribeTopic = unsubscribeTopic;
			_changePlanTopic = changePlanTopic;
			_changeQuantityTopic = changeQuantityTopic;
			_suspendTopic = suspendTopic;
			_reinstateTopic = reinstateTopic;
			_informationalTopic = informationalTopic;
		}

		public async Task SendChangePlanMessage(WebhookModel webhookData)
			=> await SendMessage(webhookData, _changePlanTopic);

		public async Task SendChangeQuantityMessage(WebhookModel webhookData)
			=> await SendMessage(webhookData, _changeQuantityTopic);

		public async Task SendInformationalMessage(WebhookModel webhookData)
			=> await SendMessage(webhookData, _informationalTopic);

		public async Task SendReinstateMessage(WebhookModel webhookData)
			=> await SendMessage(webhookData, _reinstateTopic);

		public async Task SendSuspendMessage(WebhookModel webhookData)
			=> await SendMessage(webhookData, _suspendTopic);

		public async Task SendUnsubscribeMessage(WebhookModel webhookData)
			=> await SendMessage(webhookData, _unsubscribeTopic);

		private async Task SendMessage(WebhookModel webhookData, ITopicClient topicClient)
		{
			var message = new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(webhookData)));
			await topicClient.SendAsync(message);
		}
	
	}
}
