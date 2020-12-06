using System.Collections.Generic;
using UnityEngine;
using Xsolla.Core;
using Xsolla.Core.Popup;
using Xsolla.Store;

namespace Xsolla.Demo
{
	public class DemoController : MonoSingleton<DemoController>, IMenuStateMachine
	{
		[SerializeField]private MenuStateMachine stateMachine = default;
		[SerializeField]private UrlContainer _urlContainer = default;

		private TutorialManager _tutorialManager;

		public ILoginDemoImplementation LoginDemo { get; private set; }
		public IStoreDemoImplementation StoreDemo { get; private set; }
		public IInventoryDemoImplementation InventoryDemo { get; private set; }

		public UrlContainer UrlContainer => _urlContainer;
		public TutorialManager TutorialManager => _tutorialManager;

		public bool IsTutorialAvailable => _tutorialManager != null;

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

			if (InventoryDemo != null)
				_tutorialManager = GetComponent<TutorialManager>();
		}

		private void OnStateChangingEvent(MenuState lastState, MenuState newState)
		{
			if (lastState.IsPostAuthState() && newState.IsAuthState())
			{
				if(UserFriends.IsExist)
					Destroy(UserFriends.Instance.gameObject);
				if (UserInventory.IsExist)
					Destroy(UserInventory.Instance.gameObject);
				if (UserSubscriptions.IsExist)
					Destroy(UserSubscriptions.Instance.gameObject);
				if (UserCart.IsExist)
					Destroy(UserCart.Instance.gameObject);

				LoginDemo.Token = null;
			}

			if (lastState.IsAuthState() && newState == MenuState.Main)
			{
				UpdateCatalogAndInventory();
				UserFriends.Instance.UpdateFriends();

				if (_tutorialManager != null)
				{
					if (!_tutorialManager.IsTutorialCompleted())
					{
						_tutorialManager.ShowTutorial();
					}
					else
					{
						Debug.Log("Skipping tutorial since it was already completed.");
					}
				}
				else
				{
					Debug.LogWarning("Tutorial is not available for this demo.");
				}
			}
		}

		private void UpdateCatalogAndInventory()
		{
			if(!UserInventory.IsExist)
				UserInventory.Instance.Init(InventoryDemo);
			if (!UserCatalog.IsExist)
				UserCatalog.Instance.Init(StoreDemo, InventoryDemo);
			UserCatalog.Instance.UpdateItems(() =>
			{
				if (UserInventory.IsExist)
				{
					UserInventory.Instance.Refresh();
					// This method used for fastest async image loading
					StartLoadItemImages(UserCatalog.Instance.AllItems);
				}
			});
		}

		private static void StartLoadItemImages(List<CatalogItemModel> items)
		{
			items.ForEach(i =>
			{
				if (!string.IsNullOrEmpty(i.ImageUrl))
				{
					ImageLoader.Instance.GetImageAsync(i.ImageUrl, null);    
				}
				else
				{
					Debug.LogError($"Catalog item with sku = '{i.Sku}' without image!");
				}
			});
		}

		protected override void OnDestroy()
		{
			if (UserCatalog.IsExist)
				Destroy(UserCatalog.Instance.gameObject);
			if (UserInventory.IsExist)
				Destroy(UserInventory.Instance.gameObject);
			if (UserCart.IsExist)
				Destroy(UserCart.Instance.gameObject);
			if (UserSubscriptions.IsExist)
				Destroy(UserSubscriptions.Instance.gameObject);
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
	}
}
