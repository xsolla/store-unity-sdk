using System;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public interface IStoreStringAction
	{
		Action<string> OnSuccess { get; set; }
		Action<Error> OnError { get; set; }
	}
}
