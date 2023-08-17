using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Xsolla
{
	public class AndroidFilesProcessor : IPreprocessBuildWithReport, IPostprocessBuildWithReport
	{
		private static List<string> CreatedFilePaths;

		public int callbackOrder => -1000;

		public void OnPreprocessBuild(BuildReport report)
		{
			CreatedFilePaths = new List<string>();

			ProvideBaseTemplate();

			var launcherTemplatePath = ProvideLauncherTemplate();
			if (launcherTemplatePath != null)
				PatchLauncherTemplate(launcherTemplatePath);

			var mainTemplatePath = ProvideMainTemplate();
			if (mainTemplatePath != null)
				PatchMainTemplate(mainTemplatePath);

			var manifestPath = ProvideManifest();
			if (manifestPath != null)
				PatchManifest(manifestPath);
		}

		public void OnPostprocessBuild(BuildReport report)
		{
			DeleteCreateFiles();
		}

		private static void ProvideBaseTemplate()
		{
			var sourceFileName = GetBaseSourceFileName();
			if (sourceFileName == null)
				return;

			var sourceFilePath = GetSourceFilePath(sourceFileName);
			var targetFilePath = GetTargetFilePath("baseProjectTemplate.gradle");
			CopyFileIfNotExists(sourceFilePath, targetFilePath);
		}

		private static string GetBaseSourceFileName()
		{
#if UNITY_2022_3_OR_NEWER
			return null;
#else
			return "baseProjectTemplate-2019.gradle";
#endif
		}

		private static string ProvideLauncherTemplate()
		{
			var sourceFileName = GetLauncherSourceFileName();
			if (sourceFileName == null)
				return null;

			var sourceFilePath = GetSourceFilePath(sourceFileName);
			var targetFilePath = GetTargetFilePath("launcherTemplate.gradle");
			CopyFileIfNotExists(sourceFilePath, targetFilePath);
			return targetFilePath;
		}

		private static string GetLauncherSourceFileName()
		{
#if UNITY_2022_3_OR_NEWER
			return "launcherTemplate-2022.gradle";
#elif UNITY_2021_3_OR_NEWER
			return "launcherTemplate-2021.gradle";
#elif UNITY_2020_3_OR_NEWER
			return "launcherTemplate-2020.gradle";
#else
			return "launcherTemplate-2019.gradle";
#endif
		}

		private static void PatchLauncherTemplate(string filePath)
		{
			var patchText = GetGradlePatchText();
			var originText = File.ReadAllText(filePath);
			if (originText.Contains(patchText))
				return;

			var finalText = originText + patchText;
			File.WriteAllText(filePath, finalText);
		}

		private static string ProvideMainTemplate()
		{
			var sourceFileName = GetMainSourceFileName();
			if (sourceFileName == null)
				return null;

			var sourceFilePath = GetSourceFilePath(sourceFileName);
			var targetFilePath = GetTargetFilePath("mainTemplate.gradle");
			CopyFileIfNotExists(sourceFilePath, targetFilePath);
			return targetFilePath;
		}

		private static string GetMainSourceFileName()
		{
#if UNITY_2022_3_OR_NEWER
			return "mainTemplate-2022.gradle";
#else
			return null;
#endif
		}

		private static void PatchMainTemplate(string filePath)
		{
			var patchText = GetGradlePatchText();
			var originText = File.ReadAllText(filePath);
			if (originText.Contains(patchText))
				return;

			var finalText = originText + patchText;
			File.WriteAllText(filePath, finalText);
		}

		private static string GetGradlePatchText()
		{
			var patchFilePath = GetSourceFilePath("gradle-patch.txt");
			return File.ReadAllText(patchFilePath);
		}

		private static string ProvideManifest()
		{
			const string fileName = "AndroidManifest.xml";
			var sourceFilePath = GetSourceFilePath(fileName);
			var targetFilePath = GetTargetFilePath(fileName);
			CopyFileIfNotExists(sourceFilePath, targetFilePath);
			return targetFilePath;
		}

		private static void PatchManifest(string filePath)
		{
			var xmlDoc = new XmlDocument();
			xmlDoc.Load(filePath);

			var root = xmlDoc.DocumentElement;
			var applicationNode = root?.SelectSingleNode("application");
			if (applicationNode == null)
				throw new Exception("Can't find 'application' node in AndroidManifest.xml");

			var unityActivityNode = GetUnityActivityNode(applicationNode);
			if (unityActivityNode == null)
				throw new Exception("Can't find 'UnityPlayerActivity' node in AndroidManifest.xml");

			var attribute = unityActivityNode.Attributes["android:exported"];
			if (attribute == null)
				CreateXmlAttribute("android", "exported", "true", unityActivityNode);

			var providerNode = applicationNode.SelectSingleNode("provider") as XmlElement;
			if (providerNode == null)
				providerNode = CreateXmlNode("provider", applicationNode, xmlDoc);

			if (providerNode?.Attributes == null)
				throw new Exception("Can't find 'provider' node in AndroidManifest.xml");

			attribute = providerNode.Attributes["android:name"];
			if (attribute == null)
				CreateXmlAttribute("android", "name", "androidx.startup.InitializationProvider", providerNode);

			attribute = providerNode.Attributes["android:authorities"];
			if (attribute == null)
				CreateXmlAttribute("android", "authorities", "${applicationId}.androidx-startup", providerNode);

			attribute = providerNode.Attributes["tools:node"];
			if (attribute == null)
				CreateXmlAttribute("tools", "node", "remove", providerNode);

			xmlDoc.Save(filePath);
		}

		private static XmlElement CreateXmlNode(string name, XmlNode parent, XmlDocument xmlDoc)
		{
			var node = xmlDoc.CreateElement(name);
			parent.AppendChild(node);
			return node;
		}

		private static XmlElement GetUnityActivityNode(XmlNode parent)
		{
			var children = parent.SelectNodes("activity");
			if (children == null)
				throw new Exception("Can't find 'activity' nodes in AndroidManifest.xml");

			var xmlNamespace = parent.OwnerDocument.DocumentElement.GetNamespaceOfPrefix("android");
			foreach (XmlElement child in children)
			{
				var attribute = child.GetAttributeNode("name", xmlNamespace);
				if (attribute != null && attribute.Value == "com.unity3d.player.UnityPlayerActivity")
					return child;
			}

			return null;
		}

		private static void CreateXmlAttribute(string prefix, string name, string value, XmlElement node)
		{
			var xmlNamespace = node.OwnerDocument.DocumentElement.GetNamespaceOfPrefix(prefix);
			node.SetAttribute(name, xmlNamespace, value);
		}

		private static void CopyFileIfNotExists(string sourcePath, string targetPath)
		{
			if (!File.Exists(sourcePath))
				throw new FileNotFoundException($"File not found: {sourcePath}");

			if (File.Exists(targetPath))
				return;

			var targetDir = Path.GetDirectoryName(targetPath);
			if (targetDir != null && !Directory.Exists(targetDir))
				Directory.CreateDirectory(targetDir);

			CreatedFilePaths.Add(targetPath);
			File.Copy(sourcePath, targetPath);
		}

		private static string GetTargetFilePath(string fileName)
		{
			return Path.Combine(Application.dataPath, "Plugins", "Android", fileName);
		}

		private static string GetSourceFilePath(string fileName)
		{
			var guids = AssetDatabase.FindAssets($"t:Script {nameof(AndroidFilesProcessor)}");
			if (guids.Length == 0)
				throw new FileNotFoundException($"Can't find {nameof(AndroidFilesProcessor)} script");

			var path = AssetDatabase.GUIDToAssetPath(guids[0]);
			path = path.Replace("Assets", Application.dataPath);
			path = Path.GetDirectoryName(path);

			if (path == null)
				throw new DirectoryNotFoundException("Can't find directory with android file templates");

			return Path.Combine(path, "Templates", fileName);
		}

		private static void DeleteCreateFiles()
		{
			foreach (var filePath in CreatedFilePaths)
			{
				if (File.Exists(filePath))
					File.Delete(filePath);
			}
		}
	}
}