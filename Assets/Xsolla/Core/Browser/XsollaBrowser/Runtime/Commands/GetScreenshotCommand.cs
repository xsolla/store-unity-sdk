using System;
using System.Threading.Tasks;
using PuppeteerSharp;

namespace Xsolla.XsollaBrowser
{
	public class GetScreenshotCommand : IBrowserCommand
	{
		private readonly Action<byte[]> Callback;

		public GetScreenshotCommand(Action<byte[]> callback)
		{
			Callback = callback;
		}

		public async Task Perform(IPage page)
		{
			var options = new ScreenshotOptions {
				Type = ScreenshotType.Jpeg,
				Quality = 100
			};

			try
			{
				var data = await page.ScreenshotDataAsync(options);
				Callback?.Invoke(data);
			}
			catch (Exception)
			{
				// Invoke with null so RenderLoop doesn't hang (e.g. "Not attached to an active page"
				// when page is detached during navigation/redirect or request abort)
				Callback?.Invoke(null);
				throw; // Re-throw so CommandsProcessor can log it
			}
		}
	}
}