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

		public const string ServiceBusQueue = "servicebus-queue";

	}

	public class ApplicationConfigConnectionStrings
	{
		
		public string ServiceBus { get; set; }
	
	}

	public class ApplicationConfigServiceBus
	{

		public string Unsubscribe { get; set; } = "unsubscribe";

		public string ChangePlan { get; set; } = "changeplan";

		public string ChangeQuantity { get; set; } = "changequantity";

		public string Suspend { get; set; } = "suspend";

		public string Reinstate { get; set; } = "reinstate";

		public string Informational { get; set; } = "informational";

	}
}