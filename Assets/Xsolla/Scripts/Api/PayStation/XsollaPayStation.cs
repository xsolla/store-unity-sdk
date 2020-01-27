using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Networking;
using Xsolla.Core;
using Xsolla.Paystation;

namespace Xsolla.PayStation
{
	public class XsollaPayStation : MonoSingleton<XsollaPayStation>
	{
		void OpenPurchaseUi(Token xsollaToken)
		{
			Application.OpenURL("https://secure.xsolla.com/paystation3/?access_token=" + xsollaToken.token);
		}

		public void OpenPayStation()
		{
			WebRequestHelper.Instance.PostRequest(
				"https://livedemo.xsolla.com/paystation/token_unreal.php", (string s) => OpenPurchaseUi(new Token() { token = s }), (Error error) => print("Error occured! Description: " + error)
			);
		}
	}
}
