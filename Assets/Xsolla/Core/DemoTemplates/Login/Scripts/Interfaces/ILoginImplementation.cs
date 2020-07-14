using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Xsolla.Core;

public interface ILoginImplementation
{
	void GetAttributes([CanBeNull] Action<List<UserAttributeModel>> onSuccess = null,
		[CanBeNull] Action<Error> onError = null);
	
	void SetAttributes(List<UserAttributeModel> attributes, [CanBeNull] Action onSuccess = null,
		[CanBeNull] Action<Error> onError = null);
	
	void RemoveAttributes(List<string> attributes, [CanBeNull] Action onSuccess = null,
		[CanBeNull] Action<Error> onError = null);
}