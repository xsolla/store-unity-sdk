using System.Text.RegularExpressions;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class PasswordlessPhoneAuth : LoginAuthorization
	{
		private string _currentPhone;
		private string _currentOperationID;
		private ICodeRequester _currentRequester;

		public override void TryAuth(params object[] args)
		{
			if (TryExtractArgs(args, out var requester, out var phone))
			{
				if (IsPhoneValid(phone))
				{
					SdkLoginLogic.Instance.StartAuthByPhoneNumber(
						phoneNumber:phone,
						linkUrl: null,
						sendLink:false,
						onSuccess: id => OnStartAuth(phone, id, requester),
						onError: error => OnPasswordlessError(error,requester));
				}
				else
					OnPasswordlessError(new Error(ErrorType.InvalidData, errorMessage:$"Phone not valid '{phone}'"), requester);
			}
			else
			{
				Debug.LogError("PasswordlessPhoneAuth.TryAuth: Could not extract arguments");
				base.OnError?.Invoke(new Error(errorMessage: "Passwordless auth failed"));
			}
		}

		private bool TryExtractArgs(object[] args, out ICodeRequester requester, out string phone)
		{
			requester = null;
			phone = null;

			if (args.Length >= 2 && args[0] is ICodeRequester && args[1] is string)
			{
				requester = (ICodeRequester)args[0];
				phone = (string)args[1];
				return true;
			}
			else
			{
				//TEXTREVIEW
				Debug.LogError("PasswordlessPhoneAuth: Could not extract arguments");
				return false;
			}
		}

		private bool IsPhoneValid(string phone)
		{
			var regex = new Regex("^\\+(\\d){5,25}$");
			var valid = regex.IsMatch(phone);

			if (!valid)
				Debug.LogError($"Phone is not valid: '{phone}'");//TEXTREVIEW

			return valid;
		}

		private void OnStartAuth(string phone, string operationID, ICodeRequester requester)
		{
			_currentPhone = phone;
			_currentOperationID = operationID;
			_currentRequester = requester;
			requester.RequestCode(OnCode);
		}

		private void OnCode(string code)
		{
			SdkLoginLogic.Instance.CompleteAuthByPhoneNumber(
				phoneNumber: _currentPhone,
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
