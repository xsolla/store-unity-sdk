namespace Xsolla.Demo
{
	public class LevelStateFactory
	{
		public ILevelState CreateAwakeLevelState()
			=> new AwakeLevelState();

		public ILevelState CreateStoreLevelState()
			=> new StoreLevelState();

		public ILevelState CreateResumeLevelState()
			=> new ResumeLevelState();

		public ILevelState CreateOwlPurchasedLevelState()
			=> new OwlPurchasedLevel();
	}
}