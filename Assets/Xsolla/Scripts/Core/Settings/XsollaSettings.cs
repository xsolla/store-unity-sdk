using System.IO;
using UnityEditor;
using UnityEngine;

namespace Xsolla.Core
{
	public class XsollaSettings : ScriptableObject
	{
		const string SettingsAssetName = "XsollaSettings";
		const string SettingsAssetPath = "Resources/";
		const string SettingsAssetExtension = ".asset";

		static XsollaSettings _instance;

		[SerializeField]
		string loginId = "e6dfaac6-78a8-11e9-9244-42010aa80004";
		[SerializeField]
		bool useJwtValidation;
		[SerializeField]
		string jwtValidationUrl;
		[SerializeField]
		bool useProxy;
		[SerializeField]
		string callbackUrl;
		
		[SerializeField]
		string storeProjectId = "44056";
		[SerializeField]
		bool isSandbox = true;
		
		[SerializeField]
		string payStationProjectId;
		[SerializeField]
		string merchantId;
		
		public static string LoginId
		{
			get { return Instance.loginId; }
			set
			{
				Instance.loginId = value;
				MarkAssetDirty();
			}
		}

		public static bool UseJwtValidation
		{
			get { return Instance.useJwtValidation; }
			set
			{
				Instance.useJwtValidation = value;
				MarkAssetDirty();
			}
		}

		public static string JwtValidationUrl
		{
			get { return Instance.jwtValidationUrl; }
			set
			{
				Instance.jwtValidationUrl = value;
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
		
		public static string PayStationProjectId
		{
			get { return Instance.payStationProjectId; }
			set
			{
				Instance.payStationProjectId = value;
				MarkAssetDirty();
			}
		}
		
		public static string MerchantId
		{
			get { return Instance.merchantId; }
			set
			{
				Instance.merchantId = value;
				MarkAssetDirty();
			}
		}

		public static XsollaSettings Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = Resources.Load(SettingsAssetName) as XsollaSettings;
					if (_instance == null)
					{
						_instance = CreateInstance<XsollaSettings>();
						SaveAsset(Path.Combine(GetSdkPath(), SettingsAssetPath), SettingsAssetName);
					}
				}

				return _instance;
			}
		}
		
		public static string GetSdkPath()
		{
			return GetAbsoluteSdkPath().Replace("\\", "/").Replace(Application.dataPath, "Assets");
		}

		static string GetAbsoluteSdkPath()
		{
			return Path.GetDirectoryName(Path.GetDirectoryName(FindEditor(Application.dataPath)));
		}

		static string FindEditor(string path)
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

		static void SaveAsset(string directory, string name)
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

		static void MarkAssetDirty()
		{
#if UNITY_EDITOR
			EditorUtility.SetDirty(Instance);
#endif
		}
	}
}
