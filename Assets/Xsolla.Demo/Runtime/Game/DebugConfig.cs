using UnityEngine;

namespace Xsolla.Demo
{
	public class DebugConfig : ScriptableObject
	{
		[field: SerializeField] public bool IsEraseSavedAuthData { get; private set; }

		[field: Space]
		[field: SerializeField] public bool IsSkipOnboardingControlsState { get; private set; }
		[field: SerializeField] public bool IsSkipOnboardingDocumentationState { get; private set; }
		[field: SerializeField] public bool IsSkipOnboardingGameplayState { get; private set; }

		[field: Space]
		[field: SerializeField] public bool UseFakeUserCredentials { get; private set; }
		[field: SerializeField] public string FakeUserName { get; private set; }
		[field: SerializeField] public string FakeUserEmail { get; private set; }
		[field: SerializeField] public string FakeUserPassword { get; private set; }

		[field: Space]
		[field: SerializeField] public bool UseFakeInventoryAndStore { get; private set; }
		[field: SerializeField] public InventoryStorage FakeInventoryStorage { get; private set; }
	}
}