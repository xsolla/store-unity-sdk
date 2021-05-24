using UnityEditor;
using UnityEngine;
using Facebook.Unity.Editor;

namespace Xsolla.Core
{
	public partial class XsollaSettingsEditor : UnityEditor.Editor
	{
		const string FacebookAppIdTooltip = "Application ID from your Facebook developer page";
		const string GoogleServerIdTooltip = "Server ID from your Google developer page";
		const string WechatAppIdTooltip = "Application ID from your WeCHat developer page";
		const string QQAppIdTooltip = "Application ID from your QQ developer page";

		private bool AndroidSDKSettings()
		{
			bool changed = false;

			using (new EditorGUILayout.VerticalScope("box"))
			{
				EditorGUILayout.LabelField("Android SDK Settings", EditorStyles.boldLabel);

				var facebookAppId = EditorGUILayout.TextField(new GUIContent("Facebook App ID [?]", FacebookAppIdTooltip),  XsollaSettings.FacebookAppId);
				if (facebookAppId != XsollaSettings.FacebookAppId)
				{
					XsollaSettings.FacebookAppId = facebookAppId;
					changed = true;
				}

				var googleServerId = EditorGUILayout.TextField(new GUIContent("Google server ID [?]", GoogleServerIdTooltip),  XsollaSettings.GoogleServerId);
				if (googleServerId != XsollaSettings.GoogleServerId)
				{
					XsollaSettings.GoogleServerId = googleServerId;
					changed = true;
				}
				
				var wechatAppId = EditorGUILayout.TextField(new GUIContent("WeChat App ID [?]", WechatAppIdTooltip),  XsollaSettings.WeChatAppId);
				if (wechatAppId != XsollaSettings.WeChatAppId)
				{
					XsollaSettings.WeChatAppId = wechatAppId;
					changed = true;
				}
				
				var qqAppId = EditorGUILayout.TextField(new GUIContent("QQ App ID [?]", QQAppIdTooltip),  XsollaSettings.QQAppId);
				if (qqAppId != XsollaSettings.QQAppId)
				{
					XsollaSettings.QQAppId = qqAppId;
					changed = true;
				}

				var hashkeyLabelText = FacebookAndroidUtil.SetupProperly ? FacebookAndroidUtil.DebugKeyHash : FacebookAndroidUtil.SetupError;
				EditorGUILayout.BeginHorizontal();
				{
					EditorGUILayout.LabelField("Android debug hash key");
					EditorGUILayout.TextField(hashkeyLabelText);
				}
				EditorGUILayout.EndHorizontal();


				EditorGUILayout.BeginHorizontal();
				{
					EditorGUILayout.LabelField("Android class name");
					EditorGUILayout.TextField("com.xsolla.android.storesdkexample.MainActivity");
				}
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.Space();

			return changed;
		}
	}
}
