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
			get => Core.Token.Instance;
			set => Core.Token.Instance = value;
		}
	}
}