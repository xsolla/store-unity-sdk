using UnityEditor;
using UnityEngine;
using Xsolla.PayStation;
using Xsolla.Store;

namespace Xsolla.Core
{
	public partial class XsollaSettingsEditor : UnityEditor.Editor
	{
		const string LoginIdTooltip = "Login ID from your Publisher Account.";
		const string CallbackUrlTooltip = "URL to redirect the user to after registration/authentication/password reset. " +
		                                  "Must be identical to Callback URL specified in Publisher Account in Login settings. Required if there are several Callback URLs.";
		
		private void XsollaLoginSettings()
		{
			using (new EditorGUILayout.VerticalScope("box"))
			{
				GUILayout.Label("Login SDK Settings", EditorStyles.boldLabel);
				XsollaSettings.LoginId = EditorGUILayout.TextField(new GUIContent("Login ID [?]", LoginIdTooltip),  XsollaSettings.LoginId);
				XsollaSettings.UseProxy = EditorGUILayout.Toggle("Enable proxy?", XsollaSettings.UseProxy);
				XsollaSettings.CallbackUrl = EditorGUILayout.TextField(new GUIContent("Callback URL [?]", CallbackUrlTooltip),  XsollaSettings.CallbackUrl);
			}
			
			EditorGUILayout.Space();
		}
	}
}

