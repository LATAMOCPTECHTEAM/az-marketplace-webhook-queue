using AZ.Marketplace.Core.Interfaces;
using AZ.Marketplace.Core.Model;
using AZ.Marketplace.Core.QueueImplementations;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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

			if (appConfig.QueueType == ApplicationConfigType.ServiceBusTopic)
			{
				var connectionString = connectionStrings.ServiceBus;

				builder.Services.AddSingleton<IQueueWrapper>(new ServiceBusTopicWrapper(
					unsubscribeTopic: new TopicClient(connectionString, appConfig.ServiceBus.UnsubscribeTopic),
					changePlanTopic: new TopicClient(connectionString, appConfig.ServiceBus.ChangePlanTopic),
					changeQuantityTopic: new TopicClient(connectionString, appConfig.ServiceBus.ChangeQuantityTopic),
					suspendTopic: new TopicClient(connectionString, appConfig.ServiceBus.SuspendTopic),
					reinstateTopic: new TopicClient(connectionString, appConfig.ServiceBus.ReinstateTopic),
					informationalTopic: new TopicClient(connectionString, appConfig.ServiceBus.InformationalTopic)
				));
			}
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
