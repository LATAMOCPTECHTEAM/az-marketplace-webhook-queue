using AZ.Marketplace.Core.Interfaces;
using AZ.Marketplace.Core.Model;
using AZ.Marketplace.Core.QueueImplementations;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

[assembly: FunctionsStartup(typeof(AZ.Marketplace.Functions.Startup))]
namespace AZ.Marketplace.Functions
{
	public class Startup : FunctionsStartup
	{
		public override void Configure(IFunctionsHostBuilder builder)
		{
			var configuration = LoadConfiguration(builder);
			var appConfig = GetAppConfig(configuration);
			var connectionStrings = GetAppConnectionStrings(configuration);

			builder.Services.AddHttpClient();
			Console.WriteLine($"QueueType: {appConfig.QueueType}");

			if (appConfig.QueueType == ApplicationConfigType.ServiceBusTopic)
			{
				var connectionString = connectionStrings.ServiceBus;

				builder.Services.AddSingleton<IQueueWrapper>(new ServiceBusTopicWrapper(
					unsubscribeTopic: new TopicClient(connectionString, appConfig.ServiceBus.Unsubscribe),
					changePlanTopic: new TopicClient(connectionString, appConfig.ServiceBus.ChangePlan),
					changeQuantityTopic: new TopicClient(connectionString, appConfig.ServiceBus.ChangeQuantity),
					suspendTopic: new TopicClient(connectionString, appConfig.ServiceBus.Suspend),
					reinstateTopic: new TopicClient(connectionString, appConfig.ServiceBus.Reinstate),
					informationalTopic: new TopicClient(connectionString, appConfig.ServiceBus.Informational)
				));
			}
			else if (appConfig.QueueType == ApplicationConfigType.ServiceBusQueue)
			{
				var connectionString = connectionStrings.ServiceBus;

				builder.Services.AddSingleton<IQueueWrapper>(new ServiceBusQueueWrapper(
					unsubscribeQueue: new QueueClient(connectionString, appConfig.ServiceBus.Unsubscribe),
					changePlanQueue: new QueueClient(connectionString, appConfig.ServiceBus.ChangePlan),
					changeQuantityQueue: new QueueClient(connectionString, appConfig.ServiceBus.ChangeQuantity),
					suspendQueue: new QueueClient(connectionString, appConfig.ServiceBus.Suspend),
					reinstateQueue: new QueueClient(connectionString, appConfig.ServiceBus.Reinstate),
					informationalQueue: new QueueClient(connectionString, appConfig.ServiceBus.Informational)
				));
			}

			builder.Services.AddApplicationInsightsTelemetry();

		}

		private ApplicationConfig GetAppConfig(IConfigurationRoot config)
		{
			var appConfig = new ApplicationConfig();
			config.GetSection("Values").Bind(appConfig);
			return appConfig;
		}

		private ApplicationConfigConnectionStrings GetAppConnectionStrings(IConfigurationRoot config)
		{
			var connectionStrings = new ApplicationConfigConnectionStrings();
			config.GetSection("ConnectionStrings").Bind(connectionStrings);
			return connectionStrings;
		}

		private IConfigurationRoot LoadConfiguration(IFunctionsHostBuilder builder)
		{
			var executioncontextoptions = builder.Services.BuildServiceProvider()
			.GetService<IOptions<ExecutionContextOptions>>().Value;
			var currentDirectory = executioncontextoptions.AppDirectory;

			var config = new ConfigurationBuilder()
				.SetBasePath(currentDirectory)
				.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true) // <- This gives you access to your application settings in your local development environment
				.AddEnvironmentVariables() // <- This is what actually gets you the application settings in Azure
				.Build();

			return config;
		}


	}
}
