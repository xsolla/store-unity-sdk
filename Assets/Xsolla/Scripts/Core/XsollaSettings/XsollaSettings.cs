using System.IO;
using UnityEditor;
using UnityEngine;
using Xsolla.PayStation;
using Xsolla.Store;

namespace Xsolla.Core
{
	public class XsollaSettings : ScriptableObject
	{
		private const string SettingsAssetName = "XsollaSettings";
		private const string SettingsAssetPath = "Resources/";
		private const string SettingsAssetExtension = ".asset";

		private static XsollaSettings _instance;

		[SerializeField] private string loginId = Constants.DEFAULT_LOGIN_ID;
		[SerializeField] private bool useProxy;
		[SerializeField] private string callbackUrl;
		
		[SerializeField] private bool useSteamAuth = true;
		[SerializeField] private string steamAppId = "480";
		[SerializeField] private bool useConsoleAuth;
		[SerializeField] private PlatformType platform = PlatformType.Xsolla;
		[SerializeField] private string usernameFromConsole;
		
		[SerializeField] private string storeProjectId = Constants.DEFAULT_PROJECT_ID;
		[SerializeField] private bool isSandbox = true;

		[SerializeField]
		public PaystationTheme paystationTheme = PaystationTheme.Dark;
		[SerializeField] private string payStationTokenRequestUrl = "https://livedemo.xsolla.com/sdk/token/paystation_demo/";
		[SerializeField] private bool inAppBrowserEnabled = true;

		[SerializeField] private bool useDeepLinking = false;
		[SerializeField] private string deepLinkRedirectUrl;

		public static string LoginId
		{
			get => Instance.loginId;
			set
			{
				Instance.loginId = value;
				MarkAssetDirty();
			}
		}

		public static bool UseSteamAuth {
			get => Instance.useSteamAuth;
			set {
				Instance.useSteamAuth = value;
				MarkAssetDirty();
			}
		}
		
		public static string SteamAppId {
			get => Instance.steamAppId;
			set {
				Instance.steamAppId = value;
				MarkAssetDirty();
			}
		}

		public static bool UseProxy
		{
			get => Instance.useProxy;
			set
			{
				Instance.useProxy = value;
				MarkAssetDirty();
			}
		}

		public static bool UseConsoleAuth {
			get => Instance.useConsoleAuth;
			set {
				Instance.useConsoleAuth = value;
				MarkAssetDirty();
			}
		}
		
		public static string UsernameFromConsolePlatform {
			get => Instance.usernameFromConsole;
			set {
				Instance.usernameFromConsole = value;
				MarkAssetDirty();
			}
		}

		public static string CallbackUrl
		{
			get => Instance.callbackUrl;
			set
			{
				Instance.callbackUrl = value;
				MarkAssetDirty();
			}
		}
		
		public static string StoreProjectId
		{
			get => Instance.storeProjectId;
			set
			{
				Instance.storeProjectId = value;
				MarkAssetDirty();
			}
		}
		
		public static bool IsSandbox
		{
			get => Instance.isSandbox;
			set
			{
				Instance.isSandbox = value;
				MarkAssetDirty();
			}
		}

		public static PlatformType Platform {
			get => Instance.platform;
			set {
				Instance.platform = value;
				MarkAssetDirty();
			}
		}

		public static PaystationTheme PaystationTheme {
			get => Instance.paystationTheme;
			set {
				Instance.paystationTheme = value;
				MarkAssetDirty();
			}
		}

		public static string PayStationTokenRequestUrl
		{
			get => Instance.payStationTokenRequestUrl;
			set
			{
				Instance.payStationTokenRequestUrl = value;
				MarkAssetDirty();
			}
		}

		public static bool InAppBrowserEnabled
		{
			get => Instance.inAppBrowserEnabled;
			set {
				Instance.inAppBrowserEnabled = value;
				MarkAssetDirty();
			}
		}

		public static bool UseDeepLinking
		{
			get => Instance.useDeepLinking;
			set
			{
				Instance.useDeepLinking = value;
				MarkAssetDirty();
			}
		}

		public static string DeepLinkRedirectUrl
		{
			get => Instance.deepLinkRedirectUrl;
			set
			{
				Instance.deepLinkRedirectUrl = value;
				MarkAssetDirty();
			}
		}

		public static XsollaSettings Instance
		{
			get
			{
				_instance = _instance ? _instance : Resources.Load(SettingsAssetName) as XsollaSettings;
				if (_instance != null) return _instance;
				_instance = CreateInstance<XsollaSettings>();
				SaveAsset(Path.Combine(GetSdkPath(), SettingsAssetPath), SettingsAssetName);

				return _instance;
			}
		}

		private static string GetSdkPath()
		{
			return GetAbsoluteSdkPath().Replace("\\", "/").Replace(Application.dataPath, "Assets");
		}

		private static string GetAbsoluteSdkPath()
		{
			return Path.GetDirectoryName(Path.GetDirectoryName(FindEditor(Application.dataPath)));
		}

		private static string FindEditor(string path)
		{
			foreach (var d in Directory.GetDirectories(path))
			{
				foreach (var f in Directory.GetFiles(d))
				{
					if (f.Contains("XsollaSettingsEditor.cs"))
					{ 
						return f;
					}
				}

				var rec = FindEditor(d);
				if (rec != null)
				{
					return rec;
				}
			}

			return null;
		}

		private static void SaveAsset(string directory, string name)
		{
#if UNITY_EDITOR
			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}

			AssetDatabase.CreateAsset(Instance, directory + name + SettingsAssetExtension);
			AssetDatabase.Refresh();
#endif
		}

		private static void MarkAssetDirty()
		{
#if UNITY_EDITOR
			EditorUtility.SetDirty(Instance);
#endif
		}
	}
}
