using System.Collections;
using UnityEngine;

namespace Xsolla.Demo
{
	public class BattlePassSubMenuSwitcher : MonoBehaviour
    {
		[SerializeField] private GameObject PremiumAvailable;
		[SerializeField] private GameObject PremiumPurchased;
		[SerializeField] private GameObject BattlePassEnded;
		[SerializeField] private GameObject BattlePassEndedNoItems;

		private GameObject[] _allStateObjects;
		private InitializationStep _initializationStep = InitializationStep.None;
		private bool _isBattlePassEnded;
		private bool _isPremiumPurchased;
		private bool _isItemsLeft;

		private InitializationStep Initialization
		{
			get
			{
				return _initializationStep;
			}
			set
			{
				if (value > _initializationStep)
					_initializationStep = value;
			}
		}

		private void Awake()
		{
			_allStateObjects = new GameObject[]
			{
				PremiumAvailable,
				PremiumPurchased,
				BattlePassEnded,
				BattlePassEndedNoItems
			};
		}

		public void OnDescriptionArrived(BattlePassDescription battlePassDescription)
		{
			_isBattlePassEnded = battlePassDescription.IsExpired;
			Initialization = InitializationStep.Description;
		}

		public void OnUserPremiumDefined(bool isPremiumUser)
		{
			StartCoroutine(SetUserPremiumAfterDescription(isPremiumUser));
		}

		private IEnumerator SetUserPremiumAfterDescription(bool isPremiumUser)
		{
			yield return new WaitWhile(() => Initialization < InitializationStep.Description);

			_isPremiumPurchased = isPremiumUser;
			Initialization = InitializationStep.Premium;

			if (Initialization == InitializationStep.Done)//User premium status will change on premium buy
				SwitchSubMenu();
		}

		public void OnItemSelectedArrived(ItemSelectedEventArgs eventArgs)
		{
			StartCoroutine(SetSelectedItemsAfterPremium(eventArgs));
		}

		private IEnumerator SetSelectedItemsAfterPremium(ItemSelectedEventArgs eventArgs)
		{
			yield return new WaitWhile(() => Initialization < InitializationStep.Premium);

			_isItemsLeft = eventArgs.AllItemsInCollectState.Length > 0;
			Initialization = InitializationStep.Done;
			SwitchSubMenu();
		}

		private void SwitchSubMenu()
		{
			DisableAll();

			if (_isBattlePassEnded)
			{
				if (_isItemsLeft)
					BattlePassEnded.SetActive(true);
				else
					BattlePassEndedNoItems.SetActive(true);
			}
			else
			{
				if (_isPremiumPurchased)
					PremiumPurchased.SetActive(true);
				else
					PremiumAvailable.SetActive(true);
			}
		}

		private void DisableAll()
		{
			foreach (var stateObject in _allStateObjects)
			{
				if (stateObject.activeSelf)
					stateObject.SetActive(false);
			}
		}

		private enum InitializationStep
		{
			None = 0,
			Description = 1,
			Premium = 2,
			Done = 3
		}
    }
}
