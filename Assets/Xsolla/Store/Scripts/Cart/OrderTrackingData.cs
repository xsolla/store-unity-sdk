using System;
using Xsolla.Core;

namespace Xsolla.Store
{
	public class OrderTrackingData
	{
		public string ProjectId { get; set; }

		public int OrderId { get; set; }

		public Action SuccessCallback { get; set; }

		public Action<Error> ErrorCallback { get; set; }
	}
}