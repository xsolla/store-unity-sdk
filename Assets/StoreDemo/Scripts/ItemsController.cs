using System.Collections.Generic;
using UnityEngine;

public class ItemsController : MonoBehaviour
{
    [SerializeField]
    private GameObject _itemContainer_Prefab;

    private Dictionary<string, GameObject> _containers = new Dictionary<string, GameObject>();
    
    public void AddItem(Xsolla.StoreItem itemInformation)
    {
        foreach (var group in itemInformation.groups)
        {
            if (!_containers.ContainsKey(group))
            {
                //create container
                GameObject newContainer = Instantiate(_itemContainer_Prefab, transform);
                newContainer.SetActive(false);
                _containers.Add(group, newContainer);
            }
            //add to container
            GameObject groupContainer = _containers[group];
            groupContainer.GetComponent<ItemContainer>().AddItem(itemInformation);
        }
    }
    public void ActivateContainer(string groupId)
    {
        foreach (var container in _containers.Values)
        {
            container.SetActive(false);
        }
        if (_containers.ContainsKey(groupId))
        {
            _containers[groupId].SetActive(true);
        }
    }
}
