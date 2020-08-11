using System;

namespace Xsolla.Login
{
	/// <summary>
	/// User's picture upload response entity.
	/// </summary>
	/// <see cref="https://developers.xsolla.com/user-account-api/user-picture/postusersmepicture"/>
	[Serializable]
	public class UserPictureUploadResponse
	{
		/// <summary>
		/// CDN link with the user image.
		/// </summary>
		public string picture;
	}
}