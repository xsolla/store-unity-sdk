using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Xsolla.Core.Popup;

public class MenuStateMachine : MonoBehaviour, IMenuStateMachine
{
	[SerializeField] private Canvas canvas;
	[SerializeField] private MenuState initialState;
	[SerializeField] private GameObject authMenuPrefab;
	[SerializeField] private GameObject mainMenuPrefab;
	[SerializeField] private GameObject storeMenuPrefab;
	[SerializeField] private GameObject buyCurrencyMenuPrefab;
	[SerializeField] private GameObject cartMenuPrefab;
	[SerializeField] private GameObject inventoryMenuPrefab;
	[SerializeField] private GameObject profileMenuPrefab;

	private Dictionary<MenuState, GameObject> _stateMachine;
	private List<MenuState> _stateTrace = new List<MenuState>();
	private MenuState _state;
	private GameObject _stateObject;

	private void Awake()
	{
		_stateObject = null;
		canvas = canvas ? canvas : GameObject.Find("Canvas").GetComponent<Canvas>();
		_stateMachine = new Dictionary<MenuState, GameObject>
		{
			{MenuState.Authorization, authMenuPrefab},
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

		if (_stateMachine[state] != null)
			_stateObject = Instantiate(_stateMachine[_state], canvas.transform);
		else
		{
			PopupFactory.Instance.CreateError().
				SetMessage($"Prefab object is null for state = {_state.ToString()}. Changing state to initial.").
				SetCallback(() => SetState(initialState));
		}
	}

	public void SetPreviousState()
	{
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
}
