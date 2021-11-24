using System;
using UnityEngine;

namespace Xsolla.Demo
{
	public class BlurSelectedUser : MonoBehaviour
	{
		public static event Action OnActivated;
		public static event Action OnDeactivated;

		[SerializeField] private GameObject blurMaskObject;
		[SerializeField] private ItemSelection blurTrigger;

		private void Start()
		{
			if (blurTrigger == null || blurMaskObject == null) return;
			blurTrigger.OnPointerEnterEvent += () =>
			{
				blurMaskObject.SetActive(true);
				if (OnActivated != null)
					OnActivated.Invoke();
			};
			blurTrigger.OnPointerExitEvent += () =>
			{
				blurMaskObject.SetActive(false);
				if (OnDeactivated != null)
					OnDeactivated.Invoke();
			};
		}
	}
}
