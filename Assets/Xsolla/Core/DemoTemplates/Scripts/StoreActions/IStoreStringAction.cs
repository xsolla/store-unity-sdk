using System;
using Xsolla.Core;

public interface IStoreStringAction
{
	Action<string> OnSuccess { get; set; }
	Action<Error> OnError { get; set; }
}
