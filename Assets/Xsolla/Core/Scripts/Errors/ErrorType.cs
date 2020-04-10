namespace Xsolla.Core
{
	public enum ErrorType
	{
		UnknownError,
		NetworkError,
		InvalidToken,

		MethodIsNotAllowed,
		InvalidData,
		ProductDoesNotExist,
		UserNotFound,
		CartNotFound,
		OrderNotFound,

		PasswordResetingNotAllowedForProject,
		TokenVerificationException,
		RegistrationNotAllowedException,
		UsernameIsTaken,
		EmailIsTaken,
		UserIsNotActivated,
		CaptchaRequiredException,
		InvalidProjectSettings,
		InvalidLoginOrPassword,
		MultipleLoginUrlsException,
		SubmittedLoginUrlNotFoundException
	}
}