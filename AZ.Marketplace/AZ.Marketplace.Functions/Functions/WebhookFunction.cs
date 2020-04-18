using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using AZ.Marketplace.Core.Model;
using AZ.Marketplace.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Mvc;

namespace AZ.Marketplace.Functions
{
	public class WebhookFunction
	{
		private IQueueWrapper _queueWrapper;

		public WebhookFunction(IQueueWrapper queueWrapper) =>
			_queueWrapper = queueWrapper;

		[FunctionName("Webhook")]
		public async Task<IActionResult> Run(
			[HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
			ILogger log)
		{
			string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

			var data = JsonConvert.DeserializeObject<WebhookModel>(requestBody);

			//		log.LogInformation("C# HTTP trigger function processed a request.");

			if (data.Status == "Succeeded")
			{
				switch (data.Action)
				{
					case WebhookActionType.Unsubscribe:
						await _queueWrapper.SendUnsubscribeMessage(data);
						break;
					case WebhookActionType.ChangePlan:
						await _queueWrapper.SendChangePlanMessage(data);
						break;
					case WebhookActionType.ChangeQuantity:
						await _queueWrapper.SendChangeQuantityMessage(data);
						break;
					case WebhookActionType.Suspend:
						await _queueWrapper.SendSuspendMessage(data);
						break;
					case WebhookActionType.Reinstate:
						await _queueWrapper.SendReinstateMessage(data);
						break;
				}
			}
			else
			{
				await _queueWrapper.SendInformationalMessage(data);
			}

			return new OkResult();
		}
	}
}
