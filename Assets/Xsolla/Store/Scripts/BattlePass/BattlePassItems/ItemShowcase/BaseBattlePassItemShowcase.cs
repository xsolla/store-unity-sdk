using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Demo
{
	public abstract class BaseBattlePassItemShowcase : MonoBehaviour, IBattlePassSelectedItemSubscriber
	{
		[SerializeField] protected Image ItemImage = default;
		[SerializeField] protected Text ItemName = default;
		[SerializeField] protected Text ItemDescription = default;
		[SerializeField] protected Text ItemInfo = default;
		[Space]
		[SerializeField] private GameObject[] StateObjects = default;

		protected bool IsBattlePassExpired { get; private set; }

		public void OnItemSelected(ItemSelectedEventArgs eventArgs)
		{
			IsBattlePassExpired = eventArgs.IsBattlePassExpired;

			var selectedItem = eventArgs.SelectedItemInfo;
			StartCoroutine(SetItemImage(ItemImage, selectedItem.ItemImage));
			AdditionalImageActions(selectedItem.ItemImage);

			SetItemTextInfo(selectedItem.ItemDescription);
			SetState(selectedItem.ItemState);

			AdditionalActions(eventArgs);
		}

		protected IEnumerator SetItemImage(Image target, ItemImageContainer imageContainer)
		{
			target.gameObject.SetActive(false);
			yield return new WaitWhile(() => imageContainer.Image == null);
			target.sprite = imageContainer.Image;
			target.gameObject.SetActive(true);
		}

		protected abstract void AdditionalImageActions(ItemImageContainer imageContainer);
		protected abstract void SetItemTextInfo(BattlePassItemDescription itemDescription);

		private void SetState(BattlePassItemState selectedItemState)
		{
			foreach (var stateObject in StateObjects)
			{
				if (stateObject.activeSelf)
					stateObject.SetActive(false);
			}

			if (!IsBattlePassExpired)
				StateObjects[(int)selectedItemState].SetActive(true);
		}

		protected abstract void AdditionalActions(ItemSelectedEventArgs selectedEventArgs);
	}
}
