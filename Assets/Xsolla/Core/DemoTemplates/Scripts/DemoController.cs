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
        if (lastState == MenuState.Main && newState == MenuState.Authorization)
        {
            if (UserInventory.IsExist)
                Destroy(UserInventory.Instance.gameObject);
            if (UserSubscriptions.IsExist)
                Destroy(UserSubscriptions.Instance.gameObject);
            if (OldUserAttributes.IsExist)
                Destroy(OldUserAttributes.Instance.gameObject);
        }

        if (
            (lastState == MenuState.Authorization
			|| lastState == MenuState.AuthorizationFailed
			|| lastState == MenuState.ChangePassword
			|| lastState == MenuState.ChangePasswordFailed
			|| lastState == MenuState.ChangePasswordSuccess
			|| lastState == MenuState.Registration
			|| lastState == MenuState.RegistrationFailed
			|| lastState == MenuState.RegistrationSuccess)
            && newState == MenuState.Main)
        {
            UpdateCatalogAndInventory();
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
            UserInventory.Instance.Refresh();
            // This method used for fastest async image loading
            StartLoadItemImages(UserCatalog.Instance.AllItems);
        });
    }
    
    private static void StartLoadItemImages(List<CatalogItemModel> items)
    {
        items.ForEach(i => ImageLoader.Instance.GetImageAsync(i.ImageUrl, null));
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
}
