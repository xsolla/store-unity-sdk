using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupsController : MonoBehaviour
{
    [SerializeField]
    private GameObject _groupPrefab;
    [SerializeField]
    private RectTransform _scrollView;

    public Action<string> OnGroupClick;

    public void AddGroup(Xsolla.XsollaStore.GroupItemInformation groupInformation)
    {
        if (!groupInformation.enabled)
            return;

        GameObject newGroup = Instantiate(_groupPrefab, _scrollView.transform);
        newGroup.GetComponent<GroupUI>().InitializeGroup(groupInformation);
        newGroup.GetComponent<GroupUI>().OnGroupClick += (id) => 
        {
            if (OnGroupClick != null)
                OnGroupClick.Invoke(id);
        };
    }
}
