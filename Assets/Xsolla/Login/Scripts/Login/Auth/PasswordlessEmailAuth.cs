using Xsolla.Core;

namespace Xsolla.Demo
{
	public class PasswordlessEmailAuth : LoginAuthorization
	{
		private string _currentEmail;
		private string _currentOperationID;
		private ICodeRequester _currentRequester;

		public override void TryAuth(params object[] args)
		{
			if (TryExtractArgs(args, out var requester, out var email))
			{
				if (IsEmailValid(email))
				{
					SdkLoginLogic.Instance.StartAuthByEmail(
						email:email,
						linkUrl: null,
						sendLink:false,
						onSuccess: id => OnStartAuth(email, id, requester),
						onError: error => OnPasswordlessError(error,requester));
				}
				else
					OnPasswordlessError(new Error(ErrorType.InvalidData, errorMessage:$"Email not valid '{email}'"), requester);
			}
			else
			{
				Debug.LogError("PasswordlessEmailAuth.TryAuth: Could not extract arguments");
				base.OnError?.Invoke(new Error(errorMessage: "Passwordless auth failed"));
			}
		}

		private bool TryExtractArgs(object[] args, out ICodeRequester requester, out string email)
		{
			requester = null;
			email = null;

			if (args.Length >= 2 && args[0] is ICodeRequester && args[1] is string)
			{
				requester = (ICodeRequester)args[0];
				email = (string)args[1];
				return true;
			}
			else
			{
				//TEXTREVIEW
				Debug.LogError("PasswordlessEmailAuth: Could not extract arguments");
				return false;
			}
		}

		private bool IsEmailValid(string email)
		{
			return (email.Length > 1 && email.Length < 255);
		}

		private void OnStartAuth(string email, string operationID, ICodeRequester requester)
		{
			_currentEmail = email;
			_currentOperationID = operationID;
			_currentRequester = requester;
			requester.RequestCode(OnCode);
		}

		private void OnCode(string code)
		{
			SdkLoginLogic.Instance.CompleteAuthByEmail(
				email: _currentEmail,
				confirmationCode: code,
				operationId: _currentOperationID,
				onSuccess: token => base.OnSuccess?.Invoke(token),
				onError: OnCodeError);
		}

		private void OnPasswordlessError(Error error, ICodeRequester requester)
		{
			requester.RaiseOnError(error);
			base.OnError?.Invoke(error);
		}

		private void OnCodeError(Error error)
		{
			_currentRequester.RaiseOnError(error);
		}
	}
}
