using System;

public interface ILoginAuthorization
{
    Action<string> OnSuccess { get; set; }
	Action OnFailed { get; set; }
}
