using System.Collections;
using UnityEngine;
using Xsolla.Core.Popup;

namespace Xsolla.Demo
{
	public class BattlePassInitializationAwaiter : MonoBehaviour
    {
		[SerializeField] private BattlePassPopupFactory BattlePassPopupFactory;
		[SerializeField] private GameObject[] ObjectsToDisableOnExpired;
		[SerializeField] private float SecondsToAbortWaiting = 3.0f;

		private bool _isAwaitngRequired = true;
		private bool _isFirstCall = true;
		private bool? _isBattlePassExpired = null;

		private void Start()
		{
			PopupFactory.Instance.CreateWaiting().SetCloseCondition(() => !_isAwaitngRequired);
			StartCoroutine(AbortWaitingAfterSeconds(SecondsToAbortWaiting));
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

		private	IEnumerator AbortWaitingAfterSeconds(float secondsTillAbortion)
		{
			yield return new WaitForSeconds(secondsTillAbortion);
			_isAwaitngRequired = false;
		}
    }
}
