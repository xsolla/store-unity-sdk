using System;
using System.Collections.Generic;

namespace Xsolla.Core
{
	public partial class Error
	{		
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
	}
}
