using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class XsollaSettingsValidator : MonoBehaviour
	{
		private void Start()
		{
			ValidateXsollaSettings();
		}

		private void ValidateXsollaSettings()
		{
			if (string.IsNullOrEmpty(XsollaSettings.LoginId))
			{
				var errorMessage = "Please copy the Login project ID from your Publisher Account and add it to your project settings";
				GenerateLoginSettingsError(errorMessage);
				return;
			}

			if (string.IsNullOrEmpty(XsollaSettings.StoreProjectId))
			{
				var errorMessage = "Please copy the Store project ID from your Publisher Account and add it to your project settings";
				GenerateLoginSettingsError(errorMessage);
				return;
			}

			var isDefaultLoginID = XsollaSettings.LoginId == Constants.DEFAULT_LOGIN_ID || XsollaSettings.LoginId == Constants.DEFAULT_PLATFORM_LOGIN_ID;
			var isDefaultProjectID = XsollaSettings.StoreProjectId == Constants.DEFAULT_PROJECT_ID;

			if (isDefaultLoginID && !isDefaultProjectID)
			{
				var errorMessage = string.Format("You changed [XsollaSettings->ProjectID] to '{0}', but did not change LoginID. Change LoginID from '{1}' to correct value.", XsollaSettings.StoreProjectId, XsollaSettings.LoginId);
				GenerateLoginSettingsError(errorMessage);
			}
			else if (!isDefaultLoginID && isDefaultProjectID)
			{
				var errorMessage = string.Format("You changed [XsollaSettings->LoginID] to '{0}', but did not change ProjectID. Change ProjectID from '{1}' to correct value.", XsollaSettings.LoginId, XsollaSettings.StoreProjectId);
				GenerateLoginSettingsError(errorMessage);
			}
		}

		private void GenerateLoginSettingsError(string errorMessage)
		{
			var proxyObject = new GameObject();
			var proxyScript = proxyObject.AddComponent<LoginSettingsErrorHolder>();
			proxyScript.LoginSettingsError = errorMessage;
			DemoController.Instance.SetState(MenuState.LoginSettingsError);
		}
	}
}
