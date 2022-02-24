using System;

namespace Xsolla.Cart
{
	[Serializable]
	public class RemovePromocodeFromCartRequest
	{
		public string id;

		public RemovePromocodeFromCartRequest(string id)
		{
			this.id = id;
		}
	}
}
