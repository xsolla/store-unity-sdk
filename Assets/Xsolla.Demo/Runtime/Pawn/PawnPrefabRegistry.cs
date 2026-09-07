using UnityEngine;

namespace Xsolla.Demo
{
	public class PawnPrefabRegistry : ScriptableObject
	{
		[field: SerializeField] public GameObject Guy { get; private set; }
		[field: SerializeField] public GameObject Fox { get; private set; }
		[field: SerializeField] public GameObject Owl { get; private set; }
	}
}