using System.Collections;

namespace Xsolla.Demo
{
	public interface IGameState
	{
	}

	public interface IGameStateEnter
	{
		void OnEnter();
	}

	public interface IGameStateEnterAsync
	{
		IEnumerator OnEnter();
	}
}