using System;
using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public abstract class UserAttributesRequestBase : MonoBehaviour
	{
		public Action OnSuccess { get; set; }
		public Action<Error> OnError { get; set; }

		protected string Token
		{
			get
			{
				return Xsolla.Core.Token.Instance;
			}
		}
		protected string ProjectID
		{
			get
			{
				return XsollaSettings.StoreProjectId;
			}
		}

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
}
