using UnityEngine;

namespace Xsolla.Demo
{
	public class UrlService
	{
		public void OpenDocumentation()
			=> OpenUrl("https://developers.xsolla.com/sdk/unity/");

		public void OpenIntegrationGuide()
			=> OpenUrl("https://publisher.xsolla.com");

		public void OpenHowItWorks()
			=> OpenUrl("https://developers.xsolla.com/sdk/unity/");

		public void OpenGamerSupport()
			=> OpenUrl("https://xsolla.com/support/gamer-support");

		private void OpenUrl(string url)
			=> Application.OpenURL(url);
	}
}