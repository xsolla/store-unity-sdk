using System;
using static MenuStateMachine;

public interface IMenuStateMachine
{
	event StateChangeDelegate StateChangingEvent;
	void SetState(MenuState state);
	void SetPreviousState();
}
