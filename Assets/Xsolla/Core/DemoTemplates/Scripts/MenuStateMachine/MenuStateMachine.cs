using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Xsolla.Core.Popup;

public partial class MenuStateMachine : MonoBehaviour, IMenuStateMachine
{
	public event Action<MenuState, MenuState> StateChangingEvent;
	[SerializeField] private Canvas canvas;
	[SerializeField] private MenuState initialState;
	[SerializeField] private GameObject authMenuPrefab;
	[SerializeField] private GameObject authFailedMenuPrefab;
	[SerializeField] private GameObject registrationMenuPrefab;
	[SerializeField] private GameObject registrationSuccessMenuPrefab;
	[SerializeField] private GameObject changePasswordMenuPrefab;
	[SerializeField] private GameObject changePasswordSuccessMenuPrefab;
	[SerializeField] private GameObject mainMenuPrefab;
	[SerializeField] private GameObject storeMenuPrefab;
	[SerializeField] private GameObject buyCurrencyMenuPrefab;
	[SerializeField] private GameObject cartMenuPrefab;
	[SerializeField] private GameObject inventoryMenuPrefab;
	[SerializeField] private GameObject profileMenuPrefab;

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
			{MenuState.RegistrationSuccess, registrationSuccessMenuPrefab},
			{MenuState.ChangePassword, changePasswordMenuPrefab},
			{MenuState.ChangePasswordSuccess, changePasswordSuccessMenuPrefab},
			{MenuState.Main, mainMenuPrefab},
			{MenuState.Store, storeMenuPrefab},
			{MenuState.BuyCurrency, buyCurrencyMenuPrefab},
			{MenuState.Cart, cartMenuPrefab},
			{MenuState.Inventory, inventoryMenuPrefab},
			{MenuState.Profile, profileMenuPrefab},
		};
		if (_stateMachine[initialState] == null)
		{
			PopupFactory.Instance.CreateError().
				SetMessage("Prefab object is null for initial state. Select other initial state or assign prefab object").
				SetCallback(() => Destroy(gameObject, 0.1F));
		} else
			SetState(initialState);
	}

	public void SetState(MenuState state)
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
			PopupFactory.Instance.CreateError().
				SetMessage($"Prefab object is null for state = {_state.ToString()}. Changing state to initial.").
				SetCallback(() =>
				{
					ClearTrace();
					SetState(initialState);
				});
		}
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
		if(_state == MenuState.Store || _state == MenuState.Inventory || _state == MenuState.Profile)
			SetState(MenuState.Main);
		return false;
	}
	
	private void HandleSomeCases(MenuState lastState, MenuState newState)
	{
		if(lastState == newState) return;
		if(lastState == MenuState.Cart && newState == MenuState.Inventory)
			ClearTrace();
		if(newState == MenuState.Main || newState == MenuState.Authorization)
			ClearTrace();
	}

	private void ClearTrace()
	{
		_stateTrace.Clear();
	}
}
