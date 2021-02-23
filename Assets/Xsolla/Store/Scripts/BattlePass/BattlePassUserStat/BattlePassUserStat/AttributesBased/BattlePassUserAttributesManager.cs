using System;
using System.Collections.Generic;
using UnityEngine;
using Xsolla.Login;

namespace Xsolla.Demo
{
	public class BattlePassUserAttributesManager : BaseAttributeManager
	{
		[SerializeField] private UserAttributesProvider AttributesProvider = default;

		private const string BATTLEPASS_PREFIX = "BattlePass";

		private string _battlePassName;

		public event Action<List<UserAttribute>> UserAttributesArrived;

		public void GetBattlePassUserAttributes(string battlePassName)
		{
			_battlePassName = battlePassName;

			AttributesProvider.OnError += error => Debug.LogError($"Attributes load failed with error: '{error}'");
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
				Debug.Log($"There are {previousBattlePassesAttributes.Count} attributes of non-current BattlePass. They will be deleted.");
				DeleteAttributes(previousBattlePassesAttributes);
			}

			Debug.Log($"There were {currentBattlePassAttributes.Count} BatlePass attributes loaded.");
			UserAttributesArrived?.Invoke(currentBattlePassAttributes);
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
