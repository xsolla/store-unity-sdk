using System;

namespace Xsolla.Core
{
	public interface IInAppBrowser
	{
		void Open(string url);

		void Close(float delay = 0f);

		void AddInitHandler(Action callback);

		void AddCloseHandler(Action callback);

		void AddUrlChangeHandler(Action<string> callback);

		void UpdateSize(int width, int height);
	}
}