using System;
using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Demo
{
	public partial class BattlePassItem : MonoBehaviour
    {
		[SerializeField] private Image BackgroundImage = default;
		[SerializeField] private Sprite Background = default;
		[SerializeField] private Sprite BackgroundEmpty = default;
		[SerializeField] private Sprite BackgroundCurrentLevel = default;
		[SerializeField] private Sprite BackgroundSelected = default;
		[Space]
		[SerializeField] private GameObject HighLight = default;
		[Space]
		[SerializeField] private GameObject[] StateObjects = default;
		[Space]
		[SerializeField] private SimpleButton ItemButton = default;

		private ItemImageContainer _itemImageContainer = new ItemImageContainer();
		private Sprite _preSelectedBackground;

		public static BattlePassItem SelectedItem { get; private set; }

		public BattlePassItemDescription ItemDescription { get; private set; }
		public BattlePassItemState ItemState { get; private set; } = BattlePassItemState.Empty;

		public static event Action<BattlePassItemClickEventArgs> ItemClick;

		private void Awake()
		{
			ItemButton.onClick = OnButtonClick;
		}

		private void OnDestroy()
		{
			if (SelectedItem)
				SelectedItem = null;

			if (_allCatalogItems != null)
				_allCatalogItems = null;
		}

		public void SetCurrent(bool isCurrent)
		{
			var targetBackground = default(Sprite);

			if (isCurrent)
				targetBackground = BackgroundCurrentLevel;
			else
			{
				if (ItemDescription != null)
					targetBackground = Background;
				else
					targetBackground = BackgroundEmpty;
			}

			if (_preSelectedBackground == null)
				BackgroundImage.sprite = targetBackground;
			else
				_preSelectedBackground = targetBackground;
		}

		public void SetState(BattlePassItemState itemState)
		{
			if (ItemDescription == null)
				return;

			foreach (var stateObject in StateObjects)
			{
				if (stateObject.activeSelf)
					stateObject.SetActive(false);
			}

			ItemState = itemState;
			StateObjects[(int)itemState].SetActive(true);
		}

		public void ForceItemClick() => RaiseItemClick();

		private void OnButtonClick()
		{
			if (SelectedItem)
				SelectedItem.SetSelected(false);

			SetSelected(true);
			SelectedItem = this;
			RaiseItemClick();
		}

		private void RaiseItemClick()
		{
			var clickEventArgs = new BattlePassItemClickEventArgs(ItemDescription, ItemState, _itemImageContainer);
			ItemClick?.Invoke(clickEventArgs);
		}

		private void SetSelected(bool isSelected)
		{
			if (isSelected)
			{
				_preSelectedBackground = BackgroundImage.sprite;
				BackgroundImage.sprite = BackgroundSelected;
			}
			else
			{
				BackgroundImage.sprite = _preSelectedBackground;
				_preSelectedBackground = null;
			}
		}
	}
}
