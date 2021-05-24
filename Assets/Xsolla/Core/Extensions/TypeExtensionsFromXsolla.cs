using System;

public static class TypeExtensionsFromXsolla
{
	public static bool IsDerivedFrom(this Type derivedType, Type baseType)
	{
		if (derivedType == null || baseType == null) return false;
		do { 
			derivedType = derivedType.BaseType;
			if (derivedType == baseType)
				return true;
		} while (derivedType != null);
		return false;
	}
}
