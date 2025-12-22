namespace FolderSynchronizer
{
	public class FolderSynchronizer
	{
		private readonly SynchronizerInfo _synchronizerInfo;

		public FolderSynchronizer(SynchronizerInfo synchronizerInfo)
		{
			_synchronizerInfo = synchronizerInfo;
		}

		public void Sync()
		{
			/// TODO - handle exceptions: no source and invalid paths
			/// TODO implement logger
			/// TODO implement single responsibility
			/// 


			var sourceDirectoryInfo = new DirectoryInfo(_synchronizerInfo.SourcePath);

			var replicaDirectoryInfo = new DirectoryInfo(_synchronizerInfo.ReplicaPath);

			var sourcePath = _synchronizerInfo.SourcePath;
			var replicaPath = _synchronizerInfo.ReplicaPath;

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
		}
		private void CopyAndUpdate(Dictionary<string, FileInfo> sourceFiles, Dictionary<string, FileInfo> replicaFiles, DirectoryInfo[] allSourceDirectories)
		{
			/// TODO question: abstract methods for copy dir | copy file OR leave it as it is
			foreach (var directory in allSourceDirectories)
			{
				var directoryPath = directory.FullName.Substring(_synchronizerInfo.SourcePath.Length + 1);
				var pathToCreate = Path.Combine(_synchronizerInfo.ReplicaPath, directoryPath);

				if (!Directory.Exists(pathToCreate))
				{
					Directory.CreateDirectory(pathToCreate);
				}
			}

			///TODO - dir update should not delete and copy - dir may have files inside => dir should be just renamed

			foreach (var file in sourceFiles)
			{

				var path = Path.Combine(_synchronizerInfo.ReplicaPath, file.Key);

				if (!replicaFiles.ContainsKey(file.Key))
				{
					Console.WriteLine($"Creating directory for.");
					Directory.CreateDirectory(Path.Combine(_synchronizerInfo.ReplicaPath, file.Value.Directory.Name));
				}
				if (!File.Exists(path))
				{
					Console.WriteLine($"Copying file: {file.Key}");
					CopyFile(file, path);
				}
			}
		}

		private void DeleteFiles(Dictionary<string, FileInfo> sourceFiles, Dictionary<string, FileInfo> replicaFiles)
		{
			foreach (var file in replicaFiles)
			{
				var sourceFilePath = Path.Combine(_synchronizerInfo.SourcePath, file.Key);
				var replicaFilePath = Path.Combine(_synchronizerInfo.ReplicaPath, file.Key);

				if (!File.Exists(sourceFilePath))
				{
					File.Delete(replicaFilePath);
				}
			}
		}


		private void DeleteDirectories(DirectoryInfo[] allSourceDirectories, DirectoryInfo[] allReplicaDirectories)
		{
			foreach (var directoryToDelete in allReplicaDirectories)
			{
				var relativePath = directoryToDelete.FullName.Substring(_synchronizerInfo.SourcePath.Length + 1);
				var path = Path.Combine(_synchronizerInfo.SourcePath, relativePath.TrimStart(Path.DirectorySeparatorChar));

				var doesDirectoryExistInSource = Directory.Exists(path);
				var needsToBeDeleted = Directory.Exists(directoryToDelete.FullName);

				if (!doesDirectoryExistInSource && needsToBeDeleted)/// TODO delete after checking if dir in replica is empty
				{
					Directory.Delete(Path.Combine(_synchronizerInfo.ReplicaPath, directoryToDelete.FullName), true);
				}
			}
		}

		/// TODO it will also have to handle name change of a file -> now it creates a new file and deletes the old one (but it works xd)

		private void CopyFile(KeyValuePair<string, FileInfo> file, string path)
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

