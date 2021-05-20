using UnityEngine;

public class OnWebGLDisabler : MonoBehaviour
{
//#if UNITY_WEBGL
	private void Start()
	{
		this.gameObject.SetActive(false);
	}
//#endif
}
