using System;
using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Demo
{
	public partial class BattlePassItem : MonoBehaviour
    {
		[SerializeField] private Image BackgroundImage;
		[SerializeField] private Color Background;
		[SerializeField] private Color BackgroundEmpty;
		[SerializeField] private Color BackgroundCurrentLevel;
		[SerializeField] private Color BackgroundSelected;

		[Space]
		[SerializeField] private GameObject HighLight;
		[SerializeField] private GameObject ItemFrame;
		[Space]
		[SerializeField] private GameObject[] StateObjects;
		[Space]
		[SerializeField] private SimpleButton ItemButton;

		private ItemImageContainer _itemImageContainer = new ItemImageContainer();
		private Color? _preSelectedBackground;

		public static BattlePassItem SelectedItem { get; private set; }

		public BattlePassItemDescription ItemDescription { get; private set; }
		public BattlePassItemState ItemState { get; private set; }

		public static event Action<BattlePassItemClickEventArgs> ItemClick;

		private void Awake()
		{
			ItemState = BattlePassItemState.Empty;
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
			var targetColor = Color.white;
			if (isCurrent)
			{
				targetColor = BackgroundCurrentLevel;
			}
			else
			{
				if (ItemDescription != null)
					targetColor = Background;
				else
					targetColor = BackgroundEmpty;
			}

			if (_preSelectedBackground == null)
				BackgroundImage.color = targetColor;
			else
				_preSelectedBackground = targetColor;
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

			StateObjects[(int)itemState].SetActive(true);
			SetItemFrame(itemState);
			ItemState = itemState;
		}

		public void ForceItemClick()
		{
			RaiseItemClick();
		}

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
			if (ItemClick != null)
				ItemClick.Invoke(clickEventArgs);
		}

		private void SetSelected(bool isSelected)
		{
			if (isSelected)
			{
				_preSelectedBackground = BackgroundImage.color;
				BackgroundImage.color = BackgroundSelected;
			}
			else
			{
				BackgroundImage.color = _preSelectedBackground ?? Color.white;
				_preSelectedBackground = null;
			}
		}

		private void SetItemFrame(BattlePassItemState itemState)
		{
			switch (itemState)
			{
				case BattlePassItemState.Collect:
				case BattlePassItemState.Collected:
					ItemFrame.SetActive(true);
					break;
				case BattlePassItemState.FutureLocked:
					ItemFrame.SetActive(false);
					break;
				case BattlePassItemState.PremiumLocked:
				case BattlePassItemState.Empty:
				default:
					//Do nothing
					break;
			}
		}
	}
}
