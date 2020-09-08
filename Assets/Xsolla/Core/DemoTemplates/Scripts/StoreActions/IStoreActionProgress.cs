using System;

public interface IStoreActionProgress
{
	Action OnStarted { get; set; }
	bool IsInProgress { get; }
}
