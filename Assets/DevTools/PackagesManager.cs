using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Xsolla.Core;

namespace Xsolla.DevTools
{
	public static class PackagesManager
	{
		[MenuItem("Dev Tools/Export SDK package", false, 100)]
		public static void ExportSdk()
		{
			var assetGuids = GetGuids("Assets/Xsolla");
			var packagePath = BuildPackagePath("xsolla-commerce-sdk");

			var parameters = new object[] {
				assetGuids,
				packagePath
			};

			ExportPackage(parameters);
		}

		[MenuItem("Dev Tools/Export DEMO package", false, 110)]
		public static void ExportDemo()
		{
			ResetSdkSettings();

			var sdkAssetGuids = GetGuids("Assets/Xsolla");
			var demoAssetGuids = GetGuids("Assets/Xsolla.Demo");
			var resourcesAssetGuids = GetGuids("Assets/Resources");
			
			var assetGuids = sdkAssetGuids
				.Concat(demoAssetGuids)
				.Concat(resourcesAssetGuids)
				.Distinct()
				.ToArray();

			var packagePath = BuildPackagePath("xsolla-commerce-demo");

			var parameters = new object[] {
				assetGuids,
				packagePath
			};

			ExportPackage(parameters);
		}

		[MenuItem("Dev Tools/Reset SDK Settings", false, 120)]
		public static void ResetSdkSettings()
		{
			XsollaSettings.StoreProjectId = Constants.DEFAULT_PROJECT_ID;
			XsollaSettings.LoginId = Constants.DEFAULT_LOGIN_ID;
			XsollaSettings.OAuthClientId = Constants.DEFAULT_OAUTH_CLIENT_ID;
			XsollaSettings.CallbackUrl = string.Empty;
			XsollaSettingsEditor.SaveSettingsAsset();
		}

		private static string BuildPackagePath(string packageName)
		{
			var projectPath = Application.dataPath.Replace("Assets", string.Empty);
			return Path.Combine(projectPath, $"{packageName}_v{PlayerSettings.bundleVersion}.unitypackage");
		}

		private static void ExportPackage(object[] parameters)
		{
			var methods = new List<string> {
				"UnityEditor.PackageUtility.ExportPackageAndPackageManagerManifest"
			};

			TryStaticInvoke(methods, parameters);
		}

		#region from the unity asset store tools package

		private static string[] GetGuids(string rootPath)
		{
			var rootGuid = AssetDatabase.AssetPathToGUID(rootPath);
			var guids = CollectAllChildren(rootGuid, Array.Empty<string>());
			return BuildExportPackageAssetListGuids(guids, true);
		}

		private static string[] BuildExportPackageAssetListGuids(string[] guids, bool dependencies)
		{
			var methodInfo = GetMethodInfo(new List<string> {
				"UnityEditor.PackageUtility.BuildExportPackageItemsList",
				"UnityEditor.AssetServer.BuildExportPackageAssetListAssetsItems"
			}, new[] {
				typeof(string[]),
				typeof(bool)
			});

			var parameters = new object[] {
				guids,
				dependencies
			};

			var objArray = (object[]) methodInfo.Invoke(null, parameters);
			var strArray = new string[objArray.Length];

			var field = methodInfo.ReturnType.GetElementType()?.GetField("guid");
			if (field == null)
				throw new Exception();

			for (var index = 0; index < objArray.Length; ++index)
			{
				var str = (string) field.GetValue(objArray[index]);
				strArray[index] = str;
			}

			return strArray;
		}

		private static string[] CollectAllChildren(string guid, IEnumerable collection)
		{
			return (string[]) TryStaticInvoke(new List<string> {
					"UnityEditor.AssetDatabase.CollectAllChildren",
					"UnityEditor.AssetServer.CollectAllChildren"
				},
				new object[] {
					guid,
					collection
				}
				);
		}

		private static object TryStaticInvoke(List<string> methods, object[] parameters)
		{
			return GetMethodInfo(methods).Invoke(null, parameters);
		}

		private static MethodInfo GetMethodInfo(List<string> methods, Type[] parametersType = null)
		{
			var methodInfo = (MethodInfo) null;
			foreach (var method in methods)
			{
				var chArray = new[] {
					'.'
				};
				var strArray = method.Split(chArray);
				var assembly = Assembly.Load(strArray[0]);
				var name1 = $"{strArray[0]}.{strArray[1]}";
				var name2 = strArray[2];
				var type = assembly.GetType(name1);
				if (type != null)
					methodInfo = parametersType != null ? type.GetMethod(name2, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, parametersType, null) : type.GetMethod(name2, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				if (methodInfo != null)
					break;
			}

			return methodInfo != null ? methodInfo : throw new MissingMethodException(methods[0]);
		}

		#endregion
	}
}