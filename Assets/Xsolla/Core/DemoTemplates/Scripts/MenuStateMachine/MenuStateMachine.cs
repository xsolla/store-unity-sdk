using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Xsolla.Core.Popup;

public partial class MenuStateMachine : MonoBehaviour, IMenuStateMachine
{
	/// <summary>
	/// Changing state delegate.
	/// </summary>
	/// <param name="lastState">Previous state of state machine</param>
	/// <param name="newState">New/Current state of state machine</param>
	public delegate void StateChangeDelegate(MenuState lastState, MenuState newState);
	/// <summary>
	/// Invoked before state is changing.
	/// </summary>
	public event StateChangeDelegate StateChangingEvent;
	
	[SerializeField] private Canvas canvas;
	[SerializeField] private MenuState initialState;
	[SerializeField] private GameObject authMenuPrefab;
	[SerializeField] private GameObject authFailedMenuPrefab;
	[SerializeField] private GameObject registrationMenuPrefab;
	[SerializeField] private GameObject registrationFailedMenuPrefab;
	[SerializeField] private GameObject registrationSuccessMenuPrefab;
	[SerializeField] private GameObject changePasswordMenuPrefab;
	[SerializeField] private GameObject changePasswordFailedMenuPrefab;
	[SerializeField] private GameObject changePasswordSuccessMenuPrefab;
	[SerializeField] private GameObject mainMenuPrefab;
	[SerializeField] private GameObject storeMenuPrefab;
	[SerializeField] private GameObject buyCurrencyMenuPrefab;
	[SerializeField] private GameObject cartMenuPrefab;
	[SerializeField] private GameObject inventoryMenuPrefab;
	[SerializeField] private GameObject profileMenuPrefab;
	[SerializeField] private GameObject friendsMenuPrefab;
	[SerializeField] private GameObject loginSettingsErrorPrefab;

	private Dictionary<MenuState, GameObject> _stateMachine;
	private readonly List<MenuState> _stateTrace = new List<MenuState>();
	private MenuState _state;
	private GameObject _stateObject;

	private void Awake()
	{
		_stateObject = null;
		canvas = canvas ? canvas : GameObject.Find("Canvas").GetComponent<Canvas>();
		_stateMachine = new Dictionary<MenuState, GameObject>
		{
			{MenuState.Authorization, authMenuPrefab},
			{MenuState.AuthorizationFailed, authFailedMenuPrefab},
			{MenuState.Registration, registrationMenuPrefab},
			{MenuState.RegistrationFailed, registrationFailedMenuPrefab},
			{MenuState.RegistrationSuccess, registrationSuccessMenuPrefab},
			{MenuState.ChangePassword, changePasswordMenuPrefab},
			{MenuState.ChangePasswordFailed, changePasswordFailedMenuPrefab},
			{MenuState.ChangePasswordSuccess, changePasswordSuccessMenuPrefab},
			{MenuState.Main, mainMenuPrefab},
			{MenuState.Store, storeMenuPrefab},
			{MenuState.BuyCurrency, buyCurrencyMenuPrefab},
			{MenuState.Cart, cartMenuPrefab},
			{MenuState.Inventory, inventoryMenuPrefab},
			{MenuState.Profile, profileMenuPrefab},
			{MenuState.Friends, friendsMenuPrefab},
			{MenuState.LoginSettingsError, loginSettingsErrorPrefab},
		};
		if (_stateMachine[initialState] == null)
		{
			PopupFactory.Instance.CreateError().
				SetMessage("Prefab object is null for initial state. Select other initial state or assign prefab object").
				SetCallback(() => Destroy(gameObject, 0.1F));
		} else
			SetState(initialState);
	}

	public GameObject SetState(MenuState state)
	{
		MenuState oldState = _state;
		
		if (_state != state)
		{
			_stateTrace.Add(_state);
			_state = state;
		}
		
		if (_stateObject != null)
		{
			Destroy(_stateObject);
			_stateObject = null;
		}

		if (_stateMachine[_state] != null)
		{
			StateChangingEvent?.Invoke(oldState, _state);
			_stateObject = Instantiate(_stateMachine[_state], canvas.transform);
			CheckAuthorizationState(_state, _stateObject);
			HandleSomeCases(oldState, _state);
		}
		else
		{
			Debug.LogError($"Prefab object is null for state = {_state.ToString()}. Changing state to initial.");
			ClearTrace();
			SetState(initialState);
		}
		return _stateObject;
	}
	
	public void SetPreviousState()
	{
		if(CheckHardcodedBackCases()) return;
		if (_stateTrace.Any())
		{
			var state = _stateTrace.Last();
			_stateTrace.RemoveAt(_stateTrace.Count - 1);
			_state = state;
			SetState(state);
		}
		else
			SetState(initialState);
	}

	private bool CheckHardcodedBackCases()
	{
		if (_state == MenuState.Store || _state == MenuState.Inventory || _state == MenuState.Profile || _state == MenuState.Friends)
		{
			SetState(MenuState.Main);
			return true;
		}
		return false;
	}
	
	private void HandleSomeCases(MenuState lastState, MenuState newState)
	{
		if(lastState == newState) return;
		if(lastState == MenuState.Cart && newState == MenuState.Inventory)
			ClearTrace();
		if(newState == MenuState.Main || newState == MenuState.Authorization)
			ClearTrace();
		if(lastState == MenuState.Registration && newState == MenuState.Authorization)
		{
			var proxyScript = FindObjectOfType<LoginCreateToAuthProxyRequestHolder>();
			var loginEnterScript = _stateObject.GetComponent<LoginPageEnterController>();

			if(proxyScript != null && loginEnterScript != null)
				loginEnterScript.RunLoginAction(proxyScript.ProxyRequest, proxyScript.ProxyArgument);

			if(proxyScript != null)
				Destroy(proxyScript.gameObject);
		}
		if (newState == MenuState.LoginSettingsError)
		{
			var proxyScript = FindObjectOfType<LoginSettingsErrorHolder>();
			var errorShower = _stateObject.GetComponent<LoginPageErrorShower>();

			if (proxyScript != null && errorShower != null)
				errorShower.ShowError(proxyScript.LoginSettingsError);

			if (proxyScript != null)
				Destroy(proxyScript.gameObject);
		}
	}

	private void ClearTrace()
	{
		_stateTrace.Clear();
	}
}
