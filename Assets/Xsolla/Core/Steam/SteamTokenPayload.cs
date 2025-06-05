#if XSOLLA_STEAMWORKS_PACKAGE_EXISTS
using System;

namespace Xsolla.Core
{
	[Serializable]
	internal class SteamTokenPayload
	{
		public string id;
		public bool is_cross_auth;
	}
}
#endif