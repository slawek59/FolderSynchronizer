using FolderSynchronizer.Models;

namespace FolderSynchronizer.Core
{
	public class Scanner
	{
		public ScanResult Scan(SynchronizerInfo synchronizerInfo)
		{
			var sourcePath = synchronizerInfo.SourcePath;
			var replicaPath = synchronizerInfo.ReplicaPath;

			var sourceDirectoryInfo = new DirectoryInfo(synchronizerInfo.SourcePath);
			var replicaDirectoryInfo = new DirectoryInfo(synchronizerInfo.ReplicaPath);


			return new ScanResult
			{
				SourceFiles = sourceDirectoryInfo.GetFiles("*", SearchOption.AllDirectories).ToDictionary(
				f => Path.GetRelativePath(sourcePath, f.FullName),
				f => f),

				ReplicaFiles = replicaDirectoryInfo.GetFiles("*", SearchOption.AllDirectories).ToDictionary(
				f => Path.GetRelativePath(replicaPath, f.FullName),
				f => f),

				AllSourceDirectories = sourceDirectoryInfo.GetDirectories("*", SearchOption.AllDirectories),


				AllReplicaDirectories = replicaDirectoryInfo.GetDirectories("*", SearchOption.AllDirectories)
			};
		}
	}
}
