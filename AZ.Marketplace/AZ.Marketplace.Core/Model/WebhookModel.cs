using Newtonsoft.Json;
using System;

namespace AZ.Marketplace.Core.Model
{
	public class WebhookModel
	{

		[JsonProperty("id")]
		public string Id { get; set; }

		[JsonProperty("activityId")]
		public string ActivityId { get; set; }

		[JsonProperty("subscriptionId")]
		public string SubscriptionId { get; set; }

		[JsonProperty("publisherId")]
		public string PublisherId { get; set; }

		[JsonProperty("offerId")]
		public string OfferId { get; set; }

		[JsonProperty("planId")]
		public string PlanId { get; set; }

		[JsonProperty("quantity")]
		public int? Quantity { get; set; }

		[JsonProperty("timestamp")]
		public DateTime Timestamp { get; set; }

		[JsonProperty("action")]
		public string Action { get; set; }

		[JsonProperty("status")]
		public string Status { get; set; }

	}
}