using UnityEngine;

namespace Xsolla.Demo
{
	internal class ParallaxBackground : MonoBehaviour
	{
		[SerializeField] private Vector3 SelfOffset;
		[SerializeField] private MeshRenderer MeshRenderer;
		[SerializeField] private float MaterialParallaxFactor = 0.01f;
		[SerializeField] private Transform FollowTarget;

		private Vector2 TextureOffset;
		private Material MaterialInstance;
		private Vector3 CurrentPosition;

		private void Start()
		{
			FollowTarget = FindAnyObjectByType<ParallaxTarget>().transform;
			SelfOffset = transform.position;
			SelfOffset.y = 0;
			SelfOffset.z = 0;

			MaterialInstance = MeshRenderer.material;
			CurrentPosition = transform.position;
		}

		private void LateUpdate()
		{
			UpdateMaterial();
			UpdatePosition();
		}

		private void UpdateMaterial()
		{
			TextureOffset.x = Mathf.Repeat(FollowTarget.position.x * MaterialParallaxFactor, 1f);
			MaterialInstance.mainTextureOffset = TextureOffset;
		}

		private void UpdatePosition()
		{
			CurrentPosition.x = FollowTarget.position.x;
			transform.position = CurrentPosition + SelfOffset;
		}
	}
}