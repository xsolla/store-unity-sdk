using System.Collections.Generic;
using UnityEngine;
using Xsolla.Core;
using Xsolla.Core.Popup;

namespace Xsolla.Demo
{
	public partial class DemoController : MonoSingleton<DemoController>, IMenuStateMachine
	{
		[SerializeField]private MenuStateMachine stateMachine = default;
		[SerializeField]private UrlContainer _urlContainer = default;

		public ILoginDemoImplementation LoginDemo { get; private set; }
		public IStoreDemoImplementation StoreDemo { get; private set; }
		public IInventoryDemoImplementation InventoryDemo { get; private set; }

		public UrlContainer UrlContainer => _urlContainer;
		public bool IsTutorialAvailable { get; private set; } = false;

		public event MenuStateMachine.StateChangeDelegate StateChangingEvent
		{
			add => stateMachine.StateChangingEvent += value;
			remove => stateMachine.StateChangingEvent -= value;
		}

		public override void Init()
		{
			base.Init();

			LoginDemo = GetComponent<ILoginDemoImplementation>();
			StoreDemo = GetComponent<IStoreDemoImplementation>();
			InventoryDemo = GetComponent<IInventoryDemoImplementation>();

			StateChangingEvent += OnStateChangingEvent;
			InitInventory();
		}

		private void OnStateChangingEvent(MenuState lastState, MenuState newState)
		{
			if (lastState.IsPostAuthState() && newState.IsAuthState())
			{
				if(UserFriends.IsExist)
					Destroy(UserFriends.Instance.gameObject);

				DestroyInventory();
				DestroyStore();

				LoginDemo.Token = null;
			}

			if (lastState.IsAuthState() && newState == MenuState.Main)
			{
				UpdateCatalogAndInventory();
				UserFriends.Instance.UpdateFriends();

				AutoStartTutorial();
			}
		}

		private void UpdateCatalogAndInventory()
		{
			UpdateStore();
			UpdateInventory();
		}

		protected override void OnDestroy()
		{
			DestroyStore();
			DestroyInventory();

			if(UserFriends.IsExist)
				Destroy(UserFriends.Instance.gameObject);
			if(PopupFactory.IsExist)
				Destroy(PopupFactory.Instance.gameObject);
			base.OnDestroy();
		}

		public GameObject SetState(MenuState state)
		{
			if (stateMachine != null)
				stateMachine.SetState(state);
			return null;
		}

		public void SetPreviousState()
		{
			if (stateMachine != null)
				stateMachine.SetPreviousState();
		}

		public bool IsStateAvailable(MenuState state)
		{
			return stateMachine.IsStateAvailable(state);
		}

		public string GetWebStoreUrl()
		{
			return $"{XsollaSettings.WebStoreUrl}?token={LoginDemo.Token}&remember_me=false";
		}

		public void ShowTutorial(bool showWelcomeMessage) => ManualStartTutorial(showWelcomeMessage);

		partial void InitInventory();
		partial void AutoStartTutorial();
		partial void ManualStartTutorial(bool showWelcomeMessage);

		partial void UpdateInventory();
		partial void UpdateStore();

		partial void DestroyInventory();
		partial void DestroyStore();
	}
}
