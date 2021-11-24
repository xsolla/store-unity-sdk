using UnityEngine;

namespace Xsolla.Demo
{
	public class UserAttributesProviderInvoker : MonoBehaviour
    {
		[SerializeField] UserAttributesProvider UserAttributesProvider;

		private void Start()
		{
			UserAttributesProvider.ProvideUserAttributes();
		}
	}
}
