namespace FolderSynchronizer.Models
{
	public class ScanResult
	{
		public Dictionary<string, FileInfo> SourceFiles { get; set; }
		public Dictionary<string, FileInfo> ReplicaFiles { get; set; }
		public DirectoryInfo[] AllSourceDirectories { get; set; }
		public DirectoryInfo[] AllReplicaDirectories { get; set; }

	}
}
