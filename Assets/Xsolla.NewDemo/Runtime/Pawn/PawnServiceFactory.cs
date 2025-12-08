namespace Xsolla.Demo
{
	public class PawnServiceFactory
	{
		public PawnService Create(PawnPrefabRegistry prefabRegistry)
		{
			return new PawnService(prefabRegistry);
		}
	}
}