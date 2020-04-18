using System;
using System.Collections.Generic;
using System.Text;

namespace AZ.Marketplace.Core.Model
{
	public static class WebhookActionType
	{

		public const string Unsubscribe = "Unsubscribe";

		public const string ChangePlan = "ChangePlan";

		public const string ChangeQuantity = "ChangeQuantity";

		public const string Suspend = "Suspend";

		public const string Reinstate = "Reinstate";

	}
}