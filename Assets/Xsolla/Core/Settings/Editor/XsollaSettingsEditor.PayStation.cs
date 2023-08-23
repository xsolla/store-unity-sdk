using UnityEditor;
using UnityEngine;

namespace Xsolla.Core
{
	public partial class XsollaSettingsEditor
	{
		private const string SANDBOX_LABEL = "Test Payments Mode";
		private const string SANDBOX_TOOLTIP = "If you already accepted a real payment, test payments are available only to users " +
		                                       "who are specified in Publisher Account in the \"Company settings -> Users\" section.";

		private const string PAYSTATION_UI_GROUP_LABEL = "Pay Station UI";
		private const string REDIRECT_POLICY_GROUP_LABEL = "Redirect Policy";
		private const string DESKTOP_GROUP_LABEL = "Desktop";
		private const string WEBGL_GROUP_LABEL = "WebGL";
		private const string ANDROID_GROUP_LABEL = "Android";
		private const string IOS_GROUP_LABEL = "iOS";

		private const string PAYSTATION_THEME_LABEL = "Pay Station Theme Id";
		private const string PAYSTATION_THEME_TOOLTIP = "To use default theme ID, enter \"63295a9a2e47fab76f7708e1\" (light) or \"63295aab2e47fab76f7708e3\" (dark) values." +
		                                                "Or enter the ID of a custom theme you have configured in Publisher Account to use it.";

		private const string PAYSTATION_SIZE_LABEL = "Pay Station Size";
		private const string PAYSTATION_SIZE_TOOLTIP = "Small: 620 x 630 px\n" +
		                                               "Medium (recomended): 740 x 760 px\n" +
		                                               "Large: 820 x 840 px";

		private const string PAYSTATION_VERSION_LABEL = "Device Type";
		private const string PAYSTATION_VERSION_TOOLTIP = "The Pay Station UI depends on the type of device. " +
		                                                  "If Auto, the app automatically uses Pay Station UI supported by the device.";

		private const string OVERRIDE_REDIRECT_POLICY_LABEL = "Use settings from Publisher Account ";
		private const string OVERRIDE_REDIRECT_POLICY_TOOLTIP = "If the option is enabled, SDK uses redirect settings " +
		                                                        "specified in Publisher Account in the \"Pay Station -> Settings -> Redirect policy\" section.";

		private const string REDIRECT_URL_LABEL = "Redirect URL";
		private const string REDIRECT_URL_TOOLTIP = "URL to redirect the user to after payment.";

		private const string REDIRECT_CONDITIONS_LABEL = "Redirect Conditions";
		private const string REDIRECT_CONDITIONS_TOOLTIP = "Payment status when user is automatically redirected to the return URL.\n" +
		                                                   "\"None\" — do not redirect.\n" +
		                                                   "\"Successful payment\" — redirect when a payment is successful.\n" +
		                                                   "\"Successful or canceled payment\" — redirect when a payment is successful or canceled.\n" +
		                                                   "\"Any payment\" — redirect for any payment status.";

		private const string REDIRECT_TIMEOUT_LABEL = "Redirect Timeout";
		private const string REDIRECT_TIMEOUT_TOOLTIP = "Redirect timeout in seconds.";

		private const string MANUAL_REDIRECTION_LABEL = "Status for Manual Redirection";
		private const string MANUAL_REDIRECTION_TOOLTIP = "Payment status when the redirect button appears.\n" +
		                                                  "\"None\" — do not redirect.\n" +
		                                                  "\"Purchase using virtual currency\" — redirect when purchase is made using virtual currency.\n" +
		                                                  "\"Successful payment\" — redirect when a payment is successful.\n" +
		                                                  "\"Successful or canceled payment\" — redirect when a payment is successful or canceled.\n" +
		                                                  "\"Any payment\" — redirect for any payment status.";

		private const string REDIRECT_BUTTON_LABEL = "Redirect Button Caption";
		private const string REDIRECT_BUTTON_TOOLTIP = "Caption of the button that will redirect the user to the return URL.";

		private readonly string[] RedirectConditionsOptions = {
			"None",
			"Successful payment",
			"Successful or canceled payment",
			"Any payment"
		};

		private readonly string[] StatusForManualRedirectionOptions = {
			"None",
			"Purchase using virtual currency",
			"Successful payment",
			"Successful or canceled payment",
			"Any payment"
		};

		private bool PayStationSettings()
		{
			EditorGUILayout.Space();
			EditorGUILayout.BeginVertical(GroupAreaStyle);
			EditorGUILayout.LabelField("Pay Station", GroupHeaderStyle);

			var changed = false;

			var sandbox = EditorGUILayout.Toggle(new GUIContent(SANDBOX_LABEL, SANDBOX_TOOLTIP), XsollaSettings.IsSandbox);
			if (sandbox != XsollaSettings.IsSandbox)
			{
				XsollaSettings.IsSandbox = sandbox;
				changed = true;
			}

			EditorGUI.indentLevel++;

			var payStationFoldout = EditorGUILayout.Foldout(XsollaSettings.PayStationGroupFoldout, PAYSTATION_UI_GROUP_LABEL);
			if (payStationFoldout != XsollaSettings.PayStationGroupFoldout)
			{
				XsollaSettings.PayStationGroupFoldout = payStationFoldout;
				XsollaSettings.DesktopPayStationUISettings.isFoldout = false;
				XsollaSettings.WebglPayStationUISettings.isFoldout = false;
				XsollaSettings.AndroidPayStationUISettings.isFoldout = false;
				XsollaSettings.IosPayStationUISettings.isFoldout = false;
			}

			if (payStationFoldout)
			{
				DrawPayStationUISettings(XsollaSettings.DesktopPayStationUISettings, DESKTOP_GROUP_LABEL);
				DrawPayStationUISettings(XsollaSettings.WebglPayStationUISettings, WEBGL_GROUP_LABEL);
				DrawPayStationUISettings(XsollaSettings.AndroidPayStationUISettings, ANDROID_GROUP_LABEL);
				DrawPayStationUISettings(XsollaSettings.IosPayStationUISettings, IOS_GROUP_LABEL);
			}

			var redirectPolicyFoldout = EditorGUILayout.Foldout(XsollaSettings.RedirectPolicyGroupFoldout, REDIRECT_POLICY_GROUP_LABEL);
			if (redirectPolicyFoldout != XsollaSettings.RedirectPolicyGroupFoldout)
			{
				XsollaSettings.RedirectPolicyGroupFoldout = redirectPolicyFoldout;
				XsollaSettings.DesktopRedirectPolicySettings.IsFoldout = false;
				XsollaSettings.WebglRedirectPolicySettings.IsFoldout = false;
				XsollaSettings.AndroidRedirectPolicySettings.IsFoldout = false;
				XsollaSettings.IosRedirectPolicySettings.IsFoldout = false;
			}

			if (redirectPolicyFoldout)
			{
				DrawRedirectPolicySettings(XsollaSettings.DesktopRedirectPolicySettings, DESKTOP_GROUP_LABEL);
				DrawRedirectPolicySettings(XsollaSettings.WebglRedirectPolicySettings, WEBGL_GROUP_LABEL);
				DrawRedirectPolicySettings(XsollaSettings.AndroidRedirectPolicySettings, ANDROID_GROUP_LABEL);
				DrawRedirectPolicySettings(XsollaSettings.IosRedirectPolicySettings, IOS_GROUP_LABEL);
			}

			EditorGUI.indentLevel--;

			EditorGUILayout.EndVertical();
			return changed;
		}

		private void DrawPayStationUISettings(PayStationUISettings settings, string title)
		{
			EditorGUI.indentLevel++;

			settings.isFoldout = EditorGUILayout.Foldout(settings.isFoldout, title);
			if (settings.isFoldout)
			{
				settings.paystationThemeId = EditorGUILayout.TextField(new GUIContent(PAYSTATION_THEME_LABEL, PAYSTATION_THEME_TOOLTIP), settings.paystationThemeId);
				settings.paystationSize = (PayStationUISettings.PaystationSize) EditorGUILayout.EnumPopup(new GUIContent(PAYSTATION_SIZE_LABEL, PAYSTATION_SIZE_TOOLTIP), settings.paystationSize);
				settings.paystationVersion = (PayStationUISettings.PaystationVersion) EditorGUILayout.EnumPopup(new GUIContent(PAYSTATION_VERSION_LABEL, PAYSTATION_VERSION_TOOLTIP), settings.paystationVersion);
			}

			EditorGUI.indentLevel--;
		}

		private void DrawRedirectPolicySettings(RedirectPolicySettings settings, string title)
		{
			EditorGUI.indentLevel++;

			settings.IsFoldout = EditorGUILayout.Foldout(settings.IsFoldout, title);
			if (settings.IsFoldout)
			{
				settings.UseSettingsFromPublisherAccount = EditorGUILayout.Toggle(new GUIContent(OVERRIDE_REDIRECT_POLICY_LABEL, OVERRIDE_REDIRECT_POLICY_TOOLTIP), settings.UseSettingsFromPublisherAccount);
				GUI.enabled = !settings.UseSettingsFromPublisherAccount;

				settings.ReturnUrl = EditorGUILayout.TextField(new GUIContent(REDIRECT_URL_LABEL, REDIRECT_URL_TOOLTIP), settings.ReturnUrl);
				settings.RedirectConditions = (RedirectPolicySettings.RedirectConditionsType) EditorGUILayout.Popup(new GUIContent(REDIRECT_CONDITIONS_LABEL, REDIRECT_CONDITIONS_TOOLTIP), (int) settings.RedirectConditions, RedirectConditionsOptions);
				settings.Delay = EditorGUILayout.IntField(new GUIContent(REDIRECT_TIMEOUT_LABEL, REDIRECT_TIMEOUT_TOOLTIP), settings.Delay);
				settings.StatusForManualRedirection = (RedirectPolicySettings.StatusForManualRedirectionType) EditorGUILayout.Popup(new GUIContent(MANUAL_REDIRECTION_LABEL, MANUAL_REDIRECTION_TOOLTIP), (int) settings.StatusForManualRedirection, StatusForManualRedirectionOptions);
				settings.RedirectButtonCaption = EditorGUILayout.TextField(new GUIContent(REDIRECT_BUTTON_LABEL, REDIRECT_BUTTON_TOOLTIP), settings.RedirectButtonCaption);

				GUI.enabled = true;
			}

			EditorGUI.indentLevel--;
		}
	}
}
