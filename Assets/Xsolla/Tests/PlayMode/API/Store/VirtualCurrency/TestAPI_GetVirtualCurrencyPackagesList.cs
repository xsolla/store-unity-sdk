using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Xsolla.Core;
using Xsolla.Store;

namespace Tests
{
    public class TestAPI_GetVirtualCurrencyPackagesList : BaseTestApiScript
    {
        protected override void Request()
        {
            StoreAPI.GetVirtualCurrencyPackagesList(XsollaSettings.StoreProjectId, SuccessRequest, FailedRequest);
        }

        private void SuccessRequest(VirtualCurrencyPackages packages)
        {
            try
            {
                VirtualCurrencyCheck.ValidatePackages(packages);
            } catch(Exception e)
            {
                throw e;
            } finally {
                Complete();
            }
        }
    }
}
