using System;
using UnityEngine;

public class LoginCreateToAuthProxyRequestHolder : MonoBehaviour
{
	public Action<LoginPageEnterController, object> ProxyRequest { get; set; }
	public object ProxyArgument { get; set; }
}
