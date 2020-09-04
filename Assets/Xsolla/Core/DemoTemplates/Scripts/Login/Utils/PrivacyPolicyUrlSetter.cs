using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrivacyPolicyUrlSetter : MonoBehaviour
{
#pragma warning disable 0649
	[SerializeField] private OpenUrlOnClick PrivacyPolicyOpenScript;
#pragma warning restore 0649

	private void Start()
	{
		var currentUrl = PrivacyPolicyOpenScript.URL;
		var currentLanguage = Application.systemLanguage;
		var urlAddition = default(string);

		switch (currentLanguage)
		{
			case SystemLanguage.German:
				urlAddition = "/de/";
				break;
			case SystemLanguage.Korean:
				urlAddition = "/ko/";
				break;
			case SystemLanguage.Chinese:
			case SystemLanguage.ChineseSimplified:
			case SystemLanguage.ChineseTraditional:
				urlAddition = "/zh/";
				break;
			case SystemLanguage.Japanese:
				urlAddition = "/ja/";
				break;
			case SystemLanguage.Russian:
				urlAddition = "/ru/";
				break;
			default:
				urlAddition = "/";
				break;
		}

		PrivacyPolicyOpenScript.URL = string.Format(currentUrl, urlAddition);
	}
}
