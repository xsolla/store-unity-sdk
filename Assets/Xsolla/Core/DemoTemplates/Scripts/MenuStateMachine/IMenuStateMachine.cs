using System;
using UnityEngine;
using static MenuStateMachine;

public interface IMenuStateMachine
{
	event StateChangeDelegate StateChangingEvent;
	GameObject SetState(MenuState state);
	void SetPreviousState();
}
