#if UNITY_ANDROID
using System;
using System.IO;
using System.Xml;

namespace Xsolla.DevTools
{
	public static class ManifestProcessor
	{
		public static void Process()
		{
			var manifestPath = ProvideManifest();
			if (manifestPath != null)
				PatchManifest(manifestPath);
		}

		private static string ProvideManifest()
		{
			const string fileName = "AndroidManifest.xml";

			var workDir = Utils.GetWorkDir();
			var sourceFilePath = Path.Combine(workDir, "Templates", fileName);

			var targetDir = Utils.GetTargetDir();
			var targetFilePath = Path.Combine(targetDir, fileName);
			Utils.CopyFileIfNotExists(sourceFilePath, targetFilePath);
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

			// var providerNode = applicationNode.SelectSingleNode("provider") as XmlElement;
			// if (providerNode == null)
			// 	providerNode = CreateXmlNode("provider", applicationNode, xmlDoc);
			//
			// if (providerNode?.Attributes == null)
			// 	throw new Exception("Can't find 'provider' node in AndroidManifest.xml");
			//
			// attribute = providerNode.Attributes["android:name"];
			// if (attribute == null)
			// 	CreateXmlAttribute("android", "name", "androidx.startup.InitializationProvider", providerNode);
			//
			// attribute = providerNode.Attributes["android:authorities"];
			// if (attribute == null)
			// 	CreateXmlAttribute("android", "authorities", "${applicationId}.androidx-startup", providerNode);
			//
			// attribute = providerNode.Attributes["tools:node"];
			// if (attribute == null)
			// 	CreateXmlAttribute("tools", "node", "remove", providerNode);

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
	}
}
#endif