using System;

namespace Xsolla
{
	[Serializable]
	public class XsollaError
	{
		public string http_status_code;
		public string message;
		public string request_id;
		public string extended_message;
	}
}
