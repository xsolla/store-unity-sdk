using System;

namespace Xsolla.Store
{
	[Serializable]
	public class RemovePromocodeFromCardRequest
	{
		public string id;

		public RemovePromocodeFromCardRequest(string id)
		{
			this.id = id;
		}
	}
}