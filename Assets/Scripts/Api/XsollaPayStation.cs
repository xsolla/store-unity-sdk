using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xsolla;

namespace Xsolla
{
	public class XsollaPayStation : MonoSingleton<XsollaPayStation>
	{
		[SerializeField]
		string serverUrl;
		
		void OpenPurchaseUi(Token xsollaToken)
		{
			Application.OpenURL("https://secure.xsolla.com/paystation3/?access_token=" + xsollaToken.token);
		}

		public void OpenPayStation()
		{
			Debug.Log("OpenPayStation");
			
			// TODO
		}
	}
}
