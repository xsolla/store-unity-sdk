using System;
using Xsolla;
using Xsolla.Login;

public interface OldISignUp
{
    void SignUp();
    string SignUpEmail { get; }
    Action OnSuccessfulSignUp { get; set; }
    Action<Xsolla.Core.Error> OnUnsuccessfulSignUp { get; set; }
}
