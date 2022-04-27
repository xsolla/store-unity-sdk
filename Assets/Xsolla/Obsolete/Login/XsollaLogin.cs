using System;
using Xsolla.Core;
using JetBrains.Annotations;
using Xsolla.Auth;

namespace Xsolla.Login
{
	[PublicAPI]
	public partial class XsollaLogin : MonoSingleton<XsollaLogin>
	{
		[Obsolete("Use XsollaAuth instead")]
		public void DeleteToken(string key) => XsollaAuth.Instance.DeleteToken(key);

		[Obsolete("Use XsollaAuth instead")]
		public void SaveToken(string key, string token) => XsollaAuth.Instance.SaveToken(key,token);
	}
}
