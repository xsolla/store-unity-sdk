using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CloseApplicationScript : MonoBehaviour
{
    [SerializeField] private SimpleButton exitButton;
    
    void Start()
    {
        if (exitButton != null)
            exitButton.onClick = () =>
            {
                Application.Quit();
#if UNITY_EDITOR
	            EditorApplication.isPlaying = false;
#endif
            };
    }
}
