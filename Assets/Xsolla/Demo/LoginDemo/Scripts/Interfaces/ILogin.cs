using System;
using Xsolla;
using Xsolla.Login;

public interface ILogin
{
    void Login();
    Action<XsollaUser> OnSuccessfulLogin { get; set; }
    Action<ErrorDescription> OnUnsuccessfulLogin { get; set; }
}
