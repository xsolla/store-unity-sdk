using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Xsolla.Core.Popup;

namespace Xsolla.Demo
{
	public partial class MenuStateMachine : MonoBehaviour
	{
		/// <summary>
		/// Changing state delegate.
		/// </summary>
		/// <param name="lastState">Previous state of state machine</param>
		/// <param name="newState">New/Current state of state machine</param>
		public delegate void StateChangeDelegate(MenuState lastState, MenuState newState);
		/// <summary>
		/// Invoked after state changed.
		/// </summary>
		public event StateChangeDelegate StateChangedEvent;

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
		[SerializeField] private GameObject battlepassMenuPrefab;
		[SerializeField] private GameObject inventoryMenuPrefab;
		[SerializeField] private GameObject profileMenuPrefab;
		[SerializeField] private GameObject characterMenuPrefab;
		[SerializeField] private GameObject characterMenuPrefabLoginSDK;
		[SerializeField] private GameObject friendsMenuPrefab;
		[SerializeField] private GameObject socialFriendsMenuPrefab;
		[SerializeField] private GameObject loginSettingsErrorPrefab;

		private Dictionary<MenuState, GameObject> _stateMachine;
		private readonly List<MenuState> _stateTrace = new List<MenuState>();
		private MenuState _currentState;
		private GameObject _stateObject;

		public MenuState MenuState
		{
			get
			{
				return _currentState;
			}
		}
		public GameObject StateObject
		{
			get
			{
				return _stateObject;
			}
		}
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
				{MenuState.Battlepass, battlepassMenuPrefab},
				{MenuState.Inventory, inventoryMenuPrefab},
				{MenuState.Profile, profileMenuPrefab},
				{MenuState.Character, characterMenuPrefab},
				{MenuState.Friends, friendsMenuPrefab},
				{MenuState.SocialFriends, socialFriendsMenuPrefab},
				{MenuState.LoginSettingsError, loginSettingsErrorPrefab},
			};

			if (DemoMarker.IsLoginDemo)
				_stateMachine[MenuState.Character] = characterMenuPrefabLoginSDK;

			if (_stateMachine[initialState] == null)
			{
				PopupFactory.Instance.CreateError().
					SetMessage("Prefab object is null for MenuStateMachine's initial state. Select other initial state or assign prefab object").
					SetCallback(() => Destroy(gameObject, 0.1F));
			}
		}

		public GameObject SetInitialState()
		{
			ClearTrace();
			return SetState(initialState, addToTrace: false);
		}

		public GameObject SetState(MenuState state)
		{
			return SetState(state, addToTrace: true);
		}

		private GameObject SetState(MenuState state, bool addToTrace)
		{
			MenuState previousState = _currentState;

			if (addToTrace && _currentState != state)
			{
				_stateTrace.Add(_currentState);
			}

			_currentState = state;

			if (_stateObject != null)
			{
				Destroy(_stateObject);
				_stateObject = null;
			}

			if (_stateMachine[state] != null)
			{
				_stateObject = Instantiate(_stateMachine[state], canvas.transform);
				ClearTraceIfNeeded(previousState, state);
				if (StateChangedEvent != null)
					StateChangedEvent.Invoke(previousState, state);
			}
			else
			{
				Debug.LogError(string.Format("Prefab object is null for state = {0}. Changing state to initial.", state.ToString()));
				SetInitialState();
			}

			return _stateObject;
		}

		public void SetPreviousState()
		{
			if (_currentState.IsPostAuthState() &&
				_currentState != MenuState.Main &&
				_currentState != MenuState.BuyCurrency &&
				_currentState != MenuState.Cart)
			{
				SetState(MenuState.Main);
			}
			else if (_stateTrace.Any())
			{
				var state = _stateTrace.Last();
				_stateTrace.RemoveAt(_stateTrace.Count - 1);
				SetState(state, addToTrace: false);
			}
			else
				SetState(initialState);
		}

		public bool IsStateAvailable(MenuState state)
		{
			return _stateMachine[state] != null;
		}

		public void ClearTrace()
		{
			_stateTrace.Clear();
		}

		private void ClearTraceIfNeeded(MenuState lastState, MenuState newState)
		{
			if (lastState == newState) return;
			if (lastState == MenuState.Cart && newState == MenuState.Inventory)
				ClearTrace();
			if (newState.IsAuthState() ||
				newState == MenuState.Main ||
				newState == MenuState.Friends ||
				newState == MenuState.SocialFriends)
				ClearTrace();
		}
	}
}
