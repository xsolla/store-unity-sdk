using UnityEditor;
using UnityEngine;
using Xsolla.Core;
using Xsolla.Core.Popup;

public class UserInterfaceFSM : MonoSingleton<UserInterfaceFSM>
{
    [SerializeField] private MonoScript _demoImplementation;
    [SerializeField] private Canvas _canvas;

    public override void Init()
    {
        base.Init();
        if (_demoImplementation == null)
        {
            PopupFactory.Instance.CreateError().SetMessage(
                "Demo implementation is null! " +
                "Implement 'IDemoImplementation' interface before use this demo template.");
        }
    }
}
