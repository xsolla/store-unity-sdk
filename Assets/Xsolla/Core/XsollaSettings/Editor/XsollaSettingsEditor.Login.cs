using UnityEditor;
using UnityEngine;

namespace Xsolla.Core
{
	public partial class XsollaSettingsEditor
	{
		private const string AUTHORIZATION_TYPE_LABEL = "Authorization Type";
		private const string AUTHORIZATION_TYPE_TOOLTIP = "If you are not using Xsolla Login to authenticate user, select \"Acces Token.\"\n" +
		                                                  "If you set up OAuth 2.0 protocol based authentication in Publisher Account, select \"OAuth.\"\n" +
		                                                  "Otherwise, select \"JWT.\"";

		private const string CALLBACK_URL_LABEL = "Callback URL";
		private const string CALLBACK_URL_TOOLTIP = "UURL to redirect the user to after signing up, logging in, or password reset. " +
		                                            "Must be identical to the callback URL specified in Publisher Account in Login settings. " +
		                                            "Required if there are several сallback URLs.";

		private const string JWT_INVALIDATION_LABEL = "Invalidate Existing Sessions";
		private const string JWT_INVALIDATION_TOOLTIP = "If enabled, every time the user logs in from a new device, " +
		                                                "a new token replaces the old one and the old token becomes invalid.";

		private const string OAUTH_CLIENT_ID_LABEL = "OAuth Client ID";
		private const string OAUTH_CLIENT_ID_TOOLTIP = "To get the ID, set up an OAuth client in Publusher account in the " +
		                                               "\"Login -> your Login project -> Secure -> OAuth 2.0\" section.";

		private const string AUTH_SERVER_LABEL = "Authentication Server URL";
		private const string AUTH_SERVER_TOOLTIP = "URL for login via an access token.";

		private readonly string[] AuthorizationTypeOptions ={
			"OAuth2.0",
			"JWT",
			"Access Token"
		};

		private bool LoginSettings()
		{
			EditorGUILayout.Space();
			EditorGUILayout.BeginVertical(GroupAreaStyle);
			EditorGUILayout.LabelField("Login", GroupHeaderStyle);

			var changed = false;

			var authorizationType = (AuthorizationType) EditorGUILayout.Popup(new GUIContent(AUTHORIZATION_TYPE_LABEL, AUTHORIZATION_TYPE_TOOLTIP), (int) XsollaSettings.AuthorizationType, AuthorizationTypeOptions);
			if (authorizationType != XsollaSettings.AuthorizationType)
			{
				XsollaSettings.AuthorizationType = authorizationType;

				if (authorizationType != AuthorizationType.JWT)
					XsollaSettings.InvalidateExistingSessions = false;

				changed = true;
			}

			if (XsollaSettings.AuthorizationType == AuthorizationType.JWT)
			{
				var jwtTokenInvalidationEnabled = EditorGUILayout.Toggle(new GUIContent(JWT_INVALIDATION_LABEL, JWT_INVALIDATION_TOOLTIP), XsollaSettings.InvalidateExistingSessions);
				if (jwtTokenInvalidationEnabled != XsollaSettings.InvalidateExistingSessions)
				{
					XsollaSettings.InvalidateExistingSessions = jwtTokenInvalidationEnabled;
					changed = true;
				}
			}
			else if (authorizationType == AuthorizationType.OAuth2_0)
			{
				var oauthClientId = EditorGUILayout.IntField(new GUIContent(OAUTH_CLIENT_ID_LABEL, OAUTH_CLIENT_ID_TOOLTIP), XsollaSettings.OAuthClientId);
				if (oauthClientId != XsollaSettings.OAuthClientId)
				{
					XsollaSettings.OAuthClientId = oauthClientId;
					changed = true;
				}
			}
			else if (authorizationType == AuthorizationType.AccessToken)
			{
				var authServerUrl = EditorGUILayout.TextField(new GUIContent(AUTH_SERVER_LABEL, AUTH_SERVER_TOOLTIP), XsollaSettings.AuthServerUrl);
				if (authServerUrl != XsollaSettings.AuthServerUrl)
				{
					XsollaSettings.AuthServerUrl = authServerUrl;
					changed = true;
				}
			}

			if (XsollaSettings.AuthorizationType != AuthorizationType.AccessToken)
			{
				var callback = EditorGUILayout.TextField(new GUIContent(CALLBACK_URL_LABEL, CALLBACK_URL_TOOLTIP), XsollaSettings.CallbackUrl);
				if (callback != XsollaSettings.CallbackUrl)
				{
					XsollaSettings.CallbackUrl = callback;
					changed = true;
				}
			}

			EditorGUILayout.EndVertical();
			return changed;
		}
	}
}