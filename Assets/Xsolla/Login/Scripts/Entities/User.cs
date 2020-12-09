using System;

namespace Xsolla.Login
{
	[Serializable]
	public class User
	{
		public string exp;
		public string iss;
		public string iat;
		public string username;
		public string xsolla_login_access_key;
		public string sub;
		public string email;
		public string xsolla_login_project_id;
		public string publisher_id;
		public string provider;
		public string name;
		public bool is_linked;
	}
}