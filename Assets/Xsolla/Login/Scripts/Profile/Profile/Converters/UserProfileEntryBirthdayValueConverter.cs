using System;

namespace Xsolla.Demo
{
	public class UserProfileEntryBirthdayValueConverter : BaseUserProfileValueConverter
	{
		public override string Convert(string value)
		{
			DateTime birthday;
			if (DateTime.TryParse(value, out birthday))
				return birthday.ToString("dd/MM/yyyy").Replace(".","/");
			else
				return value;
		}

		public override string ConvertBack(string value)
		{
			DateTime birthday;
			if (DateTime.TryParse(value, out birthday))
				return birthday.ToString("yyyy-MM-dd");
			else
				return value;
		}
	}
}
