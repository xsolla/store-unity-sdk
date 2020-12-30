using System;
using UnityEngine;

namespace Xsolla.Demo
{
	/// <summary>
	/// This component MUST be attached to an object called 'SocialNetworks' because its method is called by using Unity.SendMessage
	/// </summary>
	public class AndroidSDKSocialAuthListener : MonoBehaviour
	{
		public event Action<string> OnSocialAuthResult;

		/// <summary>
		/// This method is called from within AndroidSDK by using Unity.SendMessage
		/// </summary>
		/// <param name="authResult"></param>
		public void ReceiveSocialAuthResult(string authResult)
		{
			Debug.Log("AndroidSDKListener.ReceiveSocialAuthResult: auth result arrived");
			OnSocialAuthResult?.Invoke(authResult);
		}
	}
}