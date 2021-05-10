using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;
using Xsolla.Login;

namespace Xsolla.Demo
{
	public class DeviceIDPageManager : MonoBehaviour
    {
		[SerializeField] private InputField EmailInput = default;
		[SerializeField] private InputField PasswrodInput = default;
		[SerializeField] private InputField UsernameInput = default;
		[SerializeField] private Button AddButton = default;
		[SerializeField] private Toggle SendAgreeToggle = default;
		[SerializeField] private Toggle AgreeToggle = default;
		[Space]
		[SerializeField] private InputField DeviceIdInput = default;
		[SerializeField] private InputField DeviceNameInput = default;
		[SerializeField] private Toggle AndroidToggle = default;
		[SerializeField] private Button AddDeviceIDButton = default;
		[Space]
		[SerializeField] private InputField IndexInput = default;
		[SerializeField] private Button RemoveDeviceIDButton = default;
		[Space]
		[SerializeField] private GameObject DeviceIDPrefab = default;
		[SerializeField] private Transform ScrollViewRoot = default;

		private List<int> _xsollaDeviceIDs = new List<int>();

		private void Awake()
		{
			AddButton.onClick.AddListener(AddEmailPassUsername);
			AddDeviceIDButton.onClick.AddListener(AddDeviceID);
			RemoveDeviceIDButton.onClick.AddListener(RemoveDeviceID);
		}

		private void Start()
		{
			ShowAddedDeviceIDs();
		}

		private void ShowAddedDeviceIDs()
		{
			//Delete previous if any
			_xsollaDeviceIDs.Clear();
			List<GameObject> toDelete = new List<GameObject>(ScrollViewRoot.childCount);
			for (int i = 0; i < ScrollViewRoot.childCount; i++)
				toDelete.Add(ScrollViewRoot.GetChild(i).gameObject);

			foreach (var item in toDelete)
				Destroy(item);

			//Prepare outcome handlers
			Action<List<UserDeviceInfo>> onSuccess = devices =>
			{
				foreach (var device in devices)
				{
					var uiObject = Instantiate(DeviceIDPrefab, ScrollViewRoot);
					var uiScript = uiObject.GetComponent<AttributeItem>();
					uiScript.IsReadOnly = true;
					uiScript.Key = $"{device.type}:{device.last_used_at}";
					uiScript.Value = device.device;
					_xsollaDeviceIDs.Add(device.id);
				}
			};

			Action<Error> onError = error =>
			{
				StoreDemoPopup.ShowError(error);
			};

			//Get current devices
			XsollaLogin.Instance.GetUserDevices(onSuccess, onError);
		}

		private void AddEmailPassUsername()
		{
			var email = EmailInput.text;
			var password = PasswrodInput.text;
			var username = UsernameInput.text;
			int? sendSpam = null;

			if (SendAgreeToggle.isOn)
				sendSpam = AgreeToggle.isOn ? 1 : 0;

			Action<bool> onSuccess = isEmailConfirmationRequired =>
			{
				var message = $"Details added, email confirmation required: {isEmailConfirmationRequired}";
				StoreDemoPopup.ShowConfirm(confirmCase: ()=>{ }, message: message);
			};

			Action<Error> onError = error =>
			{
				StoreDemoPopup.ShowError(error);
			};

			XsollaLogin.Instance.AddUsernameEmailAuthToAccount(email, password, username, sendSpam, onSuccess, onError);
		}

		private void AddDeviceID()
		{
			var deviceType = AndroidToggle.isOn ? Core.DeviceType.Android : Core.DeviceType.iOS;
			var deviceName = DeviceNameInput.text;
			var deviceID = DeviceIdInput.text;

			Action onSuccess = () =>
			{
				var message = $"Device added successfully";
				StoreDemoPopup.ShowConfirm(confirmCase: () => { }, message: message);
				ShowAddedDeviceIDs();
			};

			Action<Error> onError = error =>
			{
				StoreDemoPopup.ShowError(error);
			};

			XsollaLogin.Instance.LinkDeviceToAccount(deviceType, deviceName, deviceID, onSuccess, onError);
		}

		private void RemoveDeviceID()
		{
			var indexAsString = IndexInput.text;
			var id = default(int);

			try
			{
				var index = int.Parse(indexAsString);
				id = _xsollaDeviceIDs[index];
			}
			catch (Exception ex)
			{
				StoreDemoPopup.ShowError(new Error(errorMessage: ex.Message));
				return;
			}

			Action onSuccess = () =>
			{
				var message = $"Device removed successfully";
				StoreDemoPopup.ShowConfirm(confirmCase: () => { }, message: message);
				ShowAddedDeviceIDs();
			};

			Action<Error> onError = error =>
			{
				StoreDemoPopup.ShowError(error);
			};

			XsollaLogin.Instance.UnlinkDeviceFromAccount(id, onSuccess, onError);
		}
	}
}
