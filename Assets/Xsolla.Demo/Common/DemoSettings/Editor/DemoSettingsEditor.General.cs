using UnityEditor;

namespace Xsolla.Demo
{
	public partial class DemoSettingsEditor
	{
		private const string REQUEST_NICKNAME_ON_AUTH_LABEL = "Request Nickname on Auth";
		private const string WEB_STORE_URL_LABEL = "Web Store URL";

		private bool GeneralSettings()
		{
			EditorGUILayout.Space();
			EditorGUILayout.BeginVertical(GroupAreaStyle);
			EditorGUILayout.LabelField("General", GroupHeaderStyle);

			var nickname = EditorGUILayout.Toggle(REQUEST_NICKNAME_ON_AUTH_LABEL, DemoSettings.RequestNicknameOnAuth);
			if (nickname != DemoSettings.RequestNicknameOnAuth)
			{
				DemoSettings.RequestNicknameOnAuth = nickname;
			}

			var webStoreUrl = EditorGUILayout.TextField(WEB_STORE_URL_LABEL, DemoSettings.WebStoreUrl);
			if (webStoreUrl != DemoSettings.WebStoreUrl)
			{
				DemoSettings.WebStoreUrl = webStoreUrl;
			}

			EditorGUILayout.EndVertical();
			//TEXTREVIEW
			return false;//These settings do not affect authorization and thus should not result in token deletion
		}
	}
}