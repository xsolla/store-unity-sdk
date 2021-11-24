using System;
using Xsolla.Core;

namespace Xsolla.Login
{
	public partial class XsollaLogin
	{
		private Token _token;

		[Obsolete]
		public Token Token
		{
			get
			{
				return Core.Token.Instance;
			}
			set
			{
				Core.Token.Instance = value;
			}
		}
	}
}
