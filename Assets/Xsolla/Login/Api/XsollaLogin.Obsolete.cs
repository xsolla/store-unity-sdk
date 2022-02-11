using System;
using Xsolla.Core;

namespace Xsolla.Login
{
	public partial class XsollaLogin
	{
		[Obsolete]
		public Token Token
		{
			get => Core.Token.Instance;
			set => Core.Token.Instance = value;
		}
	}
}
