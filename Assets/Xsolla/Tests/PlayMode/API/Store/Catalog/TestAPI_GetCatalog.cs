using System;
using NUnit.Framework;
using Xsolla.Core;
using Xsolla.Store;

namespace Tests
{
    public class TestAPI_GetCatalog : BaseTestApiScript
    {
        const int CATALOG_SIZE = 1;

        protected override void Request()
        {
            StoreAPI.GetListOfItems(XsollaSettings.StoreProjectId, SuccessRequest, FailedRequest);
        }

        private void SuccessRequest(StoreItems items)
        {
            try
            {
                ItemsCheck.ValidateItems(items, CATALOG_SIZE);
            } catch(Exception e)
            {
                throw e;
            } finally {
                Complete();
            }
        }
    }
}
