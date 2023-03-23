#if UNITY_IOS
using System;
using System.Runtime.InteropServices;

namespace Xsolla.Core
{
	public static class IosUtils
	{
		public static T Cast<T>(this IntPtr instancePtr)
		{
			var instanceHandle = GCHandle.FromIntPtr(instancePtr);
			if (instanceHandle.Target is T target)
				return target;

			throw new InvalidCastException($"Failed to cast IntPtr to {typeof(T).FullName}");
		}

		public static IntPtr GetPointer(this object obj)
		{
			return obj == null ? IntPtr.Zero : GCHandle.ToIntPtr(GCHandle.Alloc(obj));
		}
	}
}
#endif