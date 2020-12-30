using System;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public interface IStoreActionResult
	{
		Action OnSuccess { get; set; }
		Action<Error> OnError { get; set; }
	}
}
