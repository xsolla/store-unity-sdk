using UnityEngine;
using Xsolla.Core;
using Xsolla.Core.Popup;
using Xsolla.Store;

public class DemoController : MonoSingleton<DemoController>, IMenuStateMachine
{
    [SerializeField]private MenuStateMachine stateMachine;
    [SerializeField]public string documentationUrl;
    [SerializeField]public string feedbackUrl;
    [SerializeField]public string publisherUrl;
    [SerializeField]public string testCardsUrl = 
        "https://developers.xsolla.com/api/v1/pay-station/#api_payment_ui_test_cards";

    private IDemoImplementation _demoImplementation;

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
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (UserCatalog.IsExist)
            Destroy(UserCatalog.Instance.gameObject);
        if (UserInventory.IsExist)
            Destroy(UserInventory.Instance.gameObject);
        if (UserCart.IsExist)
            Destroy(UserCart.Instance.gameObject);
        if (UserInventory.IsExist)
            Destroy(UserInventory.Instance.gameObject);
        if (OldUserAttributes.IsExist)
            Destroy(OldUserAttributes.Instance.gameObject);
        if (UserSubscriptions.IsExist)
            Destroy(UserSubscriptions.Instance.gameObject);
    }

    public IDemoImplementation GetImplementation()
    {
        return _demoImplementation;
    }

    public void SetState(MenuState state)
    {
        if (stateMachine != null)
            stateMachine.SetState(state);
    }

    public void SetPreviousState()
    {
        if (stateMachine != null)
            stateMachine.SetPreviousState();
    }
}
