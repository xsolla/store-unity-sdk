using System;
using System.Collections;
using System.Net;
using System.Text;
using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Auth
{
	internal class LocalAuthHttpListener
	{
		private readonly string RedirectUrl;
		private readonly Action SuccessCallback;
		private readonly Action<Error> ErrorCallback;
		private Coroutine Coroutine;
		private HttpListener Listener;

		public LocalAuthHttpListener(string redirectUrl, Action successCallback, Action<Error> errorCallback)
		{
			RedirectUrl = redirectUrl;
			SuccessCallback = successCallback;
			ErrorCallback = errorCallback;
		}

		public void Start()
		{
			if (string.IsNullOrEmpty(RedirectUrl))
			{
				var error = new Error(errorMessage: "Redirect URL is null or empty.");
				XDebug.LogError(error.errorMessage);
				ErrorCallback?.Invoke(error);
				return;
			}

			if (Listener != null && Listener.IsListening)
			{
				XDebug.LogWarning("HTTP listener is already running");
				return;
			}

			Listener = new HttpListener();

			var prefix = RedirectUrl.EndsWith("/")
				? RedirectUrl
				: RedirectUrl + "/";
			Listener.Prefixes.Add(prefix);

			Coroutine = CoroutinesExecutor.Run(ProcessListener());
		}

		public void Stop()
		{
			if (Coroutine != null)
			{
				CoroutinesExecutor.Stop(Coroutine);
				Coroutine = null;
			}

			if (Listener != null)
			{
				try
				{
					if (Listener.IsListening)
						Listener.Stop();
				}
				catch (Exception e)
				{
					XDebug.LogWarning($"Error stopping HTTP listener: {e.Message}");
				}

				Listener.Close();
				Listener = null;
				XDebug.Log("HTTP listener stopped and closed");
			}
		}

		private IEnumerator ProcessListener()
		{
			try
			{
				Listener.Start();
			}
			catch (Exception e)
			{
				var error = new Error(errorMessage: $"Failed to start HTTP listener: {e.Message}");
				ErrorCallback?.Invoke(error);
				yield break;
			}

			if (!Listener.IsListening)
			{
				var error = new Error(errorMessage: "Failed to start HTTP listener");
				ErrorCallback?.Invoke(error);
				yield break;
			}

			XDebug.Log($"HTTP listener started and waiting for connections on {RedirectUrl}");
			var getContextTask = Listener.GetContextAsync();
			var timeout = 300f;
			var startTime = Time.realtimeSinceStartup;

			while (!getContextTask.IsCompleted)
			{
				if (Time.realtimeSinceStartup - startTime > timeout)
				{
					ErrorCallback?.Invoke(new Error(errorMessage: "HTTP listener timed out waiting for incoming connections"));
					Stop();
					yield break;
				}

				yield return null;
			}

			try
			{
				var context = getContextTask.Result;
				var requestUrl = context.Request.RawUrl;
				XDebug.Log($"Incoming request: {context.Request.HttpMethod} {context.Request.RawUrl}");

				if (ParseUtils.TryGetValueFromUrl(requestUrl, ParseParameter.code, out var code))
				{
					XsollaAuth.ExchangeCodeToToken(
						code,
						() => ProcessSuccess(context),
						error => ProcessError(context, error.errorMessage),
						redirectUri: RedirectUrl);
				}
				else if (ParseUtils.TryGetValueFromUrl(requestUrl, ParseParameter.token, out var token))
				{
					XsollaToken.Create(token);
					ProcessSuccess(context);
				}
				else
				{
					ProcessError(context, "Missing authorization code or token in the redirect URL");
				}
			}
			catch (Exception ex)
			{
				var error = new Error(errorMessage: $"Error processing HTTP request: {ex.Message}");
				XDebug.LogError(error.errorMessage);
				ErrorCallback?.Invoke(error);
				Stop();
			}
		}

		private void ProcessSuccess(HttpListenerContext context)
		{
			var response = context.Response;
			var responseString = GetSuccessHtmlResponseText();
			SendResponse(response, responseString);
			SuccessCallback?.Invoke();
			Stop();
		}

		private void ProcessError(HttpListenerContext context, string errorMessage)
		{
			var response = context.Response;
			var responseString = GetFailureHtmlResponseText();
			SendResponse(response, responseString);
			ErrorCallback?.Invoke(new Error(errorMessage: errorMessage));
			Stop();
		}

		private static string GetSuccessHtmlResponseText()
		{
			return "<!DOCTYPE html><html><head><title>Page Title</title></head><body><h1>You're logged in</h1><p>Close this tab and return to game</p></body></html>";
		}

		private static string GetFailureHtmlResponseText()
		{
			return "<!DOCTYPE html><html><body><h1>Login failed</h1></body></html>";
		}

		private static void SendResponse(HttpListenerResponse response, string html)
		{
			try
			{
				var buffer = Encoding.UTF8.GetBytes(html);
				response.ContentLength64 = buffer.Length;
				response.OutputStream.Write(buffer, 0, buffer.Length);
			}
			catch (Exception e)
			{
				XDebug.LogWarning($"Failed to send HTTP response: {e.Message}");
			}
			finally
			{
				response.Close();
			}
		}
	}
}