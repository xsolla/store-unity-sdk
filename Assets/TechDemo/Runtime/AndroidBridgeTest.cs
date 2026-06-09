using UnityEngine;

namespace Xsolla.TechDemo
{
	public class AndroidBridgeTest : MonoBehaviour
	{
		private void Start()
		{
			Debug.Log("START ANDROID_BRIDGE_TEST");
			var bridgeFacadeClass = new AndroidJavaClass("com.xsolla.android.bridge.XsollaBridge");
			bridgeFacadeClass.CallStatic("init");

			var socialNetworkClass = new AndroidJavaClass("com.xsolla.android.bridge.SocialNetwork");
			var socialNetwork = socialNetworkClass.GetStatic<AndroidJavaObject>("DISCORD");
			bridgeFacadeClass.CallStatic("testSocialNetwork", socialNetwork);

			Debug.Log("END ANDROID_BRIDGE_TEST");
		}
	}
}