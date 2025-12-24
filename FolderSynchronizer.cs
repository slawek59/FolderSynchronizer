using Serilog;

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
			var sourcePath = _synchronizerInfo.SourcePath;
			var replicaPath = _synchronizerInfo.ReplicaPath;

			if (!Directory.Exists(replicaPath))
			{
				Directory.CreateDirectory(replicaPath);
			}

			var scanner = new Scanner();

			var scanResult = scanner.Scan(_synchronizerInfo);

			CopyAndUpdate(scanResult.SourceFiles, scanResult.ReplicaFiles, scanResult.AllSourceDirectories);
			CreateEmptyDirectories(scanResult.AllSourceDirectories);

			scanResult = scanner.Scan(_synchronizerInfo);

			DeleteFiles(scanResult.ReplicaFiles);

			scanResult = scanner.Scan(_synchronizerInfo);

			DeleteDirectories(scanResult.AllReplicaDirectories);
		}

		private void CopyAndUpdate(Dictionary<string, FileInfo> sourceFiles, Dictionary<string, FileInfo> replicaFiles, DirectoryInfo[] allSourceDirectories)
		{
			foreach (var file in sourceFiles)
			{
				var relativePath = file.Key;
				var sourceFIle = file.Value;

				if (!sourceFIle.Exists)
				{
					Log.Warning($"Source file disappeared, skipping: {relativePath}");
					continue;
				}

				var replicaFilePath = Path.Combine(_synchronizerInfo.ReplicaPath, relativePath);

				if (!replicaFiles.TryGetValue(relativePath, out var replicaFile))
				{
					Log.Information($"Copying file: {relativePath}");
					CopyFile(sourceFIle, replicaFilePath);
				}
				else if (!FileComparer.AreFilesSame(sourceFIle, replicaFile))
				{
					Log.Information($"Updating file: {relativePath}");
					CopyFile(sourceFIle, replicaFilePath);
				}
			}
		}

		private void CopyFile(FileInfo sourceFile, string replicaFilePath)
		{
			try
			{
				var targetDirectory = Path.GetDirectoryName(replicaFilePath);

				if (!sourceFile.Exists)
				{
					return;
				}

				if (!Directory.Exists(targetDirectory))
				{
					Log.Information($"Creating directory for: {targetDirectory}");

					Directory.CreateDirectory(targetDirectory);
				}

				File.Copy(sourceFile.FullName, replicaFilePath, true);
			}
			catch (DirectoryNotFoundException ex)
			{
				Log.Warning($"Directory not found while copying: {sourceFile.FullName}. {ex.Message}");
			}
			catch (IOException ex)
			{
				Log.Warning($"IOException while copying: {sourceFile.FullName}. {ex.Message}");
			}
		}

		private void CreateEmptyDirectories(DirectoryInfo[] allSourceDirectories)
		{
			foreach (var sourceDir in allSourceDirectories)
			{
				var relativePath = Path.GetRelativePath(
					_synchronizerInfo.SourcePath,
					sourceDir.FullName);

				var replicaDirPath = Path.Combine(
					_synchronizerInfo.ReplicaPath,
					relativePath);

				if (!Directory.Exists(replicaDirPath))
				{
					Log.Information($"Creating empty directory: {replicaDirPath}");
					Directory.CreateDirectory(replicaDirPath);
				}
			}
		}

		private void DeleteFiles(Dictionary<string, FileInfo> replicaFiles)
		{
			foreach (var file in replicaFiles)
			{
				var sourceFilePath = Path.Combine(_synchronizerInfo.SourcePath, file.Key);
				var replicaFilePath = Path.Combine(_synchronizerInfo.ReplicaPath, file.Key);

				if (!File.Exists(sourceFilePath))
				{
					try
					{
						Log.Information($"Deleting file: {replicaFilePath}.");
						File.Delete(replicaFilePath);
					}
					catch (IOException ex)
					{
						Log.Warning($"IOException while deleting: {ex.Message}");
					}
				}
			}
		}

		private void DeleteDirectories(DirectoryInfo[] allReplicaDirectories)
		{
			foreach (var directoryToDelete in allReplicaDirectories.OrderByDescending(d => d.FullName.Length))
			{
				var relativePath = Path.GetRelativePath(_synchronizerInfo.ReplicaPath, directoryToDelete.FullName);
				var sourceDirPath = Path.Combine(_synchronizerInfo.SourcePath, relativePath);
				var needsToBeDeleted = Directory.Exists(directoryToDelete.FullName);

				try
				{
					if (!Directory.Exists(sourceDirPath) && needsToBeDeleted)
					{
						Log.Information($"Deleting directory: {directoryToDelete.FullName}");
						Directory.Delete(directoryToDelete.FullName, true);
					}
				}
				catch (Exception ex)
				{
					Log.Warning($"Could not delete the directory {ex.Message}.");
				}
			}
		}
	}
}

