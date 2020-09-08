using System;
using UnityEngine;
using Xsolla.Core;

public abstract class StoreActionResult : MonoBehaviour, IStoreActionResult
{
	public Action OnSuccess { get; set; }
	public Action<Error> OnError { get; set; }
}
