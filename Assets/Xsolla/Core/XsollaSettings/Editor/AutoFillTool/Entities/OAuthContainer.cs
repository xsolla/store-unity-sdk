using System;

namespace Xsolla.Core.AutoFillSettings
{
	[Serializable]
	public class OAuthContainer
	{
		public int id;
		public bool is_public;
		public string name;
		public string[] redirect_uris;
	}
}