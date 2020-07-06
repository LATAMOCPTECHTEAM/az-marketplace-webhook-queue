using System;
using System.Collections.Generic;
using System.Text;

namespace AZ.Marketplace.Core.Model
{
	public class ApplicationConfig
	{

		public string QueueType { get; set; }

		public ApplicationConfigServiceBus ServiceBus { get; set; } = new ApplicationConfigServiceBus();

	}

	public class ApplicationConfigType
	{

		public const string ServiceBusTopic = "servicebus-topic";

	}

	public class ApplicationConfigConnectionStrings
	{
		
		public string ServiceBus { get; set; }
	
	}

	public class ApplicationConfigServiceBus
	{

		public string UnsubscribeTopic { get; set; } = "unsubscribe";

		public string ChangePlanTopic { get; set; } = "changeplan";

		public string ChangeQuantityTopic { get; set; } = "changequantity";

		public string SuspendTopic { get; set; } = "suspend";

		public string ReinstateTopic { get; set; } = "reinstate";

		public string InformationalTopic { get; set; } = "informational";

	}
}