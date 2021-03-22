using System.Collections;
using UnityEngine;
using Xsolla.Core.Popup;

namespace Xsolla.Demo
{
	public class BattlePassInitializationAwaiter : MonoBehaviour
    {
		[SerializeField] private BattlePassPopupFactory BattlePassPopupFactory = default;
		[SerializeField] private GameObject[] ObjectsToDisableOnExpired = default;

		private bool _isAwaitngRequired = true;
		private bool _isFirstCall = true;
		private bool? _isBattlePassExpired = null;

		private void Start()
		{
			PopupFactory.Instance.CreateWaiting().SetCloseCondition(() => !_isAwaitngRequired);
		}

		public void OnBattlePassDescriptionArrived(BattlePassDescription battlePassDescription)
		{
			_isBattlePassExpired = battlePassDescription.IsExpired;
		}

		//This method will be called very last on page initialization
		public void OnUserPremiumStatusDefined()
		{
			if (_isFirstCall)
			{
				_isFirstCall = false;
				StartCoroutine(FinalizeWaiting());
			}
		}

		private IEnumerator FinalizeWaiting()
		{
			yield return new WaitWhile(() => _isBattlePassExpired == null);

			_isAwaitngRequired = false;

			if (_isBattlePassExpired == true)
			{
				foreach (var objectToDisable in ObjectsToDisableOnExpired)
					objectToDisable.SetActive(false);

				BattlePassPopupFactory.CreateBattlePassExpiredPopup();
			}
		}
    }
}
