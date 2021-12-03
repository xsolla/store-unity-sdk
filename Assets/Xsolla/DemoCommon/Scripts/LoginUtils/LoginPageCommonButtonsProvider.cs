using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class LoginPageCommonButtonsProvider : MonoBehaviour
	{
		public SimpleButton OKButton;
		public SimpleButton DemoUserButton;
		public SimpleButton LogInButton;

		private void OnEnable()
		{
			XsollaSettings.Changed += OnXsollaSettingsChanged;
		}

		private void OnDisable()
		{
			XsollaSettings.Changed -= OnXsollaSettingsChanged;
		}

		private void Start()
		{
			OnXsollaSettingsChanged();
		}

		private void OnXsollaSettingsChanged()
		{
			if (DemoUserButton)
			{
				var isChanged = IsSettingsChanged();
				DemoUserButton.gameObject.SetActive(!isChanged);
			}
		}

		private bool IsSettingsChanged()
		{
			if (XsollaSettings.LoginId != Constants.DEFAULT_LOGIN_ID)
				return true;

			if (XsollaSettings.StoreProjectId != Constants.DEFAULT_PROJECT_ID)
				return true;

			return false;
		}
	}
}