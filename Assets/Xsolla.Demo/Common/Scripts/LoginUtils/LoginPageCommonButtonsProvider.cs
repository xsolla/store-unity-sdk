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
				var isDefaultProject = XsollaSettings.LoginId == Constants.DEFAULT_LOGIN_ID &&
									   XsollaSettings.StoreProjectId == Constants.DEFAULT_PROJECT_ID;
				DemoUserButton.gameObject.SetActive(isDefaultProject);
			}
		}
	}
}