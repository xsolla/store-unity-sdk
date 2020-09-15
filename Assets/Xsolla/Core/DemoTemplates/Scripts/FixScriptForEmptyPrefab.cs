using UnityEngine;

public class FixScriptForEmptyPrefab : MonoBehaviour
{
    private void Start()
    {
        Destroy(this);
    }
}
