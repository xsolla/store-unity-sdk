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
    public class TestAPI_GetItemsInGroup : BaseTestApiScript
    {
        const int ITEMS_COUNT_IN_GROUP = 1;
        const string ITEMS_GROUP = "Permanent";

        protected override void Request()
        {
            StoreAPI.GetListOfItemsByGroup(XsollaSettings.StoreProjectId, ITEMS_GROUP, SuccessRequest, FailedRequest);
        }

        private void SuccessRequest(StoreItems items)
        {
            try
            {
                CheckItems(items);
            } catch(Exception e)
            {
                throw e;
            } finally {
                Complete();
            }
        }

        private void FailedRequest(Error error) 
        {
            Assert.Fail(error.errorMessage);
            Complete();
        }

        private void CheckItems(StoreItems items)
        {
            CheckCatalogSize(items);

            foreach (StoreItem item in items.items.ToList())
            {
                if (!CheckItem(item, out string message))
                {
                    Assert.Fail(message);
                }
            }
        }

        private void CheckCatalogSize(StoreItems items)
        {
            int catalogSize = items.items.Count();
            bool condition = catalogSize == ITEMS_COUNT_IN_GROUP;
            Assert.True(condition,
                "Catalog size must be = " + ITEMS_COUNT_IN_GROUP +
                " but we have = " + catalogSize
            );
        }

        private bool CheckItem(StoreItem item, out string message)
        {
            message = "no message";

            Assert.False(string.IsNullOrEmpty(item.sku), "Sku is null or empty");

            return true;
        }
    }
}
