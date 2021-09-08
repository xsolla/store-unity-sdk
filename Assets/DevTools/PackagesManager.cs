using System.IO;
using System.Linq;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using UnityEditor;
using UnityEngine;

namespace Xsolla
{
	public static class PackagesManager
	{
		[MenuItem("Dev Tools/Export package")]
		public static void ExportPackageDev()
		{
			var tempDir = Application.dataPath.Replace("Assets", "Temp");
			tempDir = Path.Combine(tempDir, "xsolla-commerce-sdk");
			if (!Directory.Exists(tempDir))
				Directory.CreateDirectory(tempDir);

			var originPackagePath = Path.Combine(tempDir, "origin-package.unitypackage");
			ExportOriginPackage(originPackagePath);

			var contentDir = Path.Combine(tempDir, "package-content");
			Directory.CreateDirectory(contentDir);
			ExtractTGZ(originPackagePath, contentDir);

			var dependenciesSourcePath = Path.Combine(Application.dataPath, "DevTools", "package-dependencies.json");
			var dependenciesDestPath = Path.Combine(contentDir, "packagemanagermanifest");
			Directory.CreateDirectory(dependenciesDestPath);
			dependenciesDestPath = Path.Combine(dependenciesDestPath, "asset");
			File.Copy(dependenciesSourcePath, dependenciesDestPath);

			var exportPackagePath = Application.dataPath.Replace("Assets", string.Empty);
			exportPackagePath = Path.Combine(exportPackagePath, "xsolla-commerce-sdk.unitypackage");
			CreateTarGZ(exportPackagePath, contentDir);

			Directory.Delete(tempDir, true);
			Debug.Log($"[PackagesManager] Package exported successful. Path: {exportPackagePath}");
		}

		private static void ExportOriginPackage(string packagePath)
		{
			var filter = string.Empty;
			var paths = new[]{
				"Assets/Xsolla"
			};

			var assets = AssetDatabase.FindAssets(filter, paths)
				.Select(AssetDatabase.GUIDToAssetPath)
				.ToArray();

			AssetDatabase.ExportPackage(assets, packagePath);
		}

		private static void ExtractTGZ(string gzArchiveName, string destFolder)
		{
			var inStream = File.OpenRead(gzArchiveName);
			var gzipStream = new GZipInputStream(inStream);

			var tarArchive = TarArchive.CreateInputTarArchive(gzipStream);
			tarArchive.ExtractContents(destFolder);
			tarArchive.Close();

			gzipStream.Close();
			inStream.Close();
		}

		private static void CreateTarGZ(string tgzFilename, string sourceDirectory)
		{
			var outStream = File.Create(tgzFilename);
			var gzoStream = new GZipOutputStream(outStream);
			var tarArchive = TarArchive.CreateOutputTarArchive(gzoStream);

			// Note that the RootPath is currently case sensitive and must be forward slashes e.g. "c:/temp"
			// and must not end with a slash, otherwise cuts off first char of filename
			// This is scheduled for fix in next release
			tarArchive.RootPath = sourceDirectory.Replace('\\', '/');
			if (tarArchive.RootPath.EndsWith("/"))
				tarArchive.RootPath = tarArchive.RootPath.Remove(tarArchive.RootPath.Length - 1);

			AddDirectoryFilesToTar(tarArchive, sourceDirectory, true);
			tarArchive.Close();
		}

		private static void AddDirectoryFilesToTar(TarArchive tarArchive, string sourceDirectory, bool recurse)
		{
			// Optionally, write an entry for the directory itself.
			// Specify false for recursion here if we will add the directory's files individually.
			var tarEntry = TarEntry.CreateEntryFromFile(sourceDirectory);
			tarArchive.WriteEntry(tarEntry, false);

			// Write each file to the tar.
			var filenames = Directory.GetFiles(sourceDirectory);
			foreach (var filename in filenames)
			{
				tarEntry = TarEntry.CreateEntryFromFile(filename);
				tarArchive.WriteEntry(tarEntry, true);
			}

			if (recurse)
			{
				var directories = Directory.GetDirectories(sourceDirectory);
				foreach (var directory in directories)
					AddDirectoryFilesToTar(tarArchive, directory, recurse);
			}
		}
	}
}