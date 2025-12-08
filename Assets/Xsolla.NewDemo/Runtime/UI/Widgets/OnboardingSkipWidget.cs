using System.Collections;
using UnityEngine;

namespace Xsolla.Demo
{
	public class OnboardingSkipWidget : MonoBehaviour
	{
		[field: SerializeField] private GameObject ViewRoot;
		[field: SerializeField] private float TimeToShow = 3f;

		private IEnumerator Start()
		{
			ViewRoot.SetActive(false);
			yield return new WaitForSeconds(TimeToShow);
			ViewRoot.SetActive(true);
		}
	}
}