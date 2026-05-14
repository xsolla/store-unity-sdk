namespace Xsolla.Demo
{
	public class ScreenServiceFactory
	{
		public ScreenService Create(ScreenPrefabRegistry screenPrefabRegistry)
		{
			var screenStorage = new ScreenStorage();
			return new ScreenService(screenStorage, screenPrefabRegistry);
		}
	}
}