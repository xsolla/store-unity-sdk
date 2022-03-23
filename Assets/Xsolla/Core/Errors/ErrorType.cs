namespace Xsolla.Core
{
	public enum ErrorType
	{
		Undefined,

		UnknownError,
		NetworkError,

		InvalidToken,
		AuthorizationHeaderNotSent,

		MethodIsNotAllowed,
		InvalidData,
		ProductDoesNotExist,
		UserNotFound,
		CartNotFound,
		OrderNotFound,
		InvalidCoupon,

		PasswordResetingNotAllowedForProject,
		TokenVerificationException,
		RegistrationNotAllowedException,
		UsernameIsTaken,
		EmailIsTaken,
		UserIsNotActivated,
		CaptchaRequiredException,
		InvalidProjectSettings,
		InvalidLoginOrPassword,
		InvalidAuthorizationCode,
		ExceededAuthorizationCodeAttempts,
		MultipleLoginUrlsException,
		SubmittedLoginUrlNotFoundException,

		IncorrectFriendState
	}
}