using System;
using System.Collections;
using Xsolla.Auth;
using Xsolla.Core;

namespace Xsolla.TechDemo.Scenarios
{
	public abstract class Scenario
	{
		public virtual void SetUp()
		{
			ClearAuthData();
		}

		public abstract IEnumerator Execute(Action onSuccess, Action<Error> onError);

		public virtual void TearDown()
		{
		}

		protected static void SetInAppBrowserActive(bool isActive)
		{
			XsollaSettings.InAppBrowserEnabled = isActive;
		}

		protected static void CloseInAppBrowser()
		{
			XsollaWebBrowser.Close();
		}

		protected static void ClearAuthData()
		{
			XsollaToken.DeleteSavedInstance();
		}
	}
}