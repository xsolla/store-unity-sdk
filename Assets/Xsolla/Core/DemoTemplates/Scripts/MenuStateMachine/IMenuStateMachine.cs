using UnityEngine;
using static Xsolla.Demo.MenuStateMachine;

namespace Xsolla.Demo
{
	public interface IMenuStateMachine
	{
		event StateChangeDelegate StateChangingEvent;
		GameObject SetState(MenuState state);
		void SetPreviousState();
	}
}
