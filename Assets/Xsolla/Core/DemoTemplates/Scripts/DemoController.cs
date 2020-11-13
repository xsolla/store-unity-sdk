using System.Collections.Generic;
using UnityEngine;
using Xsolla.Core;
using Xsolla.Core.Popup;
using Xsolla.Store;

public class DemoController : MonoSingleton<DemoController>, IMenuStateMachine
{
    [SerializeField]private MenuStateMachine stateMachine;
	[SerializeField]private UrlContainer _urlContainer;

	private IDemoImplementation _demoImplementation;

	public UrlContainer UrlContainer => _urlContainer;

    public event MenuStateMachine.StateChangeDelegate StateChangingEvent
    {
        add => stateMachine.StateChangingEvent += value;
        remove => stateMachine.StateChangingEvent -= value;
    }
    
    public override void Init()
    {
        base.Init();
        
        _demoImplementation = GetComponent<IDemoImplementation>();
        if (_demoImplementation == null)
        {
            PopupFactory.Instance.CreateError().
                SetMessage("DemoController object have not any script, that implements 'IDemoImplementation' interface. " +
                           "Implement this interface and attach to DemoController object.").
                SetCallback(() => Destroy(gameObject, 0.1F));
            return;
        }
        
        StateChangingEvent += OnStateChangingEvent;
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

			GetImplementation().Token = null;
		}

        if (lastState.IsAuthState() && newState == MenuState.Main)
        {
            UpdateCatalogAndInventory();
            UserFriends.Instance.UpdateFriends();
        }
    }
    
    private void UpdateCatalogAndInventory()
    {
        if(!UserInventory.IsExist)
            UserInventory.Instance.Init(_demoImplementation);
        if (!UserCatalog.IsExist)
            UserCatalog.Instance.Init(_demoImplementation);
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
        if (OldUserAttributes.IsExist)
            Destroy(OldUserAttributes.Instance.gameObject);
        if (UserSubscriptions.IsExist)
            Destroy(UserSubscriptions.Instance.gameObject);
        if(UserFriends.IsExist)
            Destroy(UserFriends.Instance.gameObject);
        if(PopupFactory.IsExist)
            Destroy(PopupFactory.Instance.gameObject);
        base.OnDestroy();
    }

    public IDemoImplementation GetImplementation()
    {
        return _demoImplementation;
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

    public string GetWebStoreUrl()
    {
        return $"{XsollaSettings.WebStoreUrl}?token={_demoImplementation.Token}&remember_me=false";
    }
}
