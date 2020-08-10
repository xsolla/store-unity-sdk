using System;
using UnityEngine;

public abstract class StoreActionProgress : MonoBehaviour, IStoreActionProgress
{
	public Action OnStarted { get; set; }
	public bool IsInProgress { get; protected set; }
}
