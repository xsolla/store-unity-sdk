using System;
using UnityEngine;

namespace Xsolla.Core
{
	[Serializable]
	public class RedirectPolicySettings
	{
		public bool IsFoldout;

		public bool IsOverride;

		public string ReturnUrl;

		public RedirectConditionsType RedirectConditions;

		public int Delay;

		public StatusForManualRedirectionType StatusForManualRedirection;

		public string RedirectButtonCaption;

		public static RedirectPolicy GeneratePolicy()
		{
#if UNITY_ANDROID
			if (XsollaSettings.AndroidRedirectPolicySettings.IsOverride)
			{
				return XsollaSettings.AndroidRedirectPolicySettings.CreatePolicy();
			}
			else
			{
				return new RedirectPolicy{
					return_url = $"app://xpayment.{Application.identifier}",
					redirect_conditions = RedirectConditionsType.Any.ToString().ToLowerInvariant(),
					delay = 0,
					status_for_manual_redirection = StatusForManualRedirectionType.None.ToString().ToLowerInvariant(),
					redirect_button_caption = string.Empty
				};
			}
#elif UNITY_WEBGL
			if (XsollaSettings.WebglRedirectPolicySettings.IsOverride)
			{
				return XsollaSettings.WebglRedirectPolicySettings.CreatePolicy();
			}
			else
			{
				return null;
			}
#else
			if (XsollaSettings.DesktopRedirectPolicySettings.IsOverride)
			{
				return XsollaSettings.DesktopRedirectPolicySettings.CreatePolicy();
			}
			else
			{
				return null;
			}
#endif
		}

		private RedirectPolicy CreatePolicy()
		{
			return new RedirectPolicy
			{
				return_url = ReturnUrl,
				redirect_conditions = ConvertToString(RedirectConditions),
				delay = Delay,
				status_for_manual_redirection = ConvertToString(StatusForManualRedirection),
				redirect_button_caption = RedirectButtonCaption
			};
		}

		private string ConvertToString(RedirectConditionsType conditions)
		{
			switch (conditions)
			{
				case RedirectConditionsType.None:                 return "none";
				case RedirectConditionsType.Successful:           return "successful";
				case RedirectConditionsType.SuccessfulOrCanceled: return "successful_or_canceled";
				case RedirectConditionsType.Any:                  return "any";
				default:                                          return "none";
			}
		}

		private string ConvertToString(StatusForManualRedirectionType status)
		{
			switch (status)
			{
				case StatusForManualRedirectionType.None:                 return "none";
				case StatusForManualRedirectionType.Vc:                   return "vc";
				case StatusForManualRedirectionType.Successful:           return "successful";
				case StatusForManualRedirectionType.SuccessfulOrCanceled: return "successful_or_canceled";
				default:                                                  return "none";
			}
		}

		public enum RedirectConditionsType
		{
			None,
			Successful,
			SuccessfulOrCanceled,
			Any
		}

		public enum StatusForManualRedirectionType
		{
			None,
			Vc,
			Successful,
			SuccessfulOrCanceled
		}
	}
}