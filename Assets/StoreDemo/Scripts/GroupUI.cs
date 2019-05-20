using System;
using UnityEngine;
using UnityEngine.UI;

public class GroupUI : MonoBehaviour
{
    [SerializeField]
    private Text _groupName_Text;
    [SerializeField]
    private Button _group_Btn;

    public Action<string> OnGroupClick;
    private string group_Name;
    private void Awake()
    {
        _group_Btn.onClick.AddListener(() => 
        {
            if (OnGroupClick != null)
                OnGroupClick.Invoke(group_Name);
        });
    }
    public void InitializeGroup(string name)
    {
        group_Name = name;
        _groupName_Text.text = name;
    }
}
