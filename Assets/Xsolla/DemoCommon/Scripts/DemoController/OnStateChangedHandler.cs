using UnityEngine;

namespace Xsolla.Demo
{
	public abstract class OnStateChangedHandler : MonoBehaviour
	{
		[SerializeField] protected MenuStateMachine StateMachine;

		private void Awake()
		{
			StateMachine.StateChangedEvent += OnStateChanged;
		}

		protected abstract void OnStateChanged(MenuState lastState, MenuState newState);
	}
}
