using System;
using Xsolla.Core;

public interface IStoreActionResult
{
	Action OnSuccess { get; set; }
	Action<Error> OnError { get; set; }
}
