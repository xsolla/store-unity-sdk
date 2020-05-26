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
		
		private bool XsollaLoginSettings()
		{
			bool changed = false;
			
			using (new EditorGUILayout.VerticalScope("box"))
			{
				GUILayout.Label("Login SDK Settings", EditorStyles.boldLabel);
				var loginId = EditorGUILayout.TextField(new GUIContent("Login ID [?]", LoginIdTooltip),  XsollaSettings.LoginId);
				if (loginId != XsollaSettings.LoginId)
				{
					XsollaSettings.LoginId = loginId;
					changed = true;
				}
				var proxy = EditorGUILayout.Toggle("Enable proxy?", XsollaSettings.UseProxy);
				if (proxy != XsollaSettings.UseProxy)
				{
					XsollaSettings.UseProxy = proxy;
					changed = true;
				}
				var callback = EditorGUILayout.TextField(new GUIContent("Callback URL [?]", CallbackUrlTooltip),  XsollaSettings.CallbackUrl);
				if (callback != XsollaSettings.CallbackUrl)
				{
					XsollaSettings.CallbackUrl = callback;
					changed = true;
				}
			}
			EditorGUILayout.Space();
			
			return changed;
		}
	}
}

