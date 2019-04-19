using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GroupUI : MonoBehaviour
{
    [SerializeField]
    private Text _groupName_Text;
    [SerializeField]
    private Button _group_Btn;

    private Xsolla.XsollaStore.GroupItemInformation _itemInformation;

    public Action<string> OnGroupClick;

    private void Awake()
    {
        _group_Btn.onClick.AddListener(() => 
        {
            if (OnGroupClick != null)
                OnGroupClick.Invoke(_itemInformation.id);
        });
    }
    public void InitializeGroup(Xsolla.XsollaStore.GroupItemInformation information)
    {
        _itemInformation = information;
        _groupName_Text.text = _itemInformation.name.en;
    }
}
