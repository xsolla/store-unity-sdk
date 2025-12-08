using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Xsolla.Demo
{
	public static class Utils
	{
		public static void ClearChildren(this Transform transform)
		{
			if (!transform)
				throw new ArgumentNullException(nameof(transform));

			for (var i = transform.childCount - 1; i >= 0; i--)
			{
				var child = transform.GetChild(i);
				Object.Destroy(child.gameObject);
			}
		}
	}
}