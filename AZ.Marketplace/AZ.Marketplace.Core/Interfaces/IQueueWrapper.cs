using AZ.Marketplace.Core.Model;
using System.Threading.Tasks;

namespace AZ.Marketplace.Core.Interfaces
{
	public interface IQueueWrapper
	{

		Task SendUnsubscribeMessage(WebhookModel webhookData);

		Task SendChangePlanMessage(WebhookModel webhookData);

		Task SendChangeQuantityMessage(WebhookModel webhookData);

		Task SendSuspendMessage(WebhookModel webhookData);

		Task SendReinstateMessage(WebhookModel webhookData);

		Task SendInformationalMessage(WebhookModel webhookData);

	}
}