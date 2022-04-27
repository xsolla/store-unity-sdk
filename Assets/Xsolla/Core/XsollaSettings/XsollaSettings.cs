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
		[SerializeField] private string callbackUrl = default;
		[SerializeField] public AuthorizationType authorizationType = AuthorizationType.OAuth2_0;
		[SerializeField] private bool invalidateExistingSessions = default;
		[SerializeField] public int oauthClientId = default;
		[SerializeField] private string authServerUrl = "https://sdk.xsolla.com/";

		[SerializeField] public string storeProjectId = Constants.DEFAULT_PROJECT_ID;
		[SerializeField] private bool isSandbox = true;
		[SerializeField] private bool inAppBrowserEnabled = true;
		[SerializeField] private bool packInAppBrowserInBuild = true;

		[SerializeField] private RedirectPolicySettings desktopRedirectPolicySettings = new RedirectPolicySettings();
		[SerializeField] private RedirectPolicySettings webglRedirectPolicySettings = new RedirectPolicySettings();
		[SerializeField] private RedirectPolicySettings androidRedirectPolicySettings = new RedirectPolicySettings();
		[SerializeField] private RedirectPolicySettings iosRedirectPolicySettings = new RedirectPolicySettings();

		[SerializeField] private PayStationUISettings desktopPayStationUISettings = new PayStationUISettings();
		[SerializeField] private PayStationUISettings webglPayStationUISettings = new PayStationUISettings();
		[SerializeField] private PayStationUISettings androidPayStationUISettings = new PayStationUISettings();
		[SerializeField] private PayStationUISettings iosPayStationUISettings = new PayStationUISettings();

		[SerializeField] private string facebookAppId = default;
		[SerializeField] private string googleServerId = default;
		[SerializeField] private string wechatAppId = default;
		[SerializeField] private string qqAppId = default;

		[SerializeField] private LogLevel logLevel = LogLevel.InfoWarningsErrors;
		
		private const string LoginIdKey = nameof(loginId);

		public static event Action Changed;
		
		public static bool PayStationGroupFoldout { get; set; }
		
		public static bool RedirectPolicyGroupFoldout { get; set; }

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

		public static bool InvalidateExistingSessions
		{
			get => Instance.invalidateExistingSessions;
			set
			{
				Instance.invalidateExistingSessions = value;
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

		public static string AuthServerUrl
		{
			get => Instance.authServerUrl;
			set
			{
				Instance.authServerUrl = value;
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

		public static bool IsSandbox
		{
			get => Instance.isSandbox;
			set
			{
				Instance.isSandbox = value;
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

		public static RedirectPolicySettings IosRedirectPolicySettings
		{
			get => Instance.iosRedirectPolicySettings;
			set => Instance.iosRedirectPolicySettings = value;
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
		
		public static PayStationUISettings IosPayStationUISettings
		{
			get => Instance.iosPayStationUISettings;
			set => Instance.iosPayStationUISettings = value;
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
