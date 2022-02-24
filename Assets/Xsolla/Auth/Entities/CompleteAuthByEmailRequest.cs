using System;

namespace Xsolla.Auth
{
	[Serializable]
	public class CompleteAuthByEmailRequest
	{
		public string email;
		public string code;
		public string operation_id;

		public CompleteAuthByEmailRequest(string email, string code, string operationId)
		{
			this.code = code;
			this.email = email;
			operation_id = operationId;
		}
	}
}
