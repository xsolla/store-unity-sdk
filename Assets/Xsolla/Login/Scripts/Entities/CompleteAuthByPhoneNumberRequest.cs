using System;

namespace Xsolla.Login
{
	[Serializable]
	public class CompleteAuthByPhoneNumberRequest
	{
		public string phone_number;
		public string code;
		public string operation_id;

		public CompleteAuthByPhoneNumberRequest(string phoneNumber, string code, string operationId)
		{
			this.code = code;
			phone_number = phoneNumber;
			operation_id = operationId;
		}
	}
}