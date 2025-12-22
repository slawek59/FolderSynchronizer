using Serilog;

namespace FolderSynchronizer
{
	internal class Program
	{
		static void Main(string[] args)
		{
			LogManager.Initialize();
			Log.Information("Folder Synchronizer starts.");

			Log.Information("Create SynchronizerInfo and load the config data.");

			var synchronizerInfo = new SynchronizerInfo(args);

			Log.Information("Get Sync Info from args");

			var folderSynchronizer = new FolderSynchronizer(synchronizerInfo);
			// SetupConfiguration() - sets up sync interval, source path, replica path, log path
			// 1. define source directory
			// 2. define targer directory
			
			///TODO maybe instead of delete + copy => use a renaming mathod

			while (true)
			{
				folderSynchronizer.Sync();

				Log.Information("scanning...");

				Thread.Sleep(synchronizerInfo.SyncIntervalInSec);
			}
		}
	}
}
