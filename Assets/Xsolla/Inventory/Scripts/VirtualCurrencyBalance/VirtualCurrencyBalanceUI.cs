using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class VirtualCurrencyBalanceUI : MonoBehaviour
	{
		[SerializeField] public Image Image;
		[SerializeField] public Text Text;

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
					Debug.LogError(string.Format("Item with sku = '{0}' without image!", item.Sku));
				}
			}
			else
			{
				Debug.LogWarning(string.Format("Your Virtual Currency with sku = `{0}` created without Image component!", item.Sku));
			}
		}

		public void SetBalance(uint balance)
		{
			if (Text)
				Text.text = balance.ToString();
		}
	}
}
