using System;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public interface ILoginAuthorization
	{
		Action<string> OnSuccess { get; set; }
		Action<Error> OnError { get; set; }

		void TryAuth(params object[] args);
	}
}
