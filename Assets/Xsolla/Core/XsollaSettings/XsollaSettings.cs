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
		[SerializeField] private bool useProxy = default(bool);
		[SerializeField] private string callbackUrl = default(string);
		[SerializeField] public AuthorizationType authorizationType = default(AuthorizationType);
		[SerializeField] private bool jwtTokenInvalidationEnabled = default(bool);
		[SerializeField] public int oauthClientId = default(int);
		[SerializeField] private bool requestNicknameOnAuth = true;
		[SerializeField] private string authServerUrl = "https://sdk.xsolla.com/";

		[SerializeField] private bool useSteamAuth = default(bool);
		[SerializeField] private string steamAppId = "480";
		[SerializeField] private bool useConsoleAuth = default(bool);
		[SerializeField] private PlatformType platform = PlatformType.Xsolla;
		[SerializeField] private string usernameFromConsole = default(string);

		[SerializeField] public string storeProjectId = Constants.DEFAULT_PROJECT_ID;
		[SerializeField] private PaymentsFlow paymentsFlow = PaymentsFlow.XsollaPayStation;
		[SerializeField] private bool isSandbox = true;
		//[SerializeField] private bool inAppBrowserEnabled = true;
		[SerializeField] private bool packInAppBrowserInBuild = true;

		[SerializeField] private RedirectPolicySettings desktopRedirectPolicySettings = new RedirectPolicySettings();
		[SerializeField] private RedirectPolicySettings webglRedirectPolicySettings = new RedirectPolicySettings();
		[SerializeField] private RedirectPolicySettings androidRedirectPolicySettings = new RedirectPolicySettings();

		[SerializeField] private PayStationUISettings desktopPayStationUISettings = new PayStationUISettings();
		[SerializeField] private PayStationUISettings webglPayStationUISettings = new PayStationUISettings();
		[SerializeField] private PayStationUISettings androidPayStationUISettings = new PayStationUISettings();

		[SerializeField] private string facebookAppId = default(string);
		[SerializeField] private string googleServerId = default(string);
		[SerializeField] private string wechatAppId = default(string);
		[SerializeField] private string qqAppId = default(string);

		[SerializeField] private bool useDeepLinking = false;
		[SerializeField] private string deepLinkRedirectUrl = default(string);

		[SerializeField] public string webStoreUrl = "https://sitebuilder.xsolla.com/game/sdk-web-store/";

		[SerializeField] private LogLevel logLevel = LogLevel.InfoWarningsErrors;
		
		private const string LoginIdKey = "loginId";

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

				if (Changed != null)
					Changed.Invoke();
			}
		}

		public static bool UseSteamAuth
		{
			get
			{
				return Instance.useSteamAuth;
			}
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

						var meta = string.Format("{0}.meta", useSteamMark);
						if (File.Exists(meta))
							File.Delete(meta);
					}
				}
				catch (Exception ex)
				{
					Debug.LogError(string.Format("Could not create or delete {0}. {1}", useSteamMark, ex.Message));
				}

				MarkAssetDirty();
			}
		}

		public static string SteamAppId
		{
			get
			{
				return Instance.steamAppId;
			}
			set
			{
				Instance.steamAppId = value;
				MarkAssetDirty();
			}
		}

		public static bool UseProxy
		{
			get
			{
				return Instance.useProxy;
			}
			set
			{
				Instance.useProxy = value;
				MarkAssetDirty();
			}
		}

		private const string AuthorizationTypeKey = "authorizationType";

		public static AuthorizationType AuthorizationType
		{
			get
			{
				if (PlayerPrefs.HasKey(AuthorizationTypeKey))
				{
					var stringResult = PlayerPrefs.GetString(AuthorizationTypeKey);
					AuthorizationType result = AuthorizationType.JWT;
					var parsed = true;
					try
					{
						result = (AuthorizationType)Enum.Parse(typeof(AuthorizationType), stringResult);
					}
					catch (Exception)
					{
						parsed = false;
					}

					if (parsed)
						return result;
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

				if (Changed != null)
					Changed.Invoke();
			}
		}

		public static bool JwtTokenInvalidationEnabled
		{
			get
			{
				return Instance.jwtTokenInvalidationEnabled;
			}
			set
			{
				Instance.jwtTokenInvalidationEnabled = value;
				MarkAssetDirty();
			}
		}

		private const string OAuthClientIdKey = "oauthClientId";

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

				if (Changed != null)
					Changed.Invoke();
			}
		}

		public static bool RequestNicknameOnAuth
		{
			get
			{
				return Instance.requestNicknameOnAuth;
			}
			set
			{
				Instance.requestNicknameOnAuth = value;
				MarkAssetDirty();
			}
		}

		public static string AuthServerUrl
		{
			get
			{
				return Instance.authServerUrl;
			}
			set
			{
				Instance.authServerUrl = value;
				MarkAssetDirty();
			}
		}

		public static bool UseConsoleAuth
		{
			get
			{
				return Instance.useConsoleAuth;
			}
			set
			{
				Instance.useConsoleAuth = value;
				MarkAssetDirty();
			}
		}

		public static string UsernameFromConsolePlatform
		{
			get
			{
				return Instance.usernameFromConsole;
			}
			set
			{
				Instance.usernameFromConsole = value;
				MarkAssetDirty();
			}
		}

		public static string CallbackUrl
		{
			get
			{
				return Instance.callbackUrl;
			}
			set
			{
				Instance.callbackUrl = value;
				MarkAssetDirty();
			}
		}

		private const string StoreProjectIdKey = "storeProjectId";

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

				if (Changed != null)
					Changed.Invoke();
			}
		}

		public static PaymentsFlow PaymentsFlow
		{
			get
			{
				return Instance.paymentsFlow;
			}
			set
			{
				Instance.paymentsFlow = value;
				MarkAssetDirty();
			}
		}

		public static bool IsSandbox
		{
			get
			{
				return Instance.isSandbox;
			}
			set
			{
				Instance.isSandbox = value;
				MarkAssetDirty();
			}
		}

		public static PlatformType Platform
		{
			get
			{
				return Instance.platform;
			}
			set
			{
				Instance.platform = value;
				MarkAssetDirty();
			}
		}

		public static bool InAppBrowserEnabled
		{
			get
			{
				return false;
			}
			set
			{
				//Instance.inAppBrowserEnabled = value;
				//MarkAssetDirty();
			}
		}

		public static bool PackInAppBrowserInBuild
		{
			get
			{
				return Instance.packInAppBrowserInBuild;
			}
			set
			{
				Instance.packInAppBrowserInBuild = value;
				MarkAssetDirty();
			}
		}

		public static RedirectPolicySettings DesktopRedirectPolicySettings
		{
			get
			{
				return Instance.desktopRedirectPolicySettings;
			}
			set
			{
				Instance.desktopRedirectPolicySettings = value;
			}
		}

		public static RedirectPolicySettings WebglRedirectPolicySettings
		{
			get
			{
				return Instance.webglRedirectPolicySettings;
			}
			set
			{
				Instance.webglRedirectPolicySettings = value;
			}
		}

		public static RedirectPolicySettings AndroidRedirectPolicySettings
		{
			get
			{
				return Instance.androidRedirectPolicySettings;
			}
			set
			{
				Instance.androidRedirectPolicySettings = value;
			}
		}

		public static PayStationUISettings DesktopPayStationUISettings
		{
			get
			{
				return Instance.desktopPayStationUISettings;
			}
			set
			{
				Instance.desktopPayStationUISettings = value;
			}
		}

		public static PayStationUISettings WebglPayStationUISettings
		{
			get
			{
				return Instance.webglPayStationUISettings;
			}
			set
			{
				Instance.webglPayStationUISettings = value;
			}
		}

		public static PayStationUISettings AndroidPayStationUISettings
		{
			get
			{
				return Instance.androidPayStationUISettings;
			}
			set
			{
				Instance.androidPayStationUISettings = value;
			}
		}

		public static string FacebookAppId
		{
			get
			{
				return Instance.facebookAppId;
			}
			set
			{
				Instance.facebookAppId = value;
			}
		}

		public static string GoogleServerId
		{
			get
			{
				return Instance.googleServerId;
			}
			set
			{
				Instance.googleServerId = value;
			}
		}

		public static string WeChatAppId
		{
			get
			{
				return Instance.wechatAppId;
			}
			set
			{
				Instance.wechatAppId = value;
			}
		}

		public static string QQAppId
		{
			get
			{
				return Instance.qqAppId;
			}
			set
			{
				Instance.qqAppId = value;
			}
		}

		public static bool UseDeepLinking
		{
			get
			{
				return Instance.useDeepLinking;
			}
			set
			{
				Instance.useDeepLinking = value;
				MarkAssetDirty();
			}
		}

		public static string DeepLinkRedirectUrl
		{
			get
			{
				return Instance.deepLinkRedirectUrl;
			}
			set
			{
				Instance.deepLinkRedirectUrl = value;
				MarkAssetDirty();
			}
		}

		private const string WebStoreUrlKey = "webStoreUrl";
		public static string WebStoreUrl
		{
			get
			{
				if (PlayerPrefs.HasKey(WebStoreUrlKey))
					return PlayerPrefs.GetString(WebStoreUrlKey);
				else
					return Instance.webStoreUrl;
			}
			set
			{
				PlayerPrefs.SetString(WebStoreUrlKey, value);

				if (!Application.isPlaying)
				{
					Instance.webStoreUrl = value;
					MarkAssetDirty();
				}

				if (Changed != null)
					Changed.Invoke();
			}
		}

		public static LogLevel LogLevel
		{
			get
			{
				return Instance.logLevel;
			}
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
