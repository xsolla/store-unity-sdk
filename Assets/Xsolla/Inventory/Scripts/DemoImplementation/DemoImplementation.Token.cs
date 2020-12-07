using UnityEngine;
using Xsolla.Core;
using Xsolla.Store;

namespace Xsolla.Demo
{
	public partial class DemoImplementation : MonoBehaviour, IInventoryDemoImplementation
	{
		partial void UpdateStoreToken(Token token)
		{
			XsollaStore.Instance.Token = token;
		}
	}
}
