using System;
using Xsolla;
using Xsolla.Login;

public interface OldILogin
{
    void Login();
    Action OnSuccessfulLogin { get; set; }
    Action<Xsolla.Core.Error> OnUnsuccessfulLogin { get; set; }
}
