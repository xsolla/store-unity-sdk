using UnityEditor;
using UnityEngine;

namespace Xsolla.Core
{
	public partial class XsollaSettingsEditor
	{
		private const string APPLE_PAY_MERCHANT_DOMAIN_LABEL = "Apple Pay Merchant Domain";
		private const string APPLE_PAY_MERCHANT_DOMAIN_TOOLTIP = "Domain of your Apple Pay merchant. "
			+ "Used to validate the merchant session.";

		private bool AdvancedSettings()
		{
			var changed = false;

			EditorGUILayout.Space();
			EditorGUI.indentLevel++;
			EditorGUILayout.BeginVertical(GroupAreaStyle);

			XsollaSettings.AdvancedGroupFoldout = EditorGUILayout.Foldout(XsollaSettings.AdvancedGroupFoldout, "Advanced Settings");
			if (XsollaSettings.AdvancedGroupFoldout)
			{
				var applePayMerchantDomain = EditorGUILayout.TextField(new GUIContent(APPLE_PAY_MERCHANT_DOMAIN_LABEL, APPLE_PAY_MERCHANT_DOMAIN_TOOLTIP), XsollaSettings.ApplePayMerchantDomain);
				if (applePayMerchantDomain != XsollaSettings.ApplePayMerchantDomain)
				{
					XsollaSettings.ApplePayMerchantDomain = applePayMerchantDomain;
					changed = true;
				}
			}

			EditorGUILayout.EndVertical();
			EditorGUI.indentLevel--;

			return changed;
		}
	}
}