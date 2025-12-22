namespace FolderSynchronizer
{
	internal class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Folder Synchronizer starts.");

			/// TODO those values below will be provided via command line arguments
			//var sourcePath = @"C:\Users\wassl\OneDrive\Pulpit\SLAWOMIR\MOJE\Dokumenty";
			//var destPath = @"C:\Users\wassl\OneDrive\Pulpit\SLAWOMIR\MOJE\replica";
			var sourcePath = @"C:\Users\wassl\source\repos\FolderSynchronizer\bin\Debug\net8.0\fileSync\source";
			var destPath = @"C:\Users\wassl\source\repos\FolderSynchronizer\bin\Debug\net8.0\fileSync\replica";
			var syncInterval = 1000;

			Console.WriteLine("Create SynchronizerInfo");
			var synchronizerInfo = new SynchronizerInfo(sourcePath, destPath, syncInterval);
			Console.WriteLine("Get Sync Info from args");
			synchronizerInfo.GetSyncInfoFromArgs();


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
				Sync();
				Console.WriteLine("next iteration");
				Thread.Sleep(2000);
			}

			void Sync()
			{
				/// TODO - handle exceptions: no source and invalid paths
				/// TODO implement logger
				/// TODO implement single responsibility

				var sourceDirectoryInfo = new DirectoryInfo(synchronizerInfo.SourcePath);

				var replicaDirectoryInfo = new DirectoryInfo(synchronizerInfo.ReplicaPath);

				var sourcePath = synchronizerInfo.SourcePath;
				var replicaPath = synchronizerInfo.ReplicaPath;

				if (!Directory.Exists(replicaPath))
				{
					Directory.CreateDirectory(replicaPath);
				}

				//extract method:
				Dictionary<string, FileInfo> sourceFiles = sourceDirectoryInfo.GetFiles("*", SearchOption.AllDirectories).ToDictionary(
					f => Path.GetRelativePath(sourcePath, f.FullName),
					f => f);

				Dictionary<string, FileInfo> replicaFiles = replicaDirectoryInfo.GetFiles("*", SearchOption.AllDirectories).ToDictionary(
					f => Path.GetRelativePath(replicaPath, f.FullName),
					f => f);

				var allSourceDirectories = sourceDirectoryInfo.GetDirectories("*", SearchOption.AllDirectories);
				var allReplicaDirectories = replicaDirectoryInfo.GetDirectories("*", SearchOption.AllDirectories);

				CopyAndUpdate(sourceFiles, replicaFiles, allSourceDirectories);
				DeleteFiles(sourceFiles, replicaFiles);
				DeleteDirectories(allSourceDirectories, allReplicaDirectories);

				void CopyAndUpdate(Dictionary<string, FileInfo> sourceFiles, Dictionary<string, FileInfo> replicaFiles, DirectoryInfo[] allSourceDirectories)
				{
					/// TODO question: abstract methods for copy dir | copy file OR leave it as it is
					foreach (var directory in allSourceDirectories)
					{
						var directoryPath = directory.FullName.Substring(synchronizerInfo.SourcePath.Length + 1);
						var pathToCreate = Path.Combine(synchronizerInfo.ReplicaPath, directoryPath);

						if (!Directory.Exists(pathToCreate))
						{
							Directory.CreateDirectory(pathToCreate);
						}
					}

					///TODO - dir update should not delete and copy - dir may have files inside => dir should be just renamed

					foreach (var file in sourceFiles)
					{

						var path = Path.Combine(synchronizerInfo.ReplicaPath, file.Key);

						if (!replicaFiles.ContainsKey(file.Key))
						{
							Console.WriteLine($"Creating directory for.");
							Directory.CreateDirectory(Path.Combine(synchronizerInfo.ReplicaPath, file.Value.Directory.Name));
						}
						if (!File.Exists(path))
						{
							Console.WriteLine($"Copying file: {file.Key}");
							CopyFile(file, path);
						}
					}
				}

				void DeleteFiles(Dictionary<string, FileInfo> sourceFiles, Dictionary<string, FileInfo> replicaFiles)
				{
					foreach (var file in replicaFiles)
					{
						var sourceFilePath = Path.Combine(synchronizerInfo.SourcePath, file.Key);
						var replicaFilePath = Path.Combine(synchronizerInfo.ReplicaPath, file.Key);

						if (!File.Exists(sourceFilePath))
						{
							File.Delete(replicaFilePath);
						}
					}
				}


				void DeleteDirectories(DirectoryInfo[] allSourceDirectories, DirectoryInfo[] allReplicaDirectories)
				{
					foreach (var directoryToDelete in allReplicaDirectories)
					{
						var relativePath = directoryToDelete.FullName.Substring(synchronizerInfo.SourcePath.Length + 1);
						var path = Path.Combine(synchronizerInfo.SourcePath, relativePath.TrimStart(Path.DirectorySeparatorChar));

						var doesDirectoryExistInSource = Directory.Exists(path);
						var needsToBeDeleted = Directory.Exists(directoryToDelete.FullName); 

						if (!doesDirectoryExistInSource && needsToBeDeleted)/// TODO delete after checking if dir in replica is empty
						{
							Directory.Delete(Path.Combine(synchronizerInfo.ReplicaPath, directoryToDelete.FullName), true);
						}
					}
				}

				/// TODO it will also have to handle name change of a file -> now it creates a new file and deletes the old one (but it works xd)

				void CopyFile(KeyValuePair<string, FileInfo> file, string path)
				{
					/// TODO implement copying only when the file does not exist or is different => use mechanisms / libraries that allow comparing files MD5? => program should not modify/copy files if source and replica have both the same files
					var destFilePath = Path.Combine(path);
					var sourceFilePath = Path.Combine(file.Value.FullName);

					Console.WriteLine($"Copying file: {file.Key} from {sourceFilePath} to {destFilePath}.");

					File.Copy(sourceFilePath, destFilePath, true);/// TODO display whole path to console
				}
				/// TODO get logging from my framework - maybe - bro's version -> vid2-17:07
			}


		}
	}
	public class SynchronizerInfo
	{

		public string SourcePath { get; set; }
		public string ReplicaPath { get; set; }
		public int SyncIntervalInSec { get; set; }
		public SynchronizerInfo(string sourcePath, string replicaPath, int syncIntervalInSec)
		{
			SourcePath = sourcePath;
			ReplicaPath = replicaPath;
			SyncIntervalInSec = syncIntervalInSec;
		}

		public void GetSyncInfoFromArgs()
		{

		}


	}
}

/// TODO implement path manipulation class / method