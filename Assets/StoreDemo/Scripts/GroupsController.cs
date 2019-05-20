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

    public void AddGroup(string groupName)
    {
        GameObject newGroup = Instantiate(_groupPrefab, _scrollView.transform);
        newGroup.GetComponent<GroupUI>().InitializeGroup(groupName);
        newGroup.GetComponent<GroupUI>().OnGroupClick += (id) => 
        {
            if (OnGroupClick != null)
                OnGroupClick.Invoke(id);
        };
    }
}
