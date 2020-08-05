using System;

public interface OldILoginAuthorization
{
    Action<string> OnSuccess { get; set; }
	Action OnFailed { get; set; }
}
