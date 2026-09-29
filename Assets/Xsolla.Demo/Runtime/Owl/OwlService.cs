using UnityEngine;

namespace Xsolla.Demo
{
	public class OwlService
	{
		private static IInventoryService InventoryService => ServiceLocator.Resolve<IInventoryService>();
		private static PawnService PawnService => ServiceLocator.Resolve<PawnService>();
		private static GuyService GuyService => ServiceLocator.Resolve<GuyService>();

		private GameObject OwlInstance { get; set; }

		public void SpawnOwl(PawnMode mode)
		{
			if (OwlInstance)
				DeleteInstance();

			CheckOwlAvailability();
		}

		public void CheckOwlAvailability()
		{
			var quantity = InventoryService.GetItem("owl").Quantity;
			UpdateOwlInstance(quantity > 0);
		}

		private void UpdateOwlInstance(bool shouldBe)
		{
			switch (shouldBe)
			{
				case true when !OwlInstance:
				{
					OwlInstance = PawnService.CreateOwl();
					InitOwlInstance();
					break;
				}
				case false when OwlInstance:
					DeleteInstance();
					break;
			}
		}

		private void InitOwlInstance()
		{
			OwlInstance.transform.rotation = Quaternion.LookRotation(Vector3.back);

			var guyInstance = GuyService.GuyInstance;
			var guyMovementState = guyInstance.GetComponent<GuyMovementState>();
			OwlInstance.GetComponent<OwlTargetFollowController>().Construct(guyMovementState);

			OwlInstance.GetComponent<OwlTargetFollowState>().TargetPosition = guyInstance.transform.position;
			OwlInstance.GetComponent<OwlTargetFollowMotor>().AlignToTargetImmediately();
		}

		public void DeleteInstance()
		{
			if (OwlInstance)
				PawnService.DestroyPawn(OwlInstance);

			OwlInstance = null;
		}
	}
}