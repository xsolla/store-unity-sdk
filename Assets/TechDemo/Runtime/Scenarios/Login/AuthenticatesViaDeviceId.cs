using System;
using System.Collections;
using UnityEngine;
using Xsolla.Auth;
using Xsolla.Core;

namespace Xsolla.TechDemo.Scenarios
{
	public class AuthenticatesViaDeviceId : Scenario
	{
		public override IEnumerator Execute(Action onSuccess, Action<Error> onError)
		{
			if (Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer)
			{
				yield return null;
				onError.Invoke(new Error(errorMessage: "Device ID authentication is only available on mobile platforms."));
				yield break;
			}

			XsollaAuth.AuthViaDeviceID(onSuccess, onError);
		}
	}
}