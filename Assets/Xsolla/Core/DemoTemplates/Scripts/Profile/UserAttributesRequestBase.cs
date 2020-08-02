using UnityEngine;
using Xsolla.Core;
using Xsolla.Login;

public abstract class UserAttributesRequestBase : StoreActionResult
{
	protected string Token => XsollaLogin.Instance.Token;
	protected string ProjectID => XsollaSettings.StoreProjectId;

	protected bool IsRequestPossible
	{
		get
		{
			bool requestPossible = true;

			if (string.IsNullOrEmpty(Token))
			{
				Debug.LogError("UserAttributesRequestBase.IsRequestPossible: Token is null or empty, can not get or update user attributes. You must be logged in to request attributes");
				requestPossible = false;
			}

			if (string.IsNullOrEmpty(ProjectID))
			{
				Debug.LogError("UserAttributesRequestBase.IsRequestPossible: ProjectID is null or empty, can not get or update user attributes. You must specify Project ID in Xsolla settings");
				requestPossible = false;
			}

			return requestPossible;
		}
	}
}
