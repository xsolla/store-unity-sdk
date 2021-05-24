using System;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.UIBuilder;

namespace Xsolla.Demo
{
	public partial class BattlePassItem : MonoBehaviour
    {
		[SerializeField] private Image BackgroundImage = default;
		[SerializeField] private ColorProvider Background = new ColorProvider();
		[SerializeField] private ColorProvider BackgroundEmpty = new ColorProvider();
		[SerializeField] private ColorProvider BackgroundCurrentLevel = new ColorProvider();
		[SerializeField] private ColorProvider BackgroundSelected = new ColorProvider();

		[Space]
		[SerializeField] private GameObject HighLight = default;
		[SerializeField] private GameObject ItemFrame = default;
		[Space]
		[SerializeField] private GameObject[] StateObjects = default;
		[Space]
		[SerializeField] private SimpleButton ItemButton = default;

		private ItemImageContainer _itemImageContainer = new ItemImageContainer();
		private Color? _preSelectedBackground;

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
			var targetColor = Color.white;
			if (isCurrent)
			{
				targetColor = BackgroundCurrentLevel.GetValue();
			}
			else
			{
				if (ItemDescription != null)
					targetColor = Background.GetValue();
				else
					targetColor = BackgroundEmpty.GetValue();
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
				_preSelectedBackground = BackgroundImage.color;
				BackgroundImage.color = BackgroundSelected.GetValue();
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
