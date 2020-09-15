using System;
using UnityEngine;
using Xsolla.Core;

public abstract class StoreStringActionResult : MonoBehaviour, IStoreStringAction
{
	public Action<string> OnSuccess { get; set; }
	public Action<Error> OnError { get; set; }
}
