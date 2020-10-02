using System;
using System.Collections.Generic;
using UnityEngine;

public class FriendActionsButton : MonoBehaviour
{
    [SerializeField] private GameObject actionPrefab;
    [SerializeField] private Transform actionContainer;
    
    private readonly List<GameObject> _actions = new List<GameObject>();

    private void OnDestroy()
    {
        ClearActions();
    }

    private void Start()
    {
        if (actionContainer != null)
        {
            var btn = gameObject.GetComponent<SimpleButton>();
            if (btn != null && actionContainer != null)
            {
                btn.onClick = () => actionContainer.gameObject.SetActive(!actionContainer.gameObject.activeInHierarchy);
            }
            actionContainer.gameObject.SetActive(false);
        }
    }

    public void AddAction(string actionName, Action callback)
    {
        if (actionPrefab != null && actionContainer != null)
        {
            var go = Instantiate(actionPrefab, actionContainer);
            var button = go.GetComponent<SimpleTextButton>();
            button.Text = actionName;
            button.onClick = () =>
            {
                actionContainer.gameObject.SetActive(false);
                callback?.Invoke();
            };
            _actions.Add(go);
        }
        else
        {
            Debug.LogError("In FriendActionsButton script `actionPrefab` or `actionContainer` = null!");
        }
    }

    public void ClearActions()
    {
        _actions.ForEach(Destroy);
        _actions.Clear();
    }
}
