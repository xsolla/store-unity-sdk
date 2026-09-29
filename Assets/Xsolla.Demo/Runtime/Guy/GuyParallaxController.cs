using UnityEngine;

namespace Xsolla.Demo
{
	public class GuyParallaxController : MonoBehaviour
	{
		[SerializeField] private Transform Root;
		private Transform Target;

		private void Start()
		{
			Target = FindAnyObjectByType<ParallaxTarget>().transform;
		}

		private void Update()
		{
			if (Target)
			{
				Target.position = Root.position;
			}
		}
	}
}