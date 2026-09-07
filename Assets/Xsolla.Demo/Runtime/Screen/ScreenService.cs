using System.Linq;
using UnityEngine;

namespace Xsolla.Demo
{
	public class ScreenService
	{
		private readonly ScreenStorage ScreenStorage;
		private readonly ScreenPrefabRegistry PrefabRegistry;

		public ScreenService(ScreenStorage screenStorage, ScreenPrefabRegistry prefabRegistry)
		{
			ScreenStorage = screenStorage;
			PrefabRegistry = prefabRegistry;
		}

		public Screen OpenSplashScreen()
			=> Open<SplashScreen>();

		public InfoPopup OpenInfoPopup()
			=> Open<InfoPopup>();

		public Screen OpenLoadingOverlay()
			=> Open<LoadingOverlay>();

		public UserAuthScreen OpenUserAuthScreen()
			=> Open<UserAuthScreen>();

		public Screen OpenOnboardingControls()
			=> Open<OnboardingControlsScreen>();

		public Screen OpenOnboardDocumentation()
			=> Open<OnboardingDocumentationScreen>();

		public Screen OpenOnboardingGameplay()
			=> Open<OnboardingGameplayScreen>();

		public Screen OpenLevelPlayScreen()
			=> Open<LevelPlayScreen>();

		public Screen OpenGameFinishScreen()
			=> Open<GameFinishScreen>();

		public Screen OpenStoreScreen()
			=> Open<StoreScreen>();

		public Screen OpenGameOverScreen()
			=> Open<GameOverScreen>();

		public void Close(Screen screen)
		{
			if (!screen)
				return;

			ScreenStorage.Remove(screen);
			Object.Destroy(screen.gameObject);
		}

		public void CloseAll()
		{
			var screens = ScreenStorage.GetAll().ToList();
			foreach (var screen in screens)
			{
				Close(screen);
			}
		}

		private TScreen Open<TScreen>() where TScreen : Screen
		{
			var instance = ScreenStorage.Get<TScreen>();
			if (instance)
				return instance;

			var prefab = PrefabRegistry.GetPrefab<TScreen>();
			instance = Object.Instantiate(prefab);
			instance.name = prefab.name;

			ScreenStorage.Add(instance);
			return instance;
		}
	}
}