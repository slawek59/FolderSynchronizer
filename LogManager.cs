using Serilog;
using Serilog.Events;

namespace FolderSynchronizer
{
	public static class LogManager
	{
		public static void Initialize(string logPath)
		{

			string logLevelConfig = "Information";

			string logDirectory = logPath;

			if (!Directory.Exists(logDirectory))
			{
				Directory.CreateDirectory(logDirectory);
			}

			var logFilePath = Path.Combine(logDirectory, $"log_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt");

			LogEventLevel logLevel = Enum.TryParse(logLevelConfig, true, out LogEventLevel parsedLogLevel) ? parsedLogLevel : LogEventLevel.Information;

			Log.Logger = new LoggerConfiguration()
				.MinimumLevel.Is(logLevel)
				.WriteTo.Console()
				.WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day)
				.CreateLogger();
		}

		public static void Close()
		{
			Log.Information("Closing logger and flushing logs.");
			Log.CloseAndFlush();
		}
	}
}
