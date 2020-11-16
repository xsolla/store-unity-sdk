using System;
using UnityEngine;

public class LoginProxyActionHolder : MonoBehaviour
{
	public Action<LoginPageEnterController, object> ProxyAction { get; set; }
	public object ProxyActionArgument { get; set; }
}
