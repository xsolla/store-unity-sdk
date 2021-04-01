using UnityEngine;

namespace Xsolla.Demo
{
	public class UserAttributesProviderInvoker : MonoBehaviour
    {
		[SerializeField] UserAttributesProvider UserAttributesProvider = default;

		private void Start()
		{
			UserAttributesProvider.ProvideUserAttributes();
		}
	}
}
