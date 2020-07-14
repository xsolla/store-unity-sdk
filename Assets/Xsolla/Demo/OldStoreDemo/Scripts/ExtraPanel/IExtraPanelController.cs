using System;
using System.Collections.Generic;
using Xsolla.Login;

public interface IExtraPanelController
{
    event Action LinkingAccountComplete;

	void Initialize();
	void SetAttributesVisibility(bool isVisible);
}
