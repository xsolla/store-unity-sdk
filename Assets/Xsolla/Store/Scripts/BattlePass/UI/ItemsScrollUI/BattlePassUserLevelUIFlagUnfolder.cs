using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Demo
{
	public class BattlePassUserLevelUIFlagUnfolder : MonoBehaviour
    {
		[SerializeField] private ScrollRect ItemsScrollRect = default;
		[SerializeField] private BattlePassScrollPositionProvider TargetPositionProvider = default;
		[SerializeField] private float MaxDelta = default;
		[Space]
		[SerializeField] private BattlePassItemsManager ItemsManager = default;
		[Space]
		[SerializeField] private GameObject LevelLabelUnfolded = default;
		[SerializeField] private GameObject LevelLabelFolded = default;
		[SerializeField] private GameObject LeftCurtain = default;

		private float _minTargetX;
		private float _maxTargetX;

		private void Awake()
		{
			TargetPositionProvider.ScrollPositionDefined += SetNewTargetPosition;
			ItemsScrollRect.onValueChanged.AddListener(CheckIfTargetPosition);
		}

		private void SetNewTargetPosition(int _, Vector2 targetScroll)
		{
			var targetX = Mathf.Abs(targetScroll.x);

			_minTargetX = targetX - MaxDelta;
			_maxTargetX = targetX + MaxDelta;

			CheckIfTargetPosition();
		}

		private void CheckIfTargetPosition(Vector2 _)
		{
			CheckIfTargetPosition();
		}

		private void CheckIfTargetPosition()
		{
			var currentX = Mathf.Abs(ItemsScrollRect.content.anchoredPosition.x);
			var isTargetPos = currentX >= _minTargetX && currentX <= _maxTargetX;

			SetState(isTargetPos);
		}

		private void SetState(bool isTarget)
		{
			LevelLabelFolded.SetActive(!isTarget);
			LevelLabelUnfolded.SetActive(isTarget);
			LeftCurtain.SetActive(!isTarget);

			ItemsManager.ShowCurrentLevelLabel(!isTarget);
		}
	}
}
