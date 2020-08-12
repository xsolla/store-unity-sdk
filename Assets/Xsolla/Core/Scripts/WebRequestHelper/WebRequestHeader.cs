namespace Xsolla.Core
{
	public class WebRequestHeader
	{
		public string Name { get; set; }
		public string Value { get; set; }

		public WebRequestHeader()
		{}
		
		public WebRequestHeader(string name, string value)
		{
			Name = name;
			Value = value;
		}

		public static WebRequestHeader AuthHeader(string bearerTokenValue)
		{
			return new WebRequestHeader {Name = "Authorization", Value = string.Format("Bearer {0}", bearerTokenValue)};
		}
		
		public static WebRequestHeader AuthBasic(string basicTokenValue)
		{
			return new WebRequestHeader {Name = "Authorization", Value = string.Format("Basic {0}", basicTokenValue)};
		}
		
		public static WebRequestHeader ContentTypeHeader()
		{
			return new WebRequestHeader {Name = "Content-Type", Value = "application/json"};
		}
		
		public static WebRequestHeader AcceptHeader()
		{
			return new WebRequestHeader {Name = "Accept", Value = "application/json"};
		}

		public static WebRequestHeader SteamPaymentHeader(string steamUserId)
		{
			return new WebRequestHeader { Name = "x-steam-userid", Value = steamUserId };
		}
	}
}
