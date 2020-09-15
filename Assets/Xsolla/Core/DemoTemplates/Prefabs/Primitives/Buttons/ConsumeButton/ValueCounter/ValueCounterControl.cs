using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;

public class ValueCounterControl : MonoBehaviour
{
	public SimpleTextButton increaseButton;
	public SimpleTextButton decreaseButton;
	public Text counterValueText;
	public float timeout = StoreConstants.DEFAULT_BUTTON_RATE_LIMIT_MS;

	private ValueCounter counter;

	void Start()
	{
		counter = gameObject.GetComponent<ValueCounter>();
		counter.ValueChanged += Counter_ValueChanged;

		counterValueText.text = counter.ToString();

		increaseButton.onClick = () => counter++;
		decreaseButton.onClick = () => counter--;

		increaseButton.RateLimitMs = timeout;
		decreaseButton.RateLimitMs = timeout;
	}

	private void Counter_ValueChanged(int value)
	{
		counterValueText.text = value.ToString();
	}
}