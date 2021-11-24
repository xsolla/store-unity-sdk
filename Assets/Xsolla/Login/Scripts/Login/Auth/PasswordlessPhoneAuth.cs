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
			ICodeRequester requester; string phone;
			if (TryExtractArgs(args, out requester, out phone))
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
					OnPasswordlessError(new Error(ErrorType.InvalidData, errorMessage:string.Format("Phone not valid '{0}'", phone)), requester);
			}
			else
			{
				Debug.LogError("PasswordlessPhoneAuth.TryAuth: Could not extract arguments");
				if (base.OnError != null)
					base.OnError.Invoke(new Error(errorMessage: "Passwordless auth failed"));
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
			var regex = new Regex("^\\+(\\d){5,25}string.Format(");
			var valid = regex.IsMatch(phone);

			if (!valid)
				Debug.LogError(string.Format("Phone is not valid: '{0}'", phone));//TEXTREVIEW

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
				onSuccess: token =>
				{
					if (base.OnSuccess != null)
						base.OnSuccess.Invoke(token);
				},
				onError: OnCodeError);
		}

		private void OnPasswordlessError(Error error, ICodeRequester requester)
		{
			requester.RaiseOnError(error);
			if (base.OnError != null)
				base.OnError.Invoke(error);
		}

		private void OnCodeError(Error error)
		{
			_currentRequester.RaiseOnError(error);
		}
	}
}
