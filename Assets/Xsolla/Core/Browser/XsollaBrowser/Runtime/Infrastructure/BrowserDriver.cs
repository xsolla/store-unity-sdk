using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PuppeteerSharp;
using Xsolla.Core;

namespace Xsolla.XsollaBrowser
{
	public class BrowserDriver : IDisposable
	{
		private readonly FetchOptions FetchOptions;
		private readonly LaunchOptions LaunchOptions;
		private readonly CommandsProcessor CommandsProcessor;
		private readonly MainThreadExecutor MainThreadExecutor;
		private readonly MainThreadLogger MainThreadLogger;
		private readonly CancellationToken CancellationToken;
		private readonly Action<int> FetchProgressAction;
		private readonly Action<string> OpenExternalUrlAction;
		private readonly List<IInAppBrowserNavigationInterceptor> NavigationInterceptors = new();

		private static readonly HashSet<ResourceType> RedirectRelevantResourceTypes = new() {
			ResourceType.Document,
			ResourceType.Xhr,
			ResourceType.Fetch,
			ResourceType.Other,
			ResourceType.EventSource,
			ResourceType.WebSocket,
			ResourceType.Manifest
		};

		private Thread Thread;
		private IBrowser Browser;
		private bool IsLaunched;

		public BrowserDriver(
			FetchOptions fetchOptions,
			LaunchOptions launchOptions,
			CommandsProcessor commandsProcessor,
			MainThreadExecutor mainThreadExecutor,
			MainThreadLogger mainThreadLogger,
			CancellationToken cancellationToken,
			Action<int> fetchProgressAction,
			Action<string> openExternalUrlAction)
		{
			FetchOptions = fetchOptions;
			LaunchOptions = launchOptions;
			CommandsProcessor = commandsProcessor;
			MainThreadExecutor = mainThreadExecutor;
			MainThreadLogger = mainThreadLogger;
			CancellationToken = cancellationToken;
			FetchProgressAction = fetchProgressAction;
			OpenExternalUrlAction = openExternalUrlAction;
		}

		public void Dispose()
		{
			Thread?.Join();
		}

		public async Task Launch()
		{
			IsLaunched = false;
			Thread = new Thread(DriverThread);
			Thread.Start();

			while (IsLaunched == false)
			{
				await Task.Delay(50);
			}
		}

		public void AddNavigationInterceptor(IInAppBrowserNavigationInterceptor interceptor)
		{
			if (interceptor == null)
				return;

			if (NavigationInterceptors.Contains(interceptor))
				return;

			NavigationInterceptors.Add(interceptor);
		}

		public void RemoveNavigationInterceptor(IInAppBrowserNavigationInterceptor interceptor)
		{
			if (interceptor == null)
				return;

			if (!NavigationInterceptors.Contains(interceptor))
				return;

			NavigationInterceptors.Remove(interceptor);
		}

		private async void DriverThread()
		{
			MainThreadLogger.Log("Browser driver thread started");
			await Task.Delay(50, CancellationToken);

			var fetcherOptions = new BrowserFetcherOptions {
				Product = FetchOptions.Product,
				Platform = FetchOptions.Platform,
				Path = FetchOptions.Path
			};

			MainThreadLogger.Log($"Fetching browser\n. Options: {JsonConvert.SerializeObject(fetcherOptions)}");

			var fetcher = new BrowserFetcher(fetcherOptions);
			fetcher.DownloadProgressChanged += OnDownloadProgressChanged;
			await fetcher.DownloadAsync(FetchOptions.Revision);
			fetcher.DownloadProgressChanged -= OnDownloadProgressChanged;

			await Task.Delay(50, CancellationToken);

			var launchOptions = new PuppeteerSharp.LaunchOptions {
				Product = FetchOptions.Product,
				Headless = LaunchOptions.Headless,
				Devtools = LaunchOptions.DevTools,
				ExecutablePath = fetcher.GetExecutablePath(FetchOptions.Revision),
				Args = LaunchOptions.Args,
				DefaultViewport = new ViewPortOptions {
					Width = LaunchOptions.ViewportWidth,
					Height = LaunchOptions.ViewportHeight
				}
			};

			MainThreadLogger.Log($"Launching browser\n. Options: {JsonConvert.SerializeObject(launchOptions)}");
			Browser = await Puppeteer.LaunchAsync(launchOptions);
			Browser.TargetCreated += OnBrowserOnTargetCreated;

			await Task.Delay(50, CancellationToken);

			MainThreadLogger.Log("Creating browser page");
			var pages = await Browser.PagesAsync();
			var page = pages.Length > 0
				? pages[0]
				: await Browser.NewPageAsync();

			await page.SetRequestInterceptionAsync(true);
			page.Request += OnPageRequest;

			await Task.Delay(50, CancellationToken);

			IsLaunched = true;
			MainThreadLogger.Log("Start processing commands");

			while (!CancellationToken.IsCancellationRequested)
			{
				try
				{
					await CommandsProcessor.ProcessCommands(page, CancellationToken);
				}
				catch (TaskCanceledException)
				{
					break;
				}
			}

			MainThreadLogger.Log("Browser driver thread was cancelled");

			MainThreadLogger.Log("Closing browser page");
			page.Request -= OnPageRequest;
			await page.CloseAsync();
			page.Dispose();

			MainThreadLogger.Log("Closing browser");
			Browser.TargetCreated -= OnBrowserOnTargetCreated;
			await Browser.CloseAsync();
			Browser.Dispose();

			MainThreadLogger.Log("Browser driver thread finished");
		}

		private void OnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
		{
			FetchProgressAction?.Invoke(e.ProgressPercentage);
		}

		private async void OnPageRequest(object _, RequestEventArgs args)
		{
			try
			{
				var request = args.Request;
				if (request == null)
					return;

				if (!RedirectRelevantResourceTypes.Contains(request.ResourceType))
				{
					await request.ContinueAsync();
					return;
				}

				if (NavigationInterceptors.Count == 0)
				{
					await request.ContinueAsync();
					return;
				}

				if (NavigationInterceptors.Any(i => i.ShouldAbortNavigation(request.Url)))
				{
					await request.AbortAsync();
					return;
				}

				await request.ContinueAsync();
			}
			catch (Exception exception)
			{
				Console.WriteLine(exception);
			}
		}

		private async void OnBrowserOnTargetCreated(object sender, TargetChangedArgs e)
		{
			try
			{
				if (e.Target.Type != TargetType.Page)
					return;

				if (Browser == null)
					return;

				if (CancellationToken.IsCancellationRequested)
					return;

				var allPages = await Browser.PagesAsync();
				if (CancellationToken.IsCancellationRequested)
					return;

				if (allPages.Length <= 1)
					return;

				var newPage = await e.Target.PageAsync();
				if (CancellationToken.IsCancellationRequested)
					return;

				MainThreadExecutor.Enqueue(() => OpenExternalUrlAction?.Invoke(newPage.Url));
				await newPage.CloseAsync();
			}
			catch (Exception exception)
			{
				Console.WriteLine(exception);
			}
		}
	}
}