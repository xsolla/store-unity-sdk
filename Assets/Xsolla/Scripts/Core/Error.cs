using System;
using System.Collections.Generic;

namespace Xsolla.Core
{
	[Serializable]
	public class Error
	{
		public string statusCode;
		public string errorCode;
		public string errorMessage;

		public ErrorType ErrorType { get; set; }

		public static Error NetworkError
		{
			get { return new Error {ErrorType = ErrorType.NetworkError}; }
		}
		
		public static Error UnknownError
		{
			get { return new Error {ErrorType = ErrorType.UnknownError}; }
		}

		public static readonly Dictionary<string, ErrorType> GeneralErrors =
			new Dictionary<string, ErrorType>()
			{
				{"403", ErrorType.InvalidToken},
				{"405", ErrorType.MethodIsNotAllowed},
			};
		
		public static readonly Dictionary<string, ErrorType> ItemsListErrors =
			new Dictionary<string, ErrorType>()
			{
				{"422", ErrorType.InvalidData},
			};

		public static readonly Dictionary<string, ErrorType> BuyItemErrors =
			new Dictionary<string, ErrorType>()
			{
				{"422", ErrorType.ProductDoesNotExist},
			};

		public static readonly Dictionary<string, ErrorType> ConsumeItemErrors =
			new Dictionary<string, ErrorType>()
			{
				{"422", ErrorType.InvalidData},
			};

		public static readonly Dictionary<string, ErrorType> CreateCartErrors =
			new Dictionary<string, ErrorType>()
			{
				{"403", ErrorType.InvalidToken},
				{"404", ErrorType.UserNotFound},
				{"422", ErrorType.InvalidData},
			};
		
		public static readonly Dictionary<string, ErrorType> AddToCartCartErrors =
			new Dictionary<string, ErrorType>()
			{
				{"403", ErrorType.InvalidToken},
				{"404", ErrorType.CartNotFound},
				{"422", ErrorType.InvalidData},
			};
		
		public static readonly Dictionary<string, ErrorType> GetCartItemsErrors =
			new Dictionary<string, ErrorType>()
			{
				{"403", ErrorType.InvalidToken},
				{"404", ErrorType.CartNotFound},
				{"422", ErrorType.InvalidData},
			};
		
		public static readonly Dictionary<string, ErrorType> DeleteFromCartErrors =
			new Dictionary<string, ErrorType>()
			{
				{"403", ErrorType.InvalidToken},
				{"404", ErrorType.CartNotFound},
				{"422", ErrorType.InvalidData},
			};
		
		public static readonly Dictionary<string, ErrorType> BuyCartErrors =
			new Dictionary<string, ErrorType>()
			{
				{"422", ErrorType.CartNotFound},
			};
		
		public static readonly Dictionary<string, ErrorType> OrderStatusErrors =
			new Dictionary<string, ErrorType>()
			{
				{"401", ErrorType.InvalidToken},
				{"404", ErrorType.OrderNotFound},
			};
		
		public override string ToString()
		{
			return string.Format("Error - Type: {0}. Status code: {1}. Error code: {2}. Error message: {3}.", ErrorType, statusCode, errorCode, errorMessage);
		}
	}
}
