using UnityEngine;

public interface IMenuStateMachine
{
	event MenuStateMachine.StateChangeDelegate StateChangingEvent;
	GameObject SetState(MenuState state);
	void SetPreviousState();
}
