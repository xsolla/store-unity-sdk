using UnityEngine;
using Xsolla.Store;

namespace Xsolla.Demo
{
	public partial class DemoImplementation : MonoBehaviour, IInventoryDemoImplementation
	{
		partial void UpdateStoreToken()
		{
			XsollaStore.Instance.Token = GetUserToken();
		}
	}
}
