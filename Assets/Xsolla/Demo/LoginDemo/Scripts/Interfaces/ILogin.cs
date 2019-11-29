using System;
using Xsolla;
using Xsolla.Login;

public interface ILogin
{
    void Login();
    Action<User> OnSuccessfulLogin { get; set; }
    Action<Xsolla.Core.Error> OnUnsuccessfulLogin { get; set; }
}
