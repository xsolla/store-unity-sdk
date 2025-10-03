namespace Xsolla.Core
{
	public interface IInAppBrowserNavigationInterceptor
	{
		 bool ShouldAbortNavigation(string url);
	}
}