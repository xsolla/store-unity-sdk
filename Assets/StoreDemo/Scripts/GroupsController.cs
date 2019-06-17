using System;
using UnityEngine;

public class GroupsController : MonoBehaviour
{
    [SerializeField]
    private GameObject _groupPrefab;
    [SerializeField]
    private GameObject _groupCartPrefab;
    
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

    public void AddCart()
    {
	    GameObject newGroup = Instantiate(_groupCartPrefab, _scrollView.transform);
	    newGroup.GetComponent<CartGroupUI>().onGroupClick += (id) => 
	    {
		    if (OnGroupClick != null)
			    OnGroupClick.Invoke(id);
	    };
    }
}
