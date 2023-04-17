using System;

namespace Xsolla.UserAccount
{
	[Serializable]
	public class UserPhoneNumber
	{
		/// <summary>
		/// User phone number.
		/// </summary>
		/// <see href="https://developers.xsolla.com/user-account-api/user-phone-number/getusersmephone"/>
		/// <see href="https://en.wikipedia.org/wiki/National_conventions_for_writing_telephone_numbers"/>
		public string phone_number;
	}
}
