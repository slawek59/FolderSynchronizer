namespace FolderSynchronizer
{
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