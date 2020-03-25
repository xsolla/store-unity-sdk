using System;

namespace Xsolla.Login
{
	[Serializable]
	public class LinkAccountRequest
	{
		public string code;
		public string platform;
		public string project_id;
		public string publisher_project_id;
		public string user_id;
	}
}