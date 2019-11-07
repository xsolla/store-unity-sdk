using UnityEngine;
using UnityEngine.UI;

public class ValueCounterControl : MonoBehaviour
{
	public SimpleTextButton increaseButton;
	public SimpleTextButton decreaseButton;
	public Text counterValueText;

	private ValueCounter counter;

	void Start()
    {
		counter = gameObject.GetComponent<ValueCounter>();
		counter.ValueChanged += Counter_ValueChanged;

		counterValueText.text = counter.ToString();

		increaseButton.onClick = () => counter++;
		decreaseButton.onClick = () => counter--;
	}

	private void Counter_ValueChanged(int value)
	{
		counterValueText.text = value.ToString();
	}
}
