using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Xsolla.Core;

public interface OldILoginImplementation
{
	void GetAttributes([CanBeNull] Action<List<OldUserAttributeModel>> onSuccess = null,
		[CanBeNull] Action<Error> onError = null);
	
	void SetAttributes(List<OldUserAttributeModel> attributes, [CanBeNull] Action onSuccess = null,
		[CanBeNull] Action<Error> onError = null);
	
	void RemoveAttributes(List<string> attributes, [CanBeNull] Action onSuccess = null,
		[CanBeNull] Action<Error> onError = null);
}