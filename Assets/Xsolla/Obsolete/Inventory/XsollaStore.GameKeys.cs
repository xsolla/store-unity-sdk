using System;
using Xsolla.Core;
using Xsolla.GameKeys;

namespace Xsolla.Store
{
	public partial class XsollaStore : MonoSingleton<XsollaStore>
	{
		[Obsolete("Use XsollaGameKeys instead")]
		public void GetGamesList(string projectId, Action<GameItems> onSuccess, Action<Error> onError = null, int limit = 50, int offset = 0, string locale = null, string additionalFields = "long_description", string country = null)
			=> XsollaGameKeys.Instance.GetGamesList(projectId, onSuccess, onError, limit, offset, locale, additionalFields, country);

		[Obsolete("Use XsollaGameKeys instead")]
		public void GetGamesListBySpecifiedGroup(string projectId, string groupId, Action<GameItems> onSuccess, Action<Error> onError = null, int limit = 50, int offset = 0, string locale = null, string additionalFields = "long_description", string country = null)
			=> XsollaGameKeys.Instance.GetGamesListBySpecifiedGroup(projectId, groupId, onSuccess, onError, limit, offset, locale, additionalFields, country);

		[Obsolete("Use XsollaGameKeys instead")]
		public void GetGameForCatalog(string projectId, string itemSku, Action<GameItem> onSuccess, Action<Error> onError = null, string locale = null, string additionalFields = "long_description", string country = null)
			=> XsollaGameKeys.Instance.GetGameForCatalog(projectId, itemSku, onSuccess, onError, locale, additionalFields, country);

		[Obsolete("Use XsollaGameKeys instead")]
		public void GetGameKeyForCatalog(string projectId, string itemSku, Action<GameKeyItems> onSuccess, Action<Error> onError = null, string locale = null, string additionalFields = "long_description", string country = null)
			=> XsollaGameKeys.Instance.GetGameKeyForCatalog(projectId, itemSku, onSuccess, onError, locale, additionalFields, country);

		[Obsolete("Use XsollaGameKeys instead")]
		public void GetGameKeysListBySpecifiedGroup(string projectId, string groupId, Action<GameKeyItems> onSuccess, Action<Error> onError = null, int limit = 50, int offset = 0, string locale = null, string additionalFields = "long_description", string country = null)
			=> XsollaGameKeys.Instance.GetGameKeysListBySpecifiedGroup(projectId, groupId, onSuccess, onError, limit, offset, locale, additionalFields, country);

		[Obsolete("Use XsollaGameKeys instead")]
		public void GetOwnedGames(string projectId, bool sandbox, Action<GameOwnership> onSuccess, Action<Error> onError = null, int limit = 50, int offset = 0, string additionalFields = "long_description")
			=> XsollaGameKeys.Instance.GetOwnedGames(projectId, sandbox, onSuccess, onError, limit, offset, additionalFields);

		[Obsolete("Use XsollaGameKeys instead")]
		public void GetDrmList(string projectId, Action<DrmItems> onSuccess, Action<Error> onError = null)
			=> XsollaGameKeys.Instance.GetDrmList(projectId, onSuccess, onError);

		[Obsolete("Use XsollaGameKeys instead")]
		public void RedeemGameCode(string projectId, string gameCode, bool sandbox, Action onSuccess, Action<Error> onError = null)
			=> XsollaGameKeys.Instance.RedeemGameCode(projectId, gameCode, sandbox, onSuccess, onError);
	}
}
