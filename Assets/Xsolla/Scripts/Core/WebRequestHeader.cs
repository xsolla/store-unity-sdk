namespace Xsolla.Core
{
	public class WebRequestHeader
	{
		public string Name { get; set; }
		public string Value { get; set; }

		public static WebRequestHeader AuthHeader(string token)
		{
			return new WebRequestHeader {Name = "Authorization", Value = string.Format("Bearer {0}", token)};
		}
		public static WebRequestHeader ContentTypeHeader()
		{
			return new WebRequestHeader {Name = "Content-Type", Value = "application/json"};
		}
	}
}
