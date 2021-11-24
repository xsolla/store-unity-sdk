using Xsolla.Core;

namespace Xsolla.Demo
{
	public class SteamAuth : LoginAuthorization
	{
		private string _steamSessionTicket;

		public override void TryAuth(params object[] args)
		{
			if (!XsollaSettings.UseSteamAuth)
			{
				Debug.Log("SteamAuth.TryAuth: Steam auth disabled");
				if (base.OnError != null)
					base.OnError.Invoke(null);
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
				{
					if (base.OnError != null)
						base.OnError.Invoke(new Error(errorMessage: "Steam auth failed"));
				}
			}
		}

		private void RequestTokenBy(string ticket)
		{
			int parsed;
			if (int.TryParse(XsollaSettings.SteamAppId, out parsed))
			{
				SdkLoginLogic.Instance.SteamAuth(XsollaSettings.SteamAppId, ticket, onSuccess:SuccessHandler, onError:FailHandler);
			}
			else
			{
				Debug.LogError(string.Format("Can't parse SteamAppId = {0}", XsollaSettings.SteamAppId));
				if (base.OnError != null)
					base.OnError.Invoke(new Error(errorMessage: "Steam auth failed"));
			}
		}

		private void SuccessHandler(string token)
		{
			Debug.Log("SteamAuth.SuccessHandler: Token loaded");
			if (base.OnSuccess != null)
				base.OnSuccess.Invoke(token);
		}

		private void FailHandler(Error error)
		{
			Debug.LogError(string.Format("Token request by steam session ticket failed. Ticket: {0} Error: {1}", _steamSessionTicket, error.ToString()));
			if (base.OnError != null)
				base.OnError.Invoke(error);
		}
	}
}
