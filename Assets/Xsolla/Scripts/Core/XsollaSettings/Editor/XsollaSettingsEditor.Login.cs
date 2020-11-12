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
		const string JwtInvalidationTooltip = "Each time a user logs in, their previous JWT token becomes invalid.";
		const string OAuthClientIdTooltip = "Your application ID. You will get it after sending the request to enable the OAuth 2.0 protocol. To get your application ID again, please contact your Account Manager.";


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

				var authorizationType = (AuthorizationType)EditorGUILayout.Popup(
					label: "Authorization method",
					selectedIndex: (int)XsollaSettings.AuthorizationType,
					displayedOptions: new string[] {"JWT", "OAuth2.0"});
				if (authorizationType != XsollaSettings.AuthorizationType)
				{
					XsollaSettings.AuthorizationType = authorizationType;
					XsollaSettings.JwtTokenInvalidationEnabled = false;
					changed = true;
				}

				if (authorizationType == AuthorizationType.JWT)
				{
					var jwtTokenInvalidationEnabled = EditorGUILayout.Toggle(new GUIContent("Enable JWT invalidation? [?]", JwtInvalidationTooltip), XsollaSettings.JwtTokenInvalidationEnabled);
					if (jwtTokenInvalidationEnabled != XsollaSettings.JwtTokenInvalidationEnabled)
					{
						XsollaSettings.JwtTokenInvalidationEnabled = jwtTokenInvalidationEnabled;
						changed = true;
					}
				}
				else/*if (authorizationType == AuthorizationType.OAuth2_0)*/
				{
					var oauthClientId = EditorGUILayout.IntField(new GUIContent("OAuth2.0 client ID [?]", OAuthClientIdTooltip), XsollaSettings.OAuthClientId);
					if (oauthClientId != XsollaSettings.OAuthClientId)
					{
						XsollaSettings.OAuthClientId = oauthClientId;
						changed = true;
					}
				}

				var nickname = EditorGUILayout.Toggle("Request nickname on auth?", XsollaSettings.RequestNicknameOnAuth);
				if (nickname != XsollaSettings.RequestNicknameOnAuth)
				{
					XsollaSettings.RequestNicknameOnAuth = nickname;
					changed = true;
				}
			}
			EditorGUILayout.Space();
			
			return changed;
		}
	}
}

