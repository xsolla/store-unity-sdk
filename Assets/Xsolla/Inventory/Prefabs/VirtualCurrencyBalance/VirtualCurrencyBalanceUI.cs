using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class VirtualCurrencyBalanceUI : MonoBehaviour
	{
		[SerializeField] public Image Image = default;
		[SerializeField] public Text Text = default;

		public void Initialize(ItemModel item)
		{
			if (Image != null)
			{
				if (!string.IsNullOrEmpty(item.ImageUrl))
				{
					ImageLoader.Instance.GetImageAsync(item.ImageUrl, (_, sprite) =>
					{
						if (Image/*still*/!= null)
							Image.sprite = sprite;
					});
				}
				else
				{
					Debug.LogError($"Item with sku = '{item.Sku}' without image!");
				}
			}
			else
			{
				Debug.LogWarning($"Your Virtual Currency with sku = `{item.Sku}` created without Image component!");
			}
		}

		public void SetBalance(uint balance)
		{
			if (Text)
				Text.text = balance.ToString();
		}
	}
}
