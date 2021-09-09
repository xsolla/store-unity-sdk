using System;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public interface IDemoAction
	{
		Action OnSuccess { get; set; }
		Action<Error> OnError { get; set; }
	}
}
