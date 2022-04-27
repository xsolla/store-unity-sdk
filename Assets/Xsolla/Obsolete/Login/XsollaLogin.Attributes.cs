using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Xsolla.Core;
using Xsolla.UserAccount;

namespace Xsolla.Login
{
	public partial class XsollaLogin : MonoSingleton<XsollaLogin>
	{
		[Obsolete("Use XsollaUserAccount instead")]
		public void GetUserAttributes(string token, string publisherProjectId, UserAttributeType attributeType, [CanBeNull] List<string> keys, [CanBeNull] string userId, [NotNull] Action<List<UserAttribute>> onSuccess, [CanBeNull] Action<Error> onError)
			=> XsollaUserAccount.Instance.GetUserAttributes(token, publisherProjectId, attributeType, keys, userId, onSuccess, onError);

		[Obsolete("Use XsollaUserAccount instead")]
		public void UpdateUserAttributes(string token, string publisherProjectId, List<UserAttribute> attributes, Action onSuccess, Action<Error> onError)
			=> XsollaUserAccount.Instance.UpdateUserAttributes(token, publisherProjectId, attributes, onSuccess, onError);

		[Obsolete("Use XsollaUserAccount instead")]
		public void RemoveUserAttributes(string token, string publisherProjectId, List<string> removingKeys, Action onSuccess, Action<Error> onError)
			=> XsollaUserAccount.Instance.RemoveUserAttributes(token, publisherProjectId, removingKeys, onSuccess, onError);
	}
}
