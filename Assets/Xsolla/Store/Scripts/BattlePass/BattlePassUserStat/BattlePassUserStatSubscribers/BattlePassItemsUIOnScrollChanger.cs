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

		private bool _suspend = true;

		private void Awake()
		{
			ItemsScrollRect.onValueChanged.AddListener(SetWorkingState);
		}

		public override void OnUserStatArrived(BattlePassUserStat userStat)
		{
			_suspend = true;
			SetState(isInitial: true);
			StartCoroutine(UnsuspendOnInitialStateSet());
		}

		private IEnumerator UnsuspendOnInitialStateSet()
		{
			yield return new WaitWhile(() => !InitialStateSetter.IsInitialStateSet);
			yield return new WaitForEndOfFrame();

			_suspend = false;
		}

		private void SetWorkingState(Vector2 _)
		{
			if (_suspend)
				return;
			else
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
