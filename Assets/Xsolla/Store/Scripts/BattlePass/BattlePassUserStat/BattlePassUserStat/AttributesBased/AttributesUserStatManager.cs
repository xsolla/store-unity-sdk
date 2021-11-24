using System.Collections.Generic;
using UnityEngine;
using Xsolla.Login;

namespace Xsolla.Demo
{
	public partial class AttributesUserStatManager : BaseBattlePassUserStatManager
	{
		[SerializeField] private BattlePassUserAttributesManager AttributesManager;

		private const string ATTRIBUTE_KEY_TEMPLATE = "BattlePass{0}{1}";

		private const string OBTAINED_VALUE_TEMPLATE = "FREE:{0} PREM:{1}";
		private const char OBTAINED_VALUE_SEPARATOR = '#';

		private const int DEFAULT_LEVEL = 1;
		private const int DEFAULT_EXP = 0;

		private const string DEFAULT_ATTRIBUTE_PERMISSION = "public";

		private BattlePassDescription _battlePassDescription;
		private UserAttribute _levelAttribute;
		private UserAttribute _expAttribute;
		private UserAttribute _obtainedAttribute;
		private List<UserAttribute> _attributesToUpdate = new List<UserAttribute>();

		private BattlePassUserStat _currentUserStat;

		private void Awake()
		{
			AttributesManager.UserAttributesArrived += OnUserAttributesArrived;
		}

		public override void GetUserStat(BattlePassDescription battlePassDescription)
		{
			_battlePassDescription = battlePassDescription;
			AttributesManager.GetBattlePassUserAttributes(battlePassDescription.Name);
		}

		private void OnUserAttributesArrived(List<UserAttribute> userAttributes)
		{
			_levelAttribute = GetAttributeOfType(userAttributes, AttributeType.Level);
			_expAttribute = GetAttributeOfType(userAttributes, AttributeType.Exp);
			_obtainedAttribute = GetAttributeOfType(userAttributes, AttributeType.Obtained);
			UpdateAttributesIfAny();

			IssueNewUserStat();
		}

		private UserAttribute GetAttributeOfType(IEnumerable<UserAttribute> attributes, AttributeType attributeType)
		{
			var result = FindExisting(attributes, attributeType);

			if (result == null)
			{
				Debug.Log(string.Format("Could not find BattlePass attribute of type: '{0}'. Creating a new one.", attributeType.ToString()));
				result = CreateNew(_battlePassDescription.Name, attributeType);
				_attributesToUpdate.Add(result);
			}

			return result;
		}

		private UserAttribute FindExisting(IEnumerable<UserAttribute> attributes, AttributeType attributeType)
		{
			foreach (var attribute in attributes)
			{
				if (attribute.key.EndsWith(attributeType.ToString()))
					return attribute;
			}
			//else
			return null;
		}

		private UserAttribute CreateNew(string battlePassName, AttributeType attributeType)
		{
			var key = string.Format(ATTRIBUTE_KEY_TEMPLATE, battlePassName, attributeType.ToString());
			var permission = DEFAULT_ATTRIBUTE_PERMISSION;
			var value = default(string);

			switch (attributeType)
			{
				case AttributeType.Level:
					value = DEFAULT_LEVEL.ToString();
					break;
				case AttributeType.Exp:
					value = DEFAULT_EXP.ToString();
					break;
				case AttributeType.Obtained:
					value = string.Format(OBTAINED_VALUE_TEMPLATE, string.Empty, string.Empty);
					break;
				default:
					Debug.LogError(string.Format("Unexpected attribute type: '{0}'", attributeType.ToString()));
					break;
			}

			var result = new UserAttribute();

			result.key = key;
			result.permission = permission;
			result.value = value;

			return result;
		}

		private void UpdateAttributesIfAny()
		{
			if (_attributesToUpdate.Count > 0)
			{
				AttributesManager.UpdateUserAttributes(_attributesToUpdate);
				_attributesToUpdate = new List<UserAttribute>();
			}
		}

		private void IssueNewUserStat()
		{
			var userStat = GenerateBattlePassUserStat(_levelAttribute, _expAttribute, _obtainedAttribute);
			UpdateAttributesIfAny();

			_currentUserStat = userStat;
			base.RaiseUserStatArrived(userStat);
		}

		private enum AttributeType
		{
			Level, Exp, Obtained
		}
	}
}
