using System;
using System.Collections.Generic;
using Xsolla.Core;
using Xsolla.Store;

public partial class DemoImplementation : MonoSingleton<DemoImplementation>, IDemoImplementation
{
	private Action<Error> WrapErrorCallback(Action<Error> onError)
	{
		return error =>
		{
			StoreDemoPopup.ShowError(error);
			onError?.Invoke(error);
		};
	}
}
