using System.Security.Cryptography;

namespace FolderSynchronizer
{
	public static class FileComparer
	{
		public static bool AreFilesSame(FileInfo file1, FileInfo file2)
		{
			try
			{
				if (!file1.Exists || !file2.Exists)
				{
					return false;
				}

				if (file1.Length != file2.Length)
				{
					return false;
				}

				using var md5 = MD5.Create();
				using var fileStream1 = file1.OpenRead();
				using var fileStream2 = file2.OpenRead();

				var hash1 = md5.ComputeHash(fileStream1);
				var hash2 = md5.ComputeHash(fileStream2);

				return hash1.SequenceEqual(hash2);

			}
			catch (IOException)
			{
				return false;
			}
		}
	}
}
