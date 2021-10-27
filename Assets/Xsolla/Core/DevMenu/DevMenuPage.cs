using UnityEngine;
using UnityEngine.UI;
using Xsolla.Demo;

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

		private void OnEnable()
		{
			BackButton.onClick += OnBackButtonClick;
			ResetButton.onClick += OnResetButtonClick;

			LoginIdInput.onEndEdit.AddListener(OnLoginIdInputEndEdit);
			StoreProjectIdInput.onEndEdit.AddListener(OnStoreProjectIdInputEndEdit);
			OAuthClientIdInput.onEndEdit.AddListener(OnOAuthClientIdInputInputEndEdit);
			WebShopUrlInput.onEndEdit.AddListener(OnWebShopUrlEdit);

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
			XsollaSettings.WebStoreUrl = XsollaSettings.Instance.webStoreUrl;

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
			XsollaSettings.WebStoreUrl = value;
			RedrawFields();
		}

		private void RedrawFields()
		{
			LoginIdInput.text = XsollaSettings.LoginId;
			StoreProjectIdInput.text = XsollaSettings.StoreProjectId;
			OAuthClientIdInput.text = XsollaSettings.OAuthClientId.ToString();
			WebShopUrlInput.text = XsollaSettings.WebStoreUrl;

			var authorizationType = XsollaSettings.AuthorizationType;
			JwtToggle.SetIsOnWithoutNotify(authorizationType == AuthorizationType.JWT);
			OAuthToggle.SetIsOnWithoutNotify(authorizationType == AuthorizationType.OAuth2_0);

			var oAuthInputParent = OAuthClientIdInput.transform.parent.gameObject;
			oAuthInputParent.SetActive(authorizationType == AuthorizationType.OAuth2_0);
		}
	}
}
