using System;
using Xsolla;
using Xsolla.Login;

public interface ISignUp
{
    void SignUp();
    string SignUpEmail { get; }
    Action OnSuccessfulSignUp { get; set; }
    Action<ErrorDescription> OnUnsuccessfulSignUp { get; set; }
}
