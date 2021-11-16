using System;
using System.Runtime.InteropServices;

namespace Xsolla.Core
{
	public static class IosUtils
	{
		public static T Cast<T>(this IntPtr instancePtr)
		{
			var instanceHandle = GCHandle.FromIntPtr(instancePtr);
			if (!(instanceHandle.Target is T))
			{
				throw new InvalidCastException("Failed to cast IntPtr");
			}

			var castedTarget = (T) instanceHandle.Target;
			return castedTarget;
		}

		public static IntPtr GetPointer(this object obj)
		{
			return obj == null ? IntPtr.Zero : GCHandle.ToIntPtr(GCHandle.Alloc(obj));
		}
	}
}