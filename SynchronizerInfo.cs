namespace FolderSynchronizer
{
	public class SynchronizerInfo
	{
		public string SourcePath { get; set; }
		public string ReplicaPath { get; set; }
		public string LogFilePath { get; set; }
		public int SyncIntervalInMs { get; set; }

		public SynchronizerInfo(string[] args)
		{

			SourcePath = @$"{args[0]}";
			ReplicaPath = @$"{args[1]}";
			LogFilePath = @$"{args[2]}";
			SyncIntervalInMs = int.Parse(args[3]);
		}
	}
}
