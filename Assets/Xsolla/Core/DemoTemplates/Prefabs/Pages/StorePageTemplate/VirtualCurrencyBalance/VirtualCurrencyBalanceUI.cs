using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;

public class VirtualCurrencyBalanceUI : MonoBehaviour
{
	[SerializeField] public Image Image;
	[SerializeField] public Text Text;

	public void Initialize(ItemModel item)
	{
		if (Image != null)
		{
			ImageLoader.Instance.GetImageAsync(item.ImageUrl, (_, sprite) => Image.sprite = sprite);
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