using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Demo
{
	public class BattlePassItemsUIOnScrollChanger : BaseBattlePassUserStatSubscriber
    {
		[SerializeField] private BattlePassItemsInitialScroller InitialStateSetter = default;
		[SerializeField] private BattlePassItemsManager ItemsManager = default;
		[Space]
		[SerializeField] private GameObject LevelLabelUnfolded = default;
		[SerializeField] private GameObject LevelLabelFolded = default;
		[SerializeField] private GameObject LeftCurtain = default;
		[SerializeField] private ScrollRect ItemsScrollRect = default;

		private bool _isFirstCall = true;

		public ScrollRect.ScrollRectEvent OnScroll { get; private set; }

		public override void OnUserStatArrived(BattlePassUserStat userStat)
		{
			_isFirstCall = true;
			SetState(isInitial: true);
			StartCoroutine(AddListenerAfterInitialState());
		}

		private IEnumerator AddListenerAfterInitialState()
		{
			yield return new WaitWhile(() => !InitialStateSetter.IsInitialStateSet);
			yield return new WaitForEndOfFrame();

			ItemsScrollRect.onValueChanged.AddListener(_ => SetWorkingState());
		}

		private void SetWorkingState()
		{
			if (_isFirstCall)
				_isFirstCall = false;
			else
				return;

			SetState(isInitial: false);
		}

		private void SetState(bool isInitial)
		{
			LevelLabelFolded.SetActive(!isInitial);
			LevelLabelUnfolded.SetActive(isInitial);
			LeftCurtain.SetActive(!isInitial);

			ItemsManager.ShowCurrentLevelLabel(!isInitial);
		}
	}
}
