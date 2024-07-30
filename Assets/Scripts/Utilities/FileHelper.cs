using System.IO;

namespace CircuitSimulation.Utilities
{
	public static class FileHelper
	{
		public static string EnsureUniqueFileName(string _originalPath)
		{
			string originalFileName = Path.GetFileName(_originalPath);
			string originalFileNameWithoutExtension = Path.GetFileNameWithoutExtension(_originalPath);
			string extension = Path.GetExtension(_originalPath);
			string uniquePath = _originalPath;

			int duplicates = 0;

			while (File.Exists(uniquePath))
			{
				duplicates++;
				string uniqueFileName = $"{originalFileNameWithoutExtension}_{duplicates}{extension}";
				uniquePath = _originalPath.Replace(originalFileName, uniqueFileName);
			}

			return uniquePath;
		}

		public static string EnsureUniqueDirectoryName(string _originalPath)
		{
			string uniquePath = _originalPath;

			int duplicates = 0;
			while (Directory.Exists(uniquePath))
			{
				duplicates++;
				uniquePath = _originalPath + "_" + duplicates;
			}

			return uniquePath;
		}

		public static void CopyDirectory(string _sourceDirectory, string _destinationDirectory, bool _recursive)
		{
			var dir = new DirectoryInfo(_sourceDirectory);

			if (!dir.Exists)
			{
				throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");
			}

			Directory.CreateDirectory(_destinationDirectory);

			foreach (var file in dir.GetFiles())
			{
				string targetFilePath = Path.Combine(_destinationDirectory, file.Name);
				file.CopyTo(targetFilePath);
			}

			if (_recursive)
			{
				DirectoryInfo[] dirs = dir.GetDirectories();
				foreach (var subDir in dirs)
				{
					string newDestinationDir = Path.Combine(_destinationDirectory, subDir.Name);
					CopyDirectory(subDir.FullName, newDestinationDir, true);
				}
			}
		}
	}
}