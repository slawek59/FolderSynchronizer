using Serilog;

namespace FolderSynchronizer
{
	internal class Program
	{
		static void Main(string[] args)
		{
			var synchronizerInfo = new SynchronizerInfo(args);
			LogManager.Initialize(synchronizerInfo.LogFilePath);
			Log.Information("Folder Synchronizer starts.");

			Log.Information("Create SynchronizerInfo and load the config data.");

			var folderSynchronizer = new FolderSynchronizer(synchronizerInfo);

			var isRunning = true;

			Console.CancelKeyPress += (s, e) =>
			{
				e.Cancel = true;
				isRunning = false;
			};

			while (isRunning)
			{
				folderSynchronizer.Sync();

				Log.Information("scanning...");

				Thread.Sleep(synchronizerInfo.SyncIntervalInMs);
			}
			
			LogManager.Close();
		}
	}
}
