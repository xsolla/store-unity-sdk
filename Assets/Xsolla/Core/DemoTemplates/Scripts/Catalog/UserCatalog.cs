using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Xsolla.Core;

public class UserCatalog : MonoSingleton<UserCatalog>
{
	public event Action<List<CatalogVirtualItemModel>> UpdateItemsEvent;
	public event Action<List<CatalogVirtualCurrencyModel>> UpdateVirtualCurrenciesEvent;

	private IDemoImplementation _demoImplementation;

	public List<CatalogItemModel> AllItems { get; private set; }
	public List<CatalogVirtualItemModel> VirtualItems { get; private set; }
	public List<CatalogVirtualCurrencyModel> Currencies { get; private set; }

	public bool IsUpdated { get; private set; }

	public void Init(IDemoImplementation demoImplementation)
	{
		_demoImplementation = demoImplementation;
		AllItems = new List<CatalogItemModel>();
		VirtualItems = new List<CatalogVirtualItemModel>();
		Currencies = new List<CatalogVirtualCurrencyModel>();
	}

	public void UpdateItems([CanBeNull] Action onSuccess = null, [CanBeNull] Action<Error> onError = null)
	{
		StartCoroutine(UpdateItemsCoroutine(onSuccess, onError));
	}

	IEnumerator UpdateItemsCoroutine([CanBeNull] Action onSuccess = null, Action<Error> onError = null)
	{
		var isError = false;
		Action<Error> errorCallback = error =>
		{
			isError = true;
			onError?.Invoke(error);
		};
		if (_demoImplementation == null)
		{
			onSuccess?.Invoke();
			yield break;
		}

		yield return StartCoroutine(UpdateVirtualItemsCoroutine(errorCallback));
		yield return StartCoroutine(UpdateVirtualCurrenciesCoroutine(errorCallback));

		if (isError) yield break;
		IsUpdated = true;
		onSuccess?.Invoke();
	}

	private IEnumerator UpdateVirtualItemsCoroutine(Action<Error> onError = null)
	{
		var busy = true;
		_demoImplementation.GetCatalogVirtualItems(items =>
		{
			VirtualItems = items;
			AllItems.AddRange(items);
			UpdateItemsEvent?.Invoke(items);
			busy = false;
		}, onError);
		yield return new WaitWhile(() => busy);
	}

	private IEnumerator UpdateVirtualCurrenciesCoroutine(Action<Error> onError = null)
	{
		var busy = true;
		_demoImplementation.GetCatalogVirtualCurrencies(items =>
		{
			Currencies = items;
			AllItems.AddRange(items);
			UpdateVirtualCurrenciesEvent?.Invoke(items);
			busy = false;
		}, onError);
		yield return new WaitWhile(() => busy);
	}
}