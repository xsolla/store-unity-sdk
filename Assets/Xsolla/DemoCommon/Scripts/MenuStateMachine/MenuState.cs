namespace Xsolla.Demo
{
	public enum MenuState
	{
		Authorization = 0,
		AuthorizationFailed = 1,
		Registration = 2,
		RegistrationFailed = 3,
		RegistrationSuccess = 4,
		ChangePassword = 5,
		ChangePasswordFailed = 6,
		ChangePasswordSuccess = 7,
		LoginSettingsError = 8,
		Main = 10,
		Store = 11,
		BuyCurrency = 12,
		Cart = 13,
		Battlepass = 14,
		Inventory = 15,
		Profile = 16,
		Character = 17,
		Friends = 18,
		SocialFriends = 19,
		None = 20
	}

	public static class MenuStateExtension
	{
		public static bool IsAuthState(this MenuState menuState) => (int)menuState < 10;
		public static bool IsPostAuthState(this MenuState menuState) => !menuState.IsAuthState() && menuState != MenuState.None;
	}
}
