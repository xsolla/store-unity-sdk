using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Demo;
using Xsolla.UIBuilder;

namespace Xsolla.Core
{
	public class DevMenuPage : MonoBehaviour
	{
		[SerializeField] private SimpleButton BackButton = default;
		[SerializeField] private SimpleButton ResetButton = default;
		[SerializeField] private InputField LoginIdInput = default;
		[SerializeField] private InputField StoreProjectIdInput = default;
		[SerializeField] private InputField OAuthClientIdInput = default;
		[SerializeField] private Toggle JwtToggle = default;
		[SerializeField] private Toggle OAuthToggle = default;
		[SerializeField] private InputField WebShopUrlInput = default;
		[SerializeField] private Dropdown UiThemeDropdown = default;

		private void OnEnable()
		{
			BackButton.onClick += OnBackButtonClick;
			ResetButton.onClick += OnResetButtonClick;

			LoginIdInput.onEndEdit.AddListener(OnLoginIdInputEndEdit);
			StoreProjectIdInput.onEndEdit.AddListener(OnStoreProjectIdInputEndEdit);
			OAuthClientIdInput.onEndEdit.AddListener(OnOAuthClientIdInputInputEndEdit);
			WebShopUrlInput.onEndEdit.AddListener(OnWebShopUrlEdit);
			UiThemeDropdown.onValueChanged.AddListener(OnUiThemeDropdownChanged);

			JwtToggle.onValueChanged.AddListener(OnJwtToggleChanged);
			OAuthToggle.onValueChanged.AddListener(OnOAuthToggleChanged);
		}

		private void OnDisable()
		{
			BackButton.onClick -= OnBackButtonClick;
			ResetButton.onClick -= OnResetButtonClick;

			LoginIdInput.onEndEdit.RemoveListener(OnLoginIdInputEndEdit);
			StoreProjectIdInput.onEndEdit.RemoveListener(OnStoreProjectIdInputEndEdit);
			OAuthClientIdInput.onEndEdit.RemoveListener(OnOAuthClientIdInputInputEndEdit);
			WebShopUrlInput.onEndEdit.RemoveListener(OnWebShopUrlEdit);
			UiThemeDropdown.onValueChanged.RemoveListener(OnUiThemeDropdownChanged);

			JwtToggle.onValueChanged.RemoveListener(OnJwtToggleChanged);
			OAuthToggle.onValueChanged.RemoveListener(OnOAuthToggleChanged);
		}

		private void Start()
		{
			RedrawFields();
		}

		private void OnBackButtonClick()
		{
			Destroy(gameObject);
		}

		private void OnResetButtonClick()
		{
			XsollaSettings.LoginId = XsollaSettings.Instance.loginId;
			XsollaSettings.StoreProjectId = XsollaSettings.Instance.storeProjectId;
			XsollaSettings.AuthorizationType = XsollaSettings.Instance.authorizationType;
			XsollaSettings.OAuthClientId = XsollaSettings.Instance.oauthClientId;
			DemoSettings.WebStoreUrl = DemoSettings.Instance.webStoreUrl;
			ThemesLibrary.Current = ThemesLibrary.Themes.FirstOrDefault();

			RedrawFields();
		}

		private void OnLoginIdInputEndEdit(string value)
		{
			XsollaSettings.LoginId = value;
			RedrawFields();
		}

		private void OnStoreProjectIdInputEndEdit(string value)
		{
			XsollaSettings.StoreProjectId = value;
			RedrawFields();
		}

		private void OnOAuthClientIdInputInputEndEdit(string value)
		{
			if (int.TryParse(value, out var oAuthClientId))
				XsollaSettings.OAuthClientId = oAuthClientId;

			RedrawFields();
		}

		private void OnJwtToggleChanged(bool isOn)
		{
			if (isOn)
				XsollaSettings.AuthorizationType = AuthorizationType.JWT;

			RedrawFields();
		}

		private void OnOAuthToggleChanged(bool isOn)
		{
			if (isOn)
				XsollaSettings.AuthorizationType = AuthorizationType.OAuth2_0;

			RedrawFields();
		}

		private void OnWebShopUrlEdit(string value)
		{
			DemoSettings.WebStoreUrl = value;
			RedrawFields();
		}

		private void OnUiThemeDropdownChanged(int index)
		{
			ThemesLibrary.Current = ThemesLibrary.Themes[index];
			SaveUiTheme();
			RedrawFields();
		}

		private void RedrawFields()
		{
			LoginIdInput.text = XsollaSettings.LoginId;
			StoreProjectIdInput.text = XsollaSettings.StoreProjectId;
			OAuthClientIdInput.text = XsollaSettings.OAuthClientId.ToString();
			WebShopUrlInput.text = DemoSettings.WebStoreUrl;

			var uiThemes = ThemesLibrary.Themes;
			UiThemeDropdown.options = uiThemes.Select(theme => new Dropdown.OptionData(theme.Name)).ToList();

			var uiThemeIndex = uiThemes.IndexOf(ThemesLibrary.Current);
			if (uiThemeIndex < 0)
				uiThemeIndex = 0;
			UiThemeDropdown.SetValueWithoutNotify(uiThemeIndex);

			var authorizationType = XsollaSettings.AuthorizationType;
			JwtToggle.SetIsOnWithoutNotify(authorizationType == AuthorizationType.JWT);
			OAuthToggle.SetIsOnWithoutNotify(authorizationType == AuthorizationType.OAuth2_0);

			var oAuthInputParent = OAuthClientIdInput.transform.parent.gameObject;
			oAuthInputParent.SetActive(authorizationType == AuthorizationType.OAuth2_0);
		}

		private const string UiThemeOverrideKey = "UiThemeOverride";

		private static void SaveUiTheme()
		{
			PlayerPrefs.SetString(UiThemeOverrideKey, ThemesLibrary.Current.Id);
		}

		public static void TryLoadUiTheme()
		{
			if (!PlayerPrefs.HasKey(UiThemeOverrideKey))
				return;

			var id = PlayerPrefs.GetString(UiThemeOverrideKey);
			ThemesLibrary.SetCurrentThemeById(id);
		}
	}
}