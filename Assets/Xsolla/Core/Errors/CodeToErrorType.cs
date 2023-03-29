namespace Xsolla.Core
{
	public static class CodeToErrorType
	{
		public static bool TryGetSpecificType(string code, ErrorCheckType checkType, out ErrorType errorType)
		{
			if (checkType == ErrorCheckType.CommonErrors)
				return TryGetCommonType(code, out errorType);

			errorType = ErrorType.Undefined;

			switch (checkType)
			{
				case ErrorCheckType.TokenErrors:
					if (string.Equals(code,"422"))
						errorType = ErrorType.TokenVerificationException;
					break;

				case ErrorCheckType.LoginErrors:
					switch (code)
					{
						case "003-007": { errorType = ErrorType.UserIsNotActivated; }; break;
						case "010-007": { errorType = ErrorType.CaptchaRequiredException; }; break;
						case "003-039": { errorType = ErrorType.InvalidAuthorizationCode; }; break;
						case "003-049": { errorType = ErrorType.ExceededAuthorizationCodeAttempts; }; break;
					}
					break;

				case ErrorCheckType.RegistrationErrors:
					switch (code)
					{
						case "010-003": { errorType = ErrorType.RegistrationNotAllowedException; }; break;
						case "003-003": { errorType = ErrorType.UsernameIsTaken; }; break;
						case "003-004": { errorType = ErrorType.EmailIsTaken; }; break;
					}
					break;

				case ErrorCheckType.ResetPasswordErrors:
					switch (code)
					{
						case "003-007": { errorType = ErrorType.PasswordResetingNotAllowedForProject; }; break;
						case "003-024": { errorType = ErrorType.PasswordResetingNotAllowedForProject; }; break;
					}
					break;

				case ErrorCheckType.ItemsListErrors:
				case ErrorCheckType.ConsumeItemErrors:
					switch (code)
					{
						case "401": { errorType = ErrorType.InvalidToken; }; break;
						case "422": { errorType = ErrorType.InvalidData; }; break;
					}
					break;

				case ErrorCheckType.BuyItemErrors:
					if (string.Equals(code,"422"))
						errorType = ErrorType.ProductDoesNotExist;
					break;

				case ErrorCheckType.CreateCartErrors:
					switch (code)
					{
						case "401":
						case "403": { errorType = ErrorType.InvalidToken; }; break;
						case "404": { errorType = ErrorType.UserNotFound; }; break;
						case "422": { errorType = ErrorType.InvalidData; }; break;
					}
					break;

				case ErrorCheckType.AddToCartCartErrors:
				case ErrorCheckType.GetCartItemsErrors:
				case ErrorCheckType.DeleteFromCartErrors:
					switch (code)
					{
						case "401":
						case "403": { errorType = ErrorType.InvalidToken; }; break;
						case "404": { errorType = ErrorType.CartNotFound; }; break;
						case "422": { errorType = ErrorType.InvalidData; }; break;
					}
					break;

				case ErrorCheckType.BuyCartErrors:
					if (string.Equals(code,"422"))
						errorType = ErrorType.CartNotFound;
					break;

				case ErrorCheckType.OrderStatusErrors:
					switch (code)
					{
						case "401": { errorType = ErrorType.InvalidToken; }; break;
						case "404": { errorType = ErrorType.OrderNotFound; }; break;
					}
					break;

				case ErrorCheckType.CouponErrors:
					switch (code)
					{
						case "401": { errorType = ErrorType.InvalidToken; }; break;
						case "403": { errorType = ErrorType.AuthorizationHeaderNotSent; }; break;
						case "404": { errorType = ErrorType.InvalidCoupon; }; break;
						case "422": { errorType = ErrorType.InvalidData; }; break;
					}
					break;
			}

			return errorType != ErrorType.Undefined;
		}

		public static bool TryGetCommonType(string code, out ErrorType errorType)
		{
			errorType = ErrorType.Undefined;

			switch (code)
			{
				case "403":
				case "010-023":
				case "010-017": { errorType = ErrorType.InvalidToken; }; break;
				case "405":		{ errorType = ErrorType.MethodIsNotAllowed; }; break;
				case "0":
				case "003-061": { errorType = ErrorType.InvalidProjectSettings; }; break;
				case "003-001": { errorType = ErrorType.InvalidLoginOrPassword; }; break;
				case "010-011": { errorType = ErrorType.MultipleLoginUrlsException; }; break;
				case "010-012": { errorType = ErrorType.SubmittedLoginUrlNotFoundException; }; break;
			}

			return errorType != ErrorType.Undefined;
		}
	}
}
