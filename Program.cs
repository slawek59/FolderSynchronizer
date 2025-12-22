using Serilog;

namespace FolderSynchronizer
{
	internal class Program
	{
		static void Main(string[] args)
		{
			LogManager.Initialize();
			Log.Information("Folder Synchronizer starts.");


			/// TODO those values below will be provided via command line arguments
			//var sourcePath = @"C:\Users\wassl\OneDrive\Pulpit\SLAWOMIR\MOJE\Dokumenty";
			//var destPath = @"C:\Users\wassl\OneDrive\Pulpit\SLAWOMIR\MOJE\replica";
			//var sourcePath = @"C:\Users\wassl\source\repos\FolderSynchronizer\bin\Debug\net8.0\fileSync\source";
			//var destPath = @"C:\Users\wassl\source\repos\FolderSynchronizer\bin\Debug\net8.0\fileSync\replica";
			//var syncInterval = 1000;

			var sourcePath = @$"{args[0]}";
			var destPath = @$"{args[1]}";
			var syncInterval = int.Parse(args[2]);

			Log.Information("Create SynchronizerInfo");

			var synchronizerInfo = new SynchronizerInfo(sourcePath, destPath, syncInterval);

			Log.Information("Get Sync Info from args");

			synchronizerInfo.GetSyncInfoFromArgs();

			var folderSynchronizer = new FolderSynchronizer(synchronizerInfo);
			/// TODO below: provide use of methods that will reflect the app flow
			// SetupConfiguration() - sets up sync interval, source path, replica path, log path
			// 1. define source directory
			// 2. define targer directory



			// 3. scan source directory
			// 4. for each file in source:
			//		determine if the same file exists in target
			//		if file does not exist in target
			//			copy from source to targer
			//		if file does exist in target
			//			determine if source is newer than target
			//			if it is, copy from source to target - overwrite
			// 4. - do the same for the folders

			// bro is using LINQ - use it as well - show I now this library

			// UpdateTargetDirs(rule, srcFolder) => UpdateTargetFiles(rules, srcFolder); SaveLogToFile(rule) | THIS COULD BE ALL THAT IS PUT IN THE MAIN METHOD

			/// TODO provide a reader that can read config from command line and from files as well
			///TODO maybe instead of delete + copy => use a renaming mathod

			while (true)
			{
				folderSynchronizer.Sync();

				Log.Information("next iteration");

				Thread.Sleep(2000);
			}

			


		}
	}
}

/// TODO implement path manipulation class / method