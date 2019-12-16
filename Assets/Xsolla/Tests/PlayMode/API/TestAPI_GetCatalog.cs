using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Xsolla.Core;
using Xsolla.Store;

namespace Tests
{
    public class TestAPI_GetCatalog : BaseTestApiScript
    {
		protected override void Request()
		{
			StoreAPI.GetListOfItems(XsollaSettings.StoreProjectId, SuccessRequest, FailedRequest);
		}

		private void SuccessRequest(StoreItems items)
		{
			CheckItems(items);
			Complete();
		}

		private void FailedRequest(Error error)
		{
			Assert.Fail(error.errorMessage);
			Complete();
		}

		private void CheckItems(StoreItems items)
		{
			foreach(StoreItem item in items.items.ToList()) {
				if (!CheckItem(item, out string message)) {
					Assert.Fail(message);
				}
			}
		}

		private bool CheckItem(StoreItem item, out string message)
		{
			message = "no message";

			Assert.False(string.IsNullOrEmpty(item.sku), "Sku is null or empty");

			return true;
		}
	}
}
