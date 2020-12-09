using System;

namespace Xsolla.Demo
{
	public interface IStoreActionProgress
	{
		Action OnStarted { get; set; }
		bool IsInProgress { get; }
	}
}
