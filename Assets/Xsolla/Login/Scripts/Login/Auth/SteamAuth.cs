using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class SteamAuth : StoreStringActionResult, ILoginAuthorization
	{
		private string _steamSessionTicket = default;

		public void TryAuth(params object[] args)
		{
			if (!XsollaSettings.UseSteamAuth)
			{
				Debug.Log("SteamAuth.TryAuth: Steam auth disabled");
				base.OnError?.Invoke(null);
			}
			else
			{
				Debug.Log("SteamAuth.TryAuth: Steam auth enabled, trying to get token");

	#if UNITY_STANDALONE || UNITY_EDITOR
				_steamSessionTicket = new SteamSessionTicket().ToString();
	#endif
				if (!string.IsNullOrEmpty(_steamSessionTicket))
					RequestTokenBy(_steamSessionTicket);
				else
					base.OnError?.Invoke(new Error(errorMessage: "Steam auth failed"));
			}
		}

		private void RequestTokenBy(string ticket)
		{
			if (int.TryParse(XsollaSettings.SteamAppId, out _))
			{
				DemoController.Instance.LoginDemo.SteamAuth(XsollaSettings.SteamAppId, ticket, SuccessHandler, FailHandler);
			}
			else
			{
				Debug.LogError($"Can't parse SteamAppId = {XsollaSettings.SteamAppId}");
				base.OnError?.Invoke(new Error(errorMessage: "Steam auth failed"));
			}
		}

		private void SuccessHandler(string token)
		{
			Debug.Log("SteamAuth.SuccessHandler: Token loaded");
			base.OnSuccess?.Invoke(token);
		}

		private void FailHandler(Error error)
		{
			Debug.LogError($"Token request by steam session ticket failed. Ticket: {_steamSessionTicket} Error: {error.ToString()}");
			base.OnError?.Invoke(error);
		}
	}
}
