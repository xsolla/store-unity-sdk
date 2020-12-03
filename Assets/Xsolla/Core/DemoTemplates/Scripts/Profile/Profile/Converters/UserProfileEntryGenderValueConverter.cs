namespace Xsolla.Demo
{
	public class UserProfileEntryGenderValueConverter : BaseUserProfileValueConverter
	{
		public override string Convert(string value)
		{
			if (value == UserProfileGender.MALE_SHORT)
				return UserProfileGender.MALE;

			if (value == UserProfileGender.FEMALE_SHORT)
				return UserProfileGender.FEMALE;

			return value;
		}

		public override string ConvertBack(string value)
		{
			if (value == UserProfileGender.MALE)
				return UserProfileGender.MALE_SHORT;

			if (value == UserProfileGender.FEMALE)
				return UserProfileGender.FEMALE_SHORT;

			return value;
		}
	}
}
