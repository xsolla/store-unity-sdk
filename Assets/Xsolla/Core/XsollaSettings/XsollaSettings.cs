using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Xsolla.Core
{
	public class XsollaSettings : ScriptableObject
	{
		private const string SettingsAssetName = "XsollaSettings";
		private const string SettingsAssetPath = "Resources/";
		private const string SettingsAssetExtension = ".asset";

		private static XsollaSettings _instance;

		[SerializeField] public string loginId = Constants.DEFAULT_LOGIN_ID;
		[SerializeField] private bool useProxy = default;
		[SerializeField] private string callbackUrl = default;
		[SerializeField] public AuthorizationType authorizationType = default;
		[SerializeField] private bool jwtTokenInvalidationEnabled = default;
		[SerializeField] public int oauthClientId = default;
		[SerializeField] private bool requestNicknameOnAuth = true;
		[SerializeField] private string authServerUrl = "https://sdk.xsolla.com/";

		[SerializeField] private bool useSteamAuth = default;
		[SerializeField] private string steamAppId = "480";
		[SerializeField] private bool useConsoleAuth = default;
		[SerializeField] private PlatformType platform = PlatformType.Xsolla;
		[SerializeField] private string usernameFromConsole = default;

		[SerializeField] public string storeProjectId = Constants.DEFAULT_PROJECT_ID;
		[SerializeField] private PaymentsFlow paymentsFlow = PaymentsFlow.XsollaPayStation;
		[SerializeField] private bool isSandbox = true;
		[SerializeField] private bool inAppBrowserEnabled = true;
		[SerializeField] private bool packInAppBrowserInBuild = true;

		[SerializeField] private RedirectPolicySettings desktopRedirectPolicySettings = new RedirectPolicySettings();
		[SerializeField] private RedirectPolicySettings webglRedirectPolicySettings = new RedirectPolicySettings();
		[SerializeField] private RedirectPolicySettings androidRedirectPolicySettings = new RedirectPolicySettings();

		[SerializeField] private PayStationUISettings desktopPayStationUISettings = new PayStationUISettings();
		[SerializeField] private PayStationUISettings webglPayStationUISettings = new PayStationUISettings();
		[SerializeField] private PayStationUISettings androidPayStationUISettings = new PayStationUISettings();

		[SerializeField] private string facebookAppId = default;
		[SerializeField] private string googleServerId = default;
		[SerializeField] private string wechatAppId = default;
		[SerializeField] private string qqAppId = default;

		[SerializeField] private bool useDeepLinking = false;
		[SerializeField] private string deepLinkRedirectUrl = default;

		[SerializeField] public string webStoreUrl = "https://sitebuilder.xsolla.com/game/sdk-web-store/";

		[SerializeField] private LogLevel logLevel = LogLevel.InfoWarningsErrors;
		
		private const string LoginIdKey = nameof(loginId);

		public static event Action Changed;

		public static string LoginId
		{
			get
			{
				if (PlayerPrefs.HasKey(LoginIdKey))
					return PlayerPrefs.GetString(LoginIdKey);

				return Instance.loginId;
			}
			set
			{
				PlayerPrefs.SetString(LoginIdKey, value);

				if (!Application.isPlaying)
				{
					Instance.loginId = value;
					MarkAssetDirty();
				}
				
				Changed?.Invoke();
			}
		}

		public static bool UseSteamAuth
		{
			get => Instance.useSteamAuth;
			set
			{
				Instance.useSteamAuth = value;

				string useSteamMark = default(string);
#if UNITY_EDITOR_WIN
				useSteamMark = "Assets\\Xsolla\\ThirdParty\\use_steam";
#else
				useSteamMark = "Assets/Xsolla/ThirdParty/use_steam";
#endif
				try
				{
					if (value)
						File.Create(useSteamMark);
					else
					{
						File.Delete(useSteamMark);

						var meta = $"{useSteamMark}.meta";
						if (File.Exists(meta))
							File.Delete(meta);
					}
				}
				catch (Exception ex)
				{
					Debug.LogError($"Could not create or delete {useSteamMark}. {ex.Message}");
				}

				MarkAssetDirty();
			}
		}

		public static string SteamAppId
		{
			get => Instance.steamAppId;
			set
			{
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

		private const string AuthorizationTypeKey = nameof(authorizationType);

		public static AuthorizationType AuthorizationType
		{
			get
			{
				if (PlayerPrefs.HasKey(AuthorizationTypeKey))
				{
					var stringResult = PlayerPrefs.GetString(AuthorizationTypeKey);
					if (Enum.TryParse(stringResult, out AuthorizationType result))
					{
						return result;
					}
				}

				return Instance.authorizationType;
			}
			set
			{
				PlayerPrefs.SetString(AuthorizationTypeKey, value.ToString());
				
				if (!Application.isPlaying)
				{
					Instance.authorizationType = value;
					MarkAssetDirty();
				}
				
				Changed?.Invoke();
			}
		}

		public static bool JwtTokenInvalidationEnabled
		{
			get => Instance.jwtTokenInvalidationEnabled;
			set
			{
				Instance.jwtTokenInvalidationEnabled = value;
				MarkAssetDirty();
			}
		}

		private const string OAuthClientIdKey = nameof(oauthClientId);

		public static int OAuthClientId
		{
			get
			{
				if (PlayerPrefs.HasKey(OAuthClientIdKey))
					return PlayerPrefs.GetInt(OAuthClientIdKey);

				return Instance.oauthClientId;
			}
			set
			{
				PlayerPrefs.SetInt(OAuthClientIdKey, value);

				if (!Application.isPlaying)
				{
					Instance.oauthClientId = value;
					MarkAssetDirty();
				}
				
				Changed?.Invoke();
			}
		}

		public static bool RequestNicknameOnAuth
		{
			get => Instance.requestNicknameOnAuth;
			set
			{
				Instance.requestNicknameOnAuth = value;
				MarkAssetDirty();
			}
		}

		public static string AuthServerUrl
		{
			get => Instance.authServerUrl;
			set
			{
				Instance.authServerUrl = value;
				MarkAssetDirty();
			}
		}

		public static bool UseConsoleAuth
		{
			get => Instance.useConsoleAuth;
			set
			{
				Instance.useConsoleAuth = value;
				MarkAssetDirty();
			}
		}

		public static string UsernameFromConsolePlatform
		{
			get => Instance.usernameFromConsole;
			set
			{
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

		private const string StoreProjectIdKey = nameof(storeProjectId);

		public static string StoreProjectId
		{
			get
			{
				if (PlayerPrefs.HasKey(StoreProjectIdKey))
					return PlayerPrefs.GetString(StoreProjectIdKey);

				return Instance.storeProjectId;
			}
			set
			{
				PlayerPrefs.SetString(StoreProjectIdKey, value);

				if (!Application.isPlaying)
				{
					Instance.storeProjectId = value;
					MarkAssetDirty();
				}
				
				Changed?.Invoke();
			}
		}

		public static PaymentsFlow PaymentsFlow
		{
			get => Instance.paymentsFlow;
			set
			{
				Instance.paymentsFlow = value;
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

		public static PlatformType Platform
		{
			get => Instance.platform;
			set
			{
				Instance.platform = value;
				MarkAssetDirty();
			}
		}

		public static bool InAppBrowserEnabled
		{
			get => Instance.inAppBrowserEnabled;
			set
			{
				Instance.inAppBrowserEnabled = value;
				MarkAssetDirty();
			}
		}

		public static bool PackInAppBrowserInBuild
		{
			get => Instance.packInAppBrowserInBuild;
			set
			{
				Instance.packInAppBrowserInBuild = value;
				MarkAssetDirty();
			}
		}

		public static RedirectPolicySettings DesktopRedirectPolicySettings
		{
			get => Instance.desktopRedirectPolicySettings;
			set => Instance.desktopRedirectPolicySettings = value;
		}

		public static RedirectPolicySettings WebglRedirectPolicySettings
		{
			get => Instance.webglRedirectPolicySettings;
			set => Instance.webglRedirectPolicySettings = value;
		}

		public static RedirectPolicySettings AndroidRedirectPolicySettings
		{
			get => Instance.androidRedirectPolicySettings;
			set => Instance.androidRedirectPolicySettings = value;
		}

		public static PayStationUISettings DesktopPayStationUISettings
		{
			get => Instance.desktopPayStationUISettings;
			set => Instance.desktopPayStationUISettings = value;
		}

		public static PayStationUISettings WebglPayStationUISettings
		{
			get => Instance.webglPayStationUISettings;
			set => Instance.webglPayStationUISettings = value;
		}

		public static PayStationUISettings AndroidPayStationUISettings
		{
			get => Instance.androidPayStationUISettings;
			set => Instance.androidPayStationUISettings = value;
		}

		public static string FacebookAppId
		{
			get => Instance.facebookAppId;
			set => Instance.facebookAppId = value;
		}

		public static string GoogleServerId
		{
			get => Instance.googleServerId;
			set => Instance.googleServerId = value;
		}

		public static string WeChatAppId
		{
			get => Instance.wechatAppId;
			set => Instance.wechatAppId = value;
		}

		public static string QQAppId
		{
			get => Instance.qqAppId;
			set => Instance.qqAppId = value;
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

		public static string WebStoreUrl
		{
			get => Instance.webStoreUrl;
			set
			{
				Instance.webStoreUrl = value;
				MarkAssetDirty();
			}
		}

		public static LogLevel LogLevel
		{
			get => Instance.logLevel;
			set
			{
				Instance.logLevel = value;
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
