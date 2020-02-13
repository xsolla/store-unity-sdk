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
                ItemsCheck.ValidateItems(items, ITEMS_COUNT_IN_GROUP);
            } catch(Exception e)
            {
                throw e;
            } finally {
                Complete();
            }
        }
	}
}
