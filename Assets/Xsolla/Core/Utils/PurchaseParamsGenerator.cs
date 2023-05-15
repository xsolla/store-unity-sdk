using System.Collections.Generic;

namespace Xsolla.Core
{
	internal static class PurchaseParamsGenerator
	{
		public static PurchaseParamsRequest GeneratePurchaseParamsRequest(PurchaseParams purchaseParams)
		{
			var settings = new PurchaseParamsRequest.Settings {
				ui = PayStationUISettings.GenerateSettings(),
				redirect_policy = RedirectPolicySettings.GeneratePolicy(),
				external_id = purchaseParams?.external_id
			};

			if (settings.redirect_policy != null)
				settings.return_url = settings.redirect_policy.return_url;

			//Fix 'The array value is found, but an object is required' in case of empty values.
			if (settings.ui == null && settings.redirect_policy == null && settings.return_url == null)
				settings = null;

			var requestData = new PurchaseParamsRequest {
				sandbox = XsollaSettings.IsSandbox,
				settings = settings,
				custom_parameters = purchaseParams?.custom_parameters,
				currency = purchaseParams?.currency,
				locale = purchaseParams?.locale,
				quantity = purchaseParams?.quantity,
				shipping_data = purchaseParams?.shipping_data,
				shipping_method = purchaseParams?.shipping_method
			};

			return requestData;
		}

		public static List<WebRequestHeader> GeneratePaymentHeaders(Dictionary<string, string> customHeaders = null)
		{
			var headers = new List<WebRequestHeader> {
				WebRequestHeader.AuthHeader()
			};

			if (customHeaders == null)
				return headers;

			foreach (var kvp in customHeaders)
			{
				if (!string.IsNullOrEmpty(kvp.Key) && !string.IsNullOrEmpty(kvp.Value))
					headers.Add(new WebRequestHeader(kvp.Key, kvp.Value));
			}

			return headers;
		}
	}
}