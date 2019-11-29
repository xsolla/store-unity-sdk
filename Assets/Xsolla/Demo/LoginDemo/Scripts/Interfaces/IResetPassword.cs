using System;
using Xsolla;
using Xsolla.Login;

public interface IResetPassword
{
    void ResetPassword();
    Action OnSuccessfulResetPassword { get; set; }
    Action<Xsolla.Core.Error> OnUnsuccessfulResetPassword { get; set; }
}
