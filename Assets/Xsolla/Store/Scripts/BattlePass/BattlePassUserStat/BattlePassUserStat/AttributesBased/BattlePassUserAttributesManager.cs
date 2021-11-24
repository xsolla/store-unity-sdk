using System;
using System.Collections.Generic;
using UnityEngine;
using Xsolla.Login;

namespace Xsolla.Demo
{
	public class BattlePassUserAttributesManager : BaseAttributeManager
	{
		[SerializeField] private UserAttributesProvider AttributesProvider;

		private const string BATTLEPASS_PREFIX = "BattlePass";

		private string _battlePassName;

		public event Action<List<UserAttribute>> UserAttributesArrived;

		public void GetBattlePassUserAttributes(string battlePassName)
		{
			_battlePassName = battlePassName;

			AttributesProvider.OnError += error => Debug.LogError(string.Format("Attributes load failed with error: '{0}'", error));
			AttributesProvider.ProvideUserAttributes();
		}

		public override void Initialize(List<UserAttribute> userReadOnlyAttributes = null, List<UserAttribute> userCustomAttributes = null)
		{
			if (userReadOnlyAttributes != null)
				Debug.LogError("ReadOnly attributes are not supported");

			if (userCustomAttributes == null || userCustomAttributes.Count == 0)
			{
				Debug.Log("User attributes were null or empty on load.");
				userCustomAttributes = new List<UserAttribute>();
			}

			var previousBattlePassesAttributes = new List<UserAttribute>();
			var currentBattlePassAttributes = new List<UserAttribute>();

			foreach (var attribute in userCustomAttributes)
			{
				if (attribute.key.StartsWith(BATTLEPASS_PREFIX))
				{
					if (attribute.key.Contains(_battlePassName))
						currentBattlePassAttributes.Add(attribute);
					else
						previousBattlePassesAttributes.Add(attribute);
				}
			}

			if (previousBattlePassesAttributes.Count > 0)
			{
				Debug.Log(string.Format("There are {0} attributes of non-current BattlePass. They will be deleted.", previousBattlePassesAttributes.Count));
				DeleteAttributes(previousBattlePassesAttributes);
			}

			Debug.Log(string.Format("There were {0} BatlePass attributes loaded.", currentBattlePassAttributes.Count));
			if (UserAttributesArrived != null)
				UserAttributesArrived.Invoke(currentBattlePassAttributes);
		}

		public void UpdateUserAttributes(List<UserAttribute> attributesToUpdate)
		{
			base.RaiseOnUpdateUserAttributes(attributesToUpdate);
		}

		private void DeleteAttributes(List<UserAttribute> attributesToDelete)
		{
			var keysToDelete = new List<string>(attributesToDelete.Count);

			foreach (var attribute in attributesToDelete)
				keysToDelete.Add(attribute.key);

			base.RaiseOnRemoveUserAttributes(keysToDelete);
		}
	}
}
