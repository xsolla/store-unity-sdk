using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;
using Xsolla.Store;

public class VirtualCurrencyBalanceUI : MonoBehaviour
{
	[SerializeField]
	public Image Image;
	[SerializeField]
	public Text Text;

	public void Initialize(StoreItem item)
	{
		if(Image != null) {
			ImageLoader.Instance.GetImageAsync(item.image_url, (_, sprite) => Image.sprite = sprite);
		} else {
			Debug.LogWarning($"Your Virtual Currency with sku = `{item.sku}` created without Image component!");
		}
	}

	public void SetBalance(uint balance)
	{
		if (Text)
			Text.text = balance.ToString();
	}
}
