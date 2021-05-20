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
		[SerializeField] private AuthorizationType authorizationType;
		[SerializeField] private bool jwtTokenInvalidationEnabled;
		[SerializeField] private int oauthClientId;
		[SerializeField] private bool requestNicknameOnAuth;

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
		[SerializeField] private bool inAppBrowserEnabled = false;

		[SerializeField] private string facebookAppId;
		[SerializeField] private string googleServerId;

		[SerializeField] private bool useDeepLinking = false;
		[SerializeField] private string deepLinkRedirectUrl;

		[SerializeField] private string webStoreUrl = "https://livedemo.xsolla.com/sdk-account-linking/";

		public static string LoginId
		{
			get { return Instance.loginId; }
			set
			{
				Instance.loginId = value;
				MarkAssetDirty();
			}
		}

		public static bool UseSteamAuth {
			get { return Instance.useSteamAuth; }
			set
			{
				Instance.useSteamAuth = value;
				MarkAssetDirty();
			}
		}
		
		public static string SteamAppId {
			get { return Instance.steamAppId; }
			set
			{
				Instance.steamAppId = value;
				MarkAssetDirty();
			}
		}

		public static bool UseProxy
		{
			get { return Instance.useProxy; }
			set
			{
				Instance.useProxy = value;
				MarkAssetDirty();
			}
		}

		public static AuthorizationType AuthorizationType
		{
			get { return Instance.authorizationType; }
			set
			{
				Instance.authorizationType = value;
				MarkAssetDirty();
			}
		}

		public static bool JwtTokenInvalidationEnabled
		{
			get { return Instance.jwtTokenInvalidationEnabled; }
			set
			{
				Instance.jwtTokenInvalidationEnabled = value;
				MarkAssetDirty();
			}
		}

		public static int OAuthClientId
		{
			get { return Instance.oauthClientId; }
			set
			{
				Instance.oauthClientId = value;
				MarkAssetDirty();
			}
		}

		public static bool RequestNicknameOnAuth
		{
			get { return Instance.requestNicknameOnAuth; }
			set
			{
				Instance.requestNicknameOnAuth = value;
				MarkAssetDirty();
			}
		}

		public static bool UseConsoleAuth
		{
			get { return Instance.useConsoleAuth; }
			set
			{
				Instance.useConsoleAuth = value;
				MarkAssetDirty();
			}
		}
		
		public static string UsernameFromConsolePlatform {
			get { return Instance.usernameFromConsole; }
			set
			{
				Instance.usernameFromConsole = value;
				MarkAssetDirty();
			}
		}

		public static string CallbackUrl
		{
			get { return Instance.callbackUrl; }
			set
			{
				Instance.callbackUrl = value;
				MarkAssetDirty();
			}
		}
		
		public static string StoreProjectId
		{
			get { return Instance.storeProjectId; }
			set
			{
				Instance.storeProjectId = value;
				MarkAssetDirty();
			}
		}
		
		public static bool IsSandbox
		{
			get { return Instance.isSandbox; }
			set
			{
				Instance.isSandbox = value;
				MarkAssetDirty();
			}
		}

		public static PlatformType Platform
		{
			get { return Instance.platform; }
			set
			{
				Instance.platform = value;
				MarkAssetDirty();
			}
		}

		public static PaystationTheme PaystationTheme {
			get { return Instance.paystationTheme; }
			set
			{
				Instance.paystationTheme = value;
				MarkAssetDirty();
			}
		}

		public static string PayStationTokenRequestUrl
		{
			get { return Instance.payStationTokenRequestUrl; }
			set
			{
				Instance.payStationTokenRequestUrl = value;
				MarkAssetDirty();
			}
		}

		public static bool InAppBrowserEnabled
		{
			//get { return Instance.inAppBrowserEnabled; }
			get { return false; }
			set
			{
				//Instance.inAppBrowserEnabled = value;
				MarkAssetDirty();
			}
		}

		public static string FacebookAppId
		{
			get { return Instance.facebookAppId; }
			set { Instance.facebookAppId = value; }
		}

		public static string GoogleServerId
		{
			get { return Instance.googleServerId; }
			set { Instance.googleServerId = value; }
		}


		public static bool UseDeepLinking
		{
			get { return Instance.useDeepLinking; }
			set
			{
				Instance.useDeepLinking = value;
				MarkAssetDirty();
			}
		}

		public static string DeepLinkRedirectUrl
		{
			get { return Instance.deepLinkRedirectUrl; }
			set
			{
				Instance.deepLinkRedirectUrl = value;
				MarkAssetDirty();
			}
		}

		public static string WebStoreUrl
		{
			get { return Instance.webStoreUrl; }
			set
			{
				Instance.webStoreUrl = value;
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
