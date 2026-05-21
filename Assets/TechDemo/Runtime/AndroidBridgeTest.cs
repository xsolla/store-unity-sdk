using UnityEngine;

namespace Xsolla.TechDemo
{
	public class AndroidBridgeTest : MonoBehaviour
	{
		private void Start()
		{
			Debug.Log("START ANDROID_BRIDGE_TEST");
			var cls = new AndroidJavaClass("com.xsolla.android.bridge.XsollaBridge");
			cls.CallStatic("init");
			Debug.Log("END ANDROID_BRIDGE_TEST");
		}
	}
}