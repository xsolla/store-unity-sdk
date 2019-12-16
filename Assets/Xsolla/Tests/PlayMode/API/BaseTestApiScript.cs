using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using Xsolla.Store;

public abstract class BaseTestApiScript
{
	protected XsollaStore StoreAPI;
	private bool inProccess;

	protected abstract void Request();

	protected BaseTestApiScript()
	{
		StoreAPI = XsollaStore.Instance;
	}

	[UnityTest]
	public IEnumerator RunRequest()
	{
		inProccess = true;
		Request();
		yield return new WaitWhile(() => inProccess);
	}

	protected void Complete()
	{
		inProccess = false;
	}
}
