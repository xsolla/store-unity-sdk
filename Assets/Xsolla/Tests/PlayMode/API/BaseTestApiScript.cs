using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Xsolla.Core;
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

	virtual protected void FailedRequest(Error error)
	{
		Assert.Fail(error.errorMessage);
		Complete();
	}

	protected void Complete()
	{
		inProccess = false;
	}
}
