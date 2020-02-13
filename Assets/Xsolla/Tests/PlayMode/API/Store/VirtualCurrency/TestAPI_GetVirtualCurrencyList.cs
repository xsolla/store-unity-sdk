using System;
using NUnit.Framework;
using Xsolla.Core;
using Xsolla.Store;

namespace Tests
{
    public class TestAPI_GetVirtualCurrencyList : BaseTestApiScript
    {
        const int VIRTUAL_CURRENCIES_COUNT = 2;

        protected override void Request()
        {
            StoreAPI.GetVirtualCurrencyList(XsollaSettings.StoreProjectId, SuccessRequest, FailedRequest);
        }

        private void SuccessRequest(VirtualCurrencyItems items)
        {
            try
            {
                ItemsCheck.ValidateItems(items, VIRTUAL_CURRENCIES_COUNT);
            } catch(Exception e)
            {
                throw e;
            } finally {
                Complete();
            }
        }
    }
}
