using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PreloaderScript : MonoBehaviour
{
    private Text text;
    // Start is called before the first frame update
    void Start()
    {
        text = transform.Find("Text").GetComponent<Text>();
    }

    public void SetPercent(uint value)
    {
		if(text != null) {
            text.text = "Update" + Environment.NewLine + value.ToString() + "%";
		}
	}
}
