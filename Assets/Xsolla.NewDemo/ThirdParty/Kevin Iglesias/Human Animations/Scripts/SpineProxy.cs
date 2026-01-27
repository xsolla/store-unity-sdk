// -- SPINE PROXY 1.0 | Kevin Iglesias --
// This script ensures correct animation display when mixing upper and lower body animations using Unity Avatar Masks.
// Attach this script to the 'B-spineProxy' transform, which is a sibling of the 'B-hips' bone.
// In the 'originalSpine' field, assign the 'B-spine' bone (child of 'B-hips' and parent of 'B-chest').
// By default it will automatically find the 'B-spine' and assign it to the 'originalSpine' field (OnValidate).
// When using a different character rig, manually assign the corresponding spine bone to the 'originalSpine' field and recreate
// 'Rig > B-root > B-spine' structure in your character hierarchy with empty GameObjects.

// More information: https://www.keviniglesias.com/spine-proxy.html
// Contact Support: support@keviniglesias.com

using UnityEngine;

namespace KevinIglesias
{
	public class SpineProxy : MonoBehaviour
	{
		//Assign 'B-spine' (or equivalent) here:
		[SerializeField] private Transform originalSpine;

		private Quaternion rotationOffset = Quaternion.identity;

#if UNITY_EDITOR

		//Attempting to find the original spine bone.
		private void OnValidate()
		{
			if (originalSpine == null)
			{
				var parent = transform.parent;
				if (parent != null)
				{
					var hips = parent.Find("B-hips");
					if (hips != null)
					{
						var spine = hips.Find("B-spine");
						if (spine != null)
						{
							originalSpine = spine;
						}
					}
				}
			}
		}
#endif

		//Match correct orientation on different character rigs
		private void Awake()
		{
			if (originalSpine != null)
			{
				//originalSpine.rotation must be the default rotation in your character T-pose when this happens:
				rotationOffset = Quaternion.Inverse(transform.rotation) * originalSpine.rotation;
			}
		}

		//Copy rotations from spine proxy bone to the original spine bone.
		private void LateUpdate()
		{
			if (originalSpine == null)
			{
				return;
			}
			originalSpine.rotation = transform.rotation * rotationOffset;
		}
	}
}