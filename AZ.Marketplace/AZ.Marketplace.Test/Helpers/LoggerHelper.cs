using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace AZ.Marketplace.Test.Helpers
{
	public static class LoggerHelper
	{
		public static ILogger CreateLogger(LoggerTypes type = LoggerTypes.Null)
		{
			ILogger logger;

			if (type == LoggerTypes.List)
			{
				logger = new ListLogger();
			}
			else
			{
				logger = NullLoggerFactory.Instance.CreateLogger("Null Logger");
			}

			return logger;
		}
	}

	public class ListLogger : ILogger
	{
		public IList<string> Logs;

		public IDisposable BeginScope<TState>(TState state) => NullScope.Instance;

		public bool IsEnabled(LogLevel logLevel) => false;

		public ListLogger()
		{
			this.Logs = new List<string>();
		}

		public void Log<TState>(LogLevel logLevel,
								EventId eventId,
								TState state,
								Exception exception,
								Func<TState, Exception, string> formatter)
		{
			string message = formatter(state, exception);
			this.Logs.Add(message);
		}
	}

	public enum LoggerTypes
	{
		Null,
		List
	}

	public class NullScope : IDisposable
	{
		public static NullScope Instance { get; } = new NullScope();

		private NullScope() { }

		public void Dispose() { }
	}
}