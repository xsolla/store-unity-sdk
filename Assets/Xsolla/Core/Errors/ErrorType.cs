namespace Xsolla.Core
{
	public enum ErrorType
	{
		/// <summary>Represents an undefined or uninitialized error state.</summary>
		/// <remarks>
		/// <para><b>Possible Causes:</b></para>
		/// <list type="bullet">
		/// <item><description>The error type has not been set or is default-initialized.</description></item>
		/// <item><description>The error was not mapped correctly from an unexpected API response.</description></item>
		/// </list>
		/// <para><b>Recommended Actions:</b></para>
		/// <description>Inspect the raw error response payload, ensure SDK components are initialized, and check if the error is unhandled in mapping logic.</description>
		/// </remarks>
		Undefined,

		/// <summary>Represents an error of unknown origin or type.</summary>
		/// <remarks>
		/// <para><b>Possible Causes:</b></para>
		/// <list type="bullet">
		/// <item><description>A generic, unhandled exception occurred.</description></item>
		/// <item><description>The backend returned an error code that has no specific mapping in the SDK.</description></item>
		/// </list>
		/// <para><b>Recommended Actions:</b></para>
		/// <description>Check the error logs for additional context, report the issue to support if it persists, or handle it as a fallback generic error.</description>
		/// </remarks>
		UnknownError,

		/// <summary>Indicates a failure during network communication.</summary>
		/// <remarks>
		/// <para><b>Possible Causes:</b></para>
		/// <list type="bullet">
		/// <item><description>No internet connection or DNS resolution failure.</description></item>
		/// <item><description>Network timeout or connection dropped by the server.</description></item>
		/// </list>
		/// <para><b>Recommended Actions:</b></para>
		/// <description>Verify the user's internet connection, implement a retry mechanism with exponential backoff, or check service status.</description>
		/// </remarks>
		NetworkError,

		/// <summary>The user authentication token is invalid, expired, or malformed.</summary>
		/// <remarks>
		/// <para><b>Possible Causes:</b></para>
		/// <list type="bullet">
		/// <item><description>Token lifetime expired or user logged out.</description></item>
		/// <item><description>The token was incorrectly formatted or modified.</description></item>
		/// </list>
		/// <para><b>Recommended Actions:</b></para>
		/// <description>Force user re-authentication to obtain a fresh token, or check that the token is passed correctly in request headers.</description>
		/// </remarks>
		InvalidToken,

		/// <summary>The HTTP request is missing the required authorization header.</summary>
		/// <remarks>
		/// <para><b>Possible Causes:</b></para>
		/// <list type="bullet">
		/// <item><description>The SDK attempted to call a secured API endpoint without attaching the authorization token.</description></item>
		/// </list>
		/// <para><b>Recommended Actions:</b></para>
		/// <description>Ensure the user is logged in before calling this endpoint, or verify the request interceptor attaches the Authorization header.</description>
		/// </remarks>
		AuthorizationHeaderNotSent,

		/// <summary>The requested HTTP method is not allowed by the endpoint.</summary>
		/// <remarks>
		/// <para><b>Possible Causes:</b></para>
		/// <list type="bullet">
		/// <item><description>The SDK sent a POST request to a GET-only endpoint.</description></item>
		/// <item><description>The backend API has been updated or restricted.</description></item>
		/// </list>
		/// <para><b>Recommended Actions:</b></para>
		/// <description>Check the API documentation to verify the allowed HTTP methods, and inspect custom API calls or SDK version compatibility.</description>
		/// </remarks>
		MethodIsNotAllowed,

		/// <summary>The requested operation or SDK feature is not supported on the current target platform.</summary>
		/// <remarks>
		/// <para><b>Possible Causes:</b></para>
		/// <list type="bullet">
		/// <item><description>Platform-specific APIs (such as certain payment options, login methods, or browser components) are invoked on an unsupported build target (e.g. WebGL vs. Mobile).</description></item>
		/// </list>
		/// <para><b>Recommended Actions:</b></para>
		/// <description>Check platform support flags before executing the method, or provide an alternative implementation for the current platform.</description>
		/// </remarks>
		NotSupportedOnCurrentPlatform,

		/// <summary>The request payload contains invalid, incomplete, or malformed data.</summary>
		/// <remarks>
		/// <para><b>Possible Causes:</b></para>
		/// <list type="bullet">
		/// <item><description>Required fields are missing or data formats (e.g., email structure) are incorrect.</description></item>
		/// <item><description>Field validation failed on either the client or server side.</description></item>
		/// </list>
		/// <para><b>Recommended Actions:</b></para>
		/// <description>Validate client-side input parameters before sending the request, and inspect server-side validation error messages.</description>
		/// </remarks>
		InvalidData,

		/// <summary>The requested product or item was not found in the catalog.</summary>
		/// <remarks>
		/// <para><b>Possible Causes:</b></para>
		/// <list type="bullet">
		/// <item><description>Incorrect product SKU or ID provided.</description></item>
		/// <item><description>The product has been deleted or deactivated in the Publisher Account.</description></item>
		/// <item><description>The store catalog is not published.</description></item>
		/// </list>
		/// <para><b>Recommended Actions:</b></para>
		/// <description>Double-check the SKU/ID matches the catalog in the Xsolla Publisher Account, and ensure the catalog is published and active.</description>
		/// </remarks>
		ProductDoesNotExist,

		/// <summary>An error occurred within the Pay Station payment service.</summary>
		/// <remarks>
		/// <para><b>Possible Causes:</b></para>
		/// <list type="bullet">
		/// <item><description>Pay Station backend issues or invalid payment configuration.</description></item>
		/// <item><description>Failed transaction initialization.</description></item>
		/// </list>
		/// <para><b>Recommended Actions:</b></para>
		/// <description>Check the Xsolla service status, verify payment settings (such as project ID and payment methods) in the Publisher Account.</description>
		/// </remarks>
		PayStationServiceException,

		/// <summary>The specified user account could not be found.</summary>
		/// <remarks>
		/// <para><b>Possible Causes:</b></para>
		/// <list type="bullet">
		/// <item><description>Incorrect username, email, or user ID provided during search, login, or inventory operations.</description></item>
		/// </list>
		/// <para><b>Recommended Actions:</b></para>
		/// <description>Prompt the user to check their login credentials, or verify the user ID exists in the Xsolla Publisher Account database.</description>
		/// </remarks>
		UserNotFound,

		/// <summary>The specified shopping cart does not exist or has expired.</summary>
		/// <remarks>
		/// <para><b>Possible Causes:</b></para>
		/// <list type="bullet">
		/// <item><description>The cart ID is invalid.</description></item>
		/// <item><description>The cart has been deleted or expired due to inactivity.</description></item>
		/// </list>
		/// <para><b>Recommended Actions:</b></para>
		/// <description>Create a new cart or verify the active cart ID before performing cart operations.</description>
		/// </remarks>
		CartNotFound,

		/// <summary>The specified order does not exist or cannot be found.</summary>
		/// <remarks>
		/// <para><b>Possible Causes:</b></para>
		/// <list type="bullet">
		/// <item><description>The order ID is invalid.</description></item>
		/// <item><description>The order has been archived or expired.</description></item>
		/// </list>
		/// <para><b>Recommended Actions:</b></para>
		/// <description>Check the order ID, verify the order status via Xsolla Publisher Account, or wait for the order creation webhook to complete.</description>
		/// </remarks>
		OrderNotFound,

		/// <summary>The coupon code entered by the user is invalid, expired, or already used.</summary>
		/// <remarks>
		/// <para><b>Possible Causes:</b></para>
		/// <list type="bullet">
		/// <item><description>Typo in the coupon code or coupon campaign expired.</description></item>
		/// <item><description>Usage limit reached or coupon doesn't apply to items in the cart.</description></item>
		/// </list>
		/// <para><b>Recommended Actions:</b></para>
		/// <description>Prompt the user to verify the coupon code, check campaign dates, and ensure cart items meet coupon criteria.</description>
		/// </remarks>
		InvalidCoupon,

		/// <summary>Password reset is not permitted or configured for the current project.</summary>
		/// <remarks>
		/// <para><b>Possible Causes:</b></para>
		/// <list type="bullet">
		/// <item><description>Password reset functionality is disabled or misconfigured in the Xsolla Login project settings.</description></item>
		/// </list>
		/// <para><b>Recommended Actions:</b></para>
		/// <description>Enable and configure password reset options in the Xsolla Publisher Account under Login settings.</description>
		/// </remarks>
		PasswordResetNotAllowedForProject,

		/// <summary>User registration is not allowed for the project or is temporarily disabled.</summary>
		/// <remarks>
		/// <para><b>Possible Causes:</b></para>
		/// <list type="bullet">
		/// <item><description>Registration settings are disabled in the Login project.</description></item>
		/// <item><description>Restrictions prevent new signups.</description></item>
		/// </list>
		/// <para><b>Recommended Actions:</b></para>
		/// <description>Enable user registration in the Login section of the Xsolla Publisher Account, or check registration filters.</description>
		/// </remarks>
		RegistrationNotAllowedException,

		/// <summary>Token verification failed on the server.</summary>
		/// <remarks>
		/// <para><b>Possible Causes:</b></para>
		/// <list type="bullet">
		/// <item><description>Invalid signature or token tampering.</description></item>
		/// <item><description>Mismatched encryption/decryption keys between the server and identity provider.</description></item>
		/// </list>
		/// <para><b>Recommended Actions:</b></para>
		/// <description>Check Login project settings, verify public keys or client secret configured in the Xsolla Publisher Account.</description>
		/// </remarks>
		TokenVerificationException,

		/// <summary>The chosen username is already registered.</summary>
		/// <remarks>
		/// <para><b>Possible Causes:</b></para>
		/// <list type="bullet">
		/// <item><description>Another user has already registered with the exact username.</description></item>
		/// </list>
		/// <para><b>Recommended Actions:</b></para>
		/// <description>Prompt the user to choose a different, unique username, or suggest variations of their input.</description>
		/// </remarks>
		UsernameIsTaken,

		/// <summary>The chosen email address is already registered.</summary>
		/// <remarks>
		/// <para><b>Possible Causes:</b></para>
		/// <list type="bullet">
		/// <item><description>A user account already exists with the provided email address.</description></item>
		/// </list>
		/// <para><b>Recommended Actions:</b></para>
		/// <description>Prompt the user to log in instead, reset their password, or use a different email address.</description>
		/// </remarks>
		EmailIsTaken,

		/// <summary>The user account is registered but not yet activated or verified.</summary>
		/// <remarks>
		/// <para><b>Possible Causes:</b></para>
		/// <list type="bullet">
		/// <item><description>The user has not clicked the activation link sent to their email.</description></item>
		/// </list>
		/// <para><b>Recommended Actions:</b></para>
		/// <description>Instruct the user to check their email for the activation link, or provide a button to resend the activation email.</description>
		/// </remarks>
		UserIsNotActivated,

		/// <summary>A CAPTCHA verification is required to complete the operation.</summary>
		/// <remarks>
		/// <para><b>Possible Causes:</b></para>
		/// <list type="bullet">
		/// <item><description>Too many failed login or registration attempts.</description></item>
		/// <item><description>Security thresholds triggered on the backend.</description></item>
		/// </list>
		/// <para><b>Recommended Actions:</b></para>
		/// <description>Implement and display a CAPTCHA widget in the user interface, and pass the CAPTCHA response token with the next request.</description>
		/// </remarks>
		CaptchaRequiredException,

		/// <summary>The SDK project configuration settings are invalid or missing.</summary>
		/// <remarks>
		/// <para><b>Possible Causes:</b></para>
		/// <list type="bullet">
		/// <item><description>Missing Project ID, Login ID, or Merchant ID in XsollaSettings.</description></item>
		/// <item><description>Mismatched credentials.</description></item>
		/// </list>
		/// <para><b>Recommended Actions:</b></para>
		/// <description>Check and configure Xsolla project settings in the Unity Editor (Window &gt; Xsolla &gt; Edit Settings).</description>
		/// </remarks>
		InvalidProjectSettings,

		/// <summary>The login credentials (username/email or password) are incorrect.</summary>
		/// <remarks>
		/// <para><b>Possible Causes:</b></para>
		/// <list type="bullet">
		/// <item><description>User typed the wrong password.</description></item>
		/// <item><description>The username or email does not match any registered account.</description></item>
		/// </list>
		/// <para><b>Recommended Actions:</b></para>
		/// <description>Notify the user of invalid credentials, and prompt them to retry or recover their password.</description>
		/// </remarks>
		InvalidLoginOrPassword,

		/// <summary>The authorization code is invalid or has expired.</summary>
		/// <remarks>
		/// <para><b>Possible Causes:</b></para>
		/// <list type="bullet">
		/// <item><description>The OAuth2/OIDC authorization code has already been exchanged or modified.</description></item>
		/// <item><description>The authorization code has timed out.</description></item>
		/// </list>
		/// <para><b>Recommended Actions:</b></para>
		/// <description>Re-initiate the OAuth2 authorization flow to obtain a new authorization code.</description>
		/// </remarks>
		InvalidAuthorizationCode,

		/// <summary>The maximum number of attempts to use or request authorization codes has been exceeded.</summary>
		/// <remarks>
		/// <para><b>Possible Causes:</b></para>
		/// <list type="bullet">
		/// <item><description>Too many consecutive failed attempts to exchange authorization codes.</description></item>
		/// <item><description>Requesting authentication too frequently within a short period.</description></item>
		/// </list>
		/// <para><b>Recommended Actions:</b></para>
		/// <description>Rate-limit requests, inform the user to wait before attempting again, or implement a cooldown timer.</description>
		/// </remarks>
		ExceededAuthorizationCodeAttempts,

		/// <summary>Multiple login URLs are configured or resolved, leading to ambiguity.</summary>
		/// <remarks>
		/// <para><b>Possible Causes:</b></para>
		/// <list type="bullet">
		/// <item><description>Conflicting redirect or callback URL configurations in the Login project settings.</description></item>
		/// </list>
		/// <para><b>Recommended Actions:</b></para>
		/// <description>Check and clean up the callback/redirect URLs in the Xsolla Publisher Account under Login settings.</description>
		/// </remarks>
		MultipleLoginUrlsException,

		/// <summary>The submitted login or redirect URL was not found in the allowed configurations.</summary>
		/// <remarks>
		/// <para><b>Possible Causes:</b></para>
		/// <list type="bullet">
		/// <item><description>The URL passed by the client is not registered in the whitelist of redirect URLs in the Publisher Account.</description></item>
		/// </list>
		/// <para><b>Recommended Actions:</b></para>
		/// <description>Add the requested redirect/callback URL to the allowed list in the Login project settings on the Xsolla Publisher Account.</description>
		/// </remarks>
		SubmittedLoginUrlNotFoundException,

		/// <summary>The social/friend operation cannot be performed due to the current relationship status.</summary>
		/// <remarks>
		/// <para><b>Possible Causes:</b></para>
		/// <list type="bullet">
		/// <item><description>Attempting to accept a request when no pending request exists.</description></item>
		/// <item><description>Attempting to block a user who is already blocked.</description></item>
		/// </list>
		/// <para><b>Recommended Actions:</b></para>
		/// <description>Refresh the user's friend list and state before performing friendship actions, and validate state transitions.</description>
		/// </remarks>
		IncorrectFriendState,

		/// <summary>The operation timed out or reached its maximum allowed time.</summary>
		/// <remarks>
		/// <para><b>Possible Causes:</b></para>
		/// <list type="bullet">
		/// <item><description>Network latency was too high.</description></item>
		/// <item><description>A long-running process (like checking order status) did not complete in time.</description></item>
		/// </list>
		/// <para><b>Recommended Actions:</b></para>
		/// <description>Retry the operation, increase timeout duration if appropriate, or check the status asynchronously later.</description>
		/// </remarks>
		TimeLimitReached
	}
}
