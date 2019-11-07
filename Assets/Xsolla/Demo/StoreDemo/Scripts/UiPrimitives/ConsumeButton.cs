using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumeButton : MonoBehaviour
{
	public SimpleTextButton consumeButton;
	public Action onClick { get; set; }
	public ValueCounter counter;

	void Start()
    {
		consumeButton.onClick = () => {
			if(counter > 0) {
				if(counter > 1) {
					Debug.LogWarning(
						"Sorry, but Xsolla_API can consume only one item at time, but later it will be fixed."
					);
					while (counter > 1)
						counter--;
				}
				this.onClick?.Invoke();
			} else {
				Debug.Log("You try consume item with quantity = 0!");
			}
		};
	}
}
