using UnityEditor;
using UnityEngine;

namespace Xsolla.Core.Editor
{
	public partial class XsollaSettingsEditor : UnityEditor.Editor
	{
		private const string PAYMENTS_FLOW_TOOLTIP = "You need to sign an additional partner agreement to use Steam as a payment system. "
		                                             + "Contact the Integration or Account Manager";

		private bool XsollaStoreSettings()
		{
			var changed = false;
			using (new EditorGUILayout.VerticalScope("box"))
			{
				GUILayout.Label("Store SDK Settings", EditorStyles.boldLabel);

				var projectId = EditorGUILayout.TextField(new GUIContent("Project ID"), XsollaSettings.StoreProjectId);
				if (projectId != XsollaSettings.StoreProjectId)
				{
					XsollaSettings.StoreProjectId = projectId;
					changed = true;
				}

				if (XsollaSettings.UseSteamAuth)
				{
					var paymentsFlow = (PaymentsFlow) EditorGUILayout.EnumPopup(new GUIContent("Payments flow", PAYMENTS_FLOW_TOOLTIP), XsollaSettings.PaymentsFlow);
					if (paymentsFlow != XsollaSettings.PaymentsFlow)
					{
						XsollaSettings.PaymentsFlow = paymentsFlow;
						changed = true;
					}
				}

				var sandbox = EditorGUILayout.Toggle("Enable sandbox?", XsollaSettings.IsSandbox);
				if (sandbox != XsollaSettings.IsSandbox)
				{
					XsollaSettings.IsSandbox = sandbox;
					changed = true;
				}

				XsollaSettings.InAppBrowserEnabled = EditorGUILayout.Toggle("Enable in-app browser?", XsollaSettings.InAppBrowserEnabled);

				if (XsollaSettings.InAppBrowserEnabled)
				{
					XsollaSettings.PackInAppBrowserInBuild = EditorGUILayout.Toggle("Pack in-app browser in the build?", XsollaSettings.PackInAppBrowserInBuild);
				}

				DrawRedirectPolicySettings(XsollaSettings.DesktopRedirectPolicySettings, "Desktop redirect policy");
				DrawRedirectPolicySettings(XsollaSettings.WebglRedirectPolicySettings, "WebGL redirect policy");
				DrawRedirectPolicySettings(XsollaSettings.AndroidRedirectPolicySettings, "Android redirect policy");
			}
			EditorGUILayout.Space();

			return changed;
		}

		private void DrawRedirectPolicySettings(RedirectPolicySettings settings, string title)
		{
			EditorGUI.indentLevel++;

			settings.IsFoldout = EditorGUILayout.Foldout(settings.IsFoldout, title);
			if (settings.IsFoldout)
			{
				settings.IsOverride = EditorGUILayout.Toggle("Is override default", settings.IsOverride);
				GUI.enabled = settings.IsOverride;

				settings.ReturnUrl = EditorGUILayout.TextField("Return URL", settings.ReturnUrl);
				settings.RedirectConditions = (RedirectPolicySettings.RedirectConditionsType) EditorGUILayout.EnumPopup("Redirect conditions", settings.RedirectConditions);
				settings.Delay = EditorGUILayout.IntField("Delay", settings.Delay);
				settings.StatusForManualRedirection = (RedirectPolicySettings.StatusForManualRedirectionType) EditorGUILayout.EnumPopup("Status for manual redirection", settings.StatusForManualRedirection);
				settings.RedirectButtonCaption = EditorGUILayout.TextField("Redirect button caption", settings.RedirectButtonCaption);

				GUI.enabled = true;
			}

			EditorGUI.indentLevel--;
		}
	}
}
