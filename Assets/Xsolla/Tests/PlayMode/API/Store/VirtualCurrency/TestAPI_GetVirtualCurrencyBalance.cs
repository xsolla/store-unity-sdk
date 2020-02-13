using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Xsolla.Core;
using Xsolla.Store;

namespace Tests
{
    public class TestAPI_GetVirtualCurrencyBalance : BaseTestApiScript
    {
        private const string VIRTUAL_CURRENCY_MONEY_SKU = "Money";
        private const int VIRTUAL_CURRENCY_MONEY_AMOUNT = 100;

        protected override void Request()
        {
            StoreAPI.GetVirtualCurrencyBalance(XsollaSettings.StoreProjectId, SuccessRequest, FailedRequest);
        }

        private void SuccessRequest(VirtualCurrenciesBalance balance)
        {
            try
            {
                VirtualCurrencyCheck.ValidateBalance(balance);
                CheckMoney(balance);
            } catch(Exception e)
            {
                throw e;
            } finally {
                Complete();
            }
        }

		private void CheckMoney(VirtualCurrenciesBalance balance)
		{
			List<VirtualCurrencyBalance> money = balance.items.ToList()
				.Where(b => b.sku == VIRTUAL_CURRENCY_MONEY_SKU)
				.ToList();
            Assert.IsTrue(money.Count == 1);
            VirtualCurrencyBalance moneyBalance = money.First();
            Assert.IsTrue(moneyBalance.amount == VIRTUAL_CURRENCY_MONEY_AMOUNT);
        }
    }
}
