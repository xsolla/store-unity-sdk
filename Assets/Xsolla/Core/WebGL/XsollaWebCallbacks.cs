using System;
using UnityEngine;

namespace Xsolla.Core
{
	internal class XsollaWebCallbacks : MonoBehaviour
	{
		private event Action OnPaymentStatusUpdate;
		private event Action OnPaymentCancel;

		public event Action<string> WidgetAuthSuccess;
		public event Action WidgetAuthCancel;

		private void Awake()
		{
			DontDestroyOnLoad(gameObject);
		}

		// Callback for Xsolla Pay Station (do not remove)
		public void PublishPaymentStatusUpdate()
		{
			OnPaymentStatusUpdate?.Invoke();
		}

		// Callback for Xsolla Pay Station (do not remove)
		public void PublishPaymentCancel()
		{
			OnPaymentCancel?.Invoke();
		}

		// Callback for Xsolla Widget (do not remove)
		public void PublishWidgetAuthSuccess(string data)
		{
			WidgetAuthSuccess?.Invoke(data);
		}

		// Callback for Xsolla Widget (do not remove)
		public void PublishWidgetAuthCancel()
		{
			WidgetAuthCancel?.Invoke();
		}

		public static void AddPaymentStatusUpdateHandler(Action action)
		{
			Instance.OnPaymentStatusUpdate += action;
		}

		public static void RemovePaymentStatusUpdateHandler(Action action)
		{
			Instance.OnPaymentStatusUpdate -= action;
		}

		public static void AddPaymentCancelHandler(Action action)
		{
			Instance.OnPaymentCancel += action;
		}

		public static void RemovePaymentCancelHandler(Action action)
		{
			Instance.OnPaymentCancel -= action;
		}

		#region Singleton

		private static XsollaWebCallbacks _instance;

		public static XsollaWebCallbacks Instance
		{
			get
			{
				if (!_instance)
					_instance = new GameObject("XsollaWebCallbacks").AddComponent<XsollaWebCallbacks>();

				return _instance;
			}
		}

		#endregion
	}
}