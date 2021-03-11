using UnityEngine;

namespace Xsolla.Demo
{
	public abstract class BaseBattlePassSelectedItemSubscriber : MonoBehaviour, IBattlePassSelectedItemSubscriber
    {
		public abstract void OnItemSelected(ItemSelectedEventArgs eventArgs);
	}
}
