using System.IO;
using UnityEditor;
using UnityEngine;

namespace Xsolla.Core
{
	public class XsollaSettings : ScriptableObject
	{
		private const string SETTINGS_ASSET_NAME = "XsollaSettings";
		private const string SETTINGS_ASSET_PATH = "Resources/";
		private const string SETTINGS_ASSET_EXTENSION = ".asset";

		[SerializeField] private string loginId = Constants.DEFAULT_LOGIN_ID;
		[SerializeField] private string callbackUrl;
		[SerializeField] private bool invalidateExistingSessions;
		[SerializeField] private int oauthClientId = Constants.DEFAULT_OAUTH_CLIENT_ID;

		[SerializeField] private string storeProjectId = Constants.DEFAULT_PROJECT_ID;
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

		[SerializeField] private string facebookAppId;
		[SerializeField] private string facebookClientToken;
		[SerializeField] private string googleServerId;
		[SerializeField] private string wechatAppId;
		[SerializeField] private string qqAppId;

		[SerializeField] private LogLevel logLevel = LogLevel.InfoWarningsErrors;

		public static bool PayStationGroupFoldout { get; set; }
		public static bool RedirectPolicyGroupFoldout { get; set; }

		public static string LoginId
		{
			get => Instance.loginId;
			set
			{
				Instance.loginId = value;
				MarkAssetDirty();
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

		public static int OAuthClientId
		{
			get => Instance.oauthClientId;
			set
			{
				Instance.oauthClientId = value;
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

		public static string FacebookClientToken
		{
			get => Instance.facebookClientToken;
			set => Instance.facebookClientToken = value;
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

		public static string QqAppId
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

		private static XsollaSettings _instance;

		public static XsollaSettings Instance
		{
			get
			{
				if (_instance)
					return _instance;

				_instance = Resources.Load(SETTINGS_ASSET_NAME) as XsollaSettings;
				if (_instance)
					return _instance;

				_instance = CreateInstance<XsollaSettings>();
#if UNITY_EDITOR
				var absolutePath = Path.GetDirectoryName(Path.GetDirectoryName(FindEditor(Application.dataPath)));
				if (absolutePath != null)
				{
					var sdkPath = absolutePath.Replace("\\", "/").Replace(Application.dataPath, "Assets");
					SaveAsset(Path.Combine(sdkPath, SETTINGS_ASSET_PATH), SETTINGS_ASSET_NAME);
				}

#endif
				return _instance;
			}
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
				Directory.CreateDirectory(directory);

			AssetDatabase.CreateAsset(Instance, $"{directory}{name}{SETTINGS_ASSET_EXTENSION}");
			AssetDatabase.Refresh();
#endif
		}

		private static void MarkAssetDirty()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
				EditorUtility.SetDirty(Instance);
#endif
		}
	}
}