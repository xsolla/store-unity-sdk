using System;
using System.Collections.Generic;
using System.Linq;
using Xsolla.Core;
using Xsolla.Store;

public class UserCart : MonoSingleton<UserCart>
{
    public event Action<StoreItem> AddItemEvent;
    public event Action<StoreItem> RemoveItemEvent;
    public event Action<StoreItem> UpdateItemEvent;
    public event Action ClearCartEvent;
    
    private List<StoreItem> _items;

    public override void Init()
    {
        base.Init();
        _items = new List<StoreItem>();
    }

    private bool Exist(StoreItem item)
    {
        return _items.Exists(storeItem => storeItem.sku.Equals(item.sku));
    }

    public void AddItem(StoreItem item)
    {
        if (Exist(item)) return;
        _items.Add(item);
        AddItemEvent?.Invoke(_items.Last());
    }

    public void RemoveItem(StoreItem item)
    {
        if(!Exist(item)) return;
        List<StoreItem> removingItems = _items.Where(storeItem => storeItem.sku.Equals(item.sku)).ToList();
        removingItems.ForEach(storeItem =>
        {
            _items.Remove(storeItem);
            RemoveItemEvent?.Invoke(storeItem);
        });
    }

    public void IncreaseCountOf(StoreItem item)
    {
        if (Exist(item))
        {
            
        }
    }
    
    public void DecreaseCountOf(StoreItem item)
    {
        
    }

    public List<StoreItem> GetItems()
    {
        return _items;
    }

    public void Clear()
    {
        _items.Clear();
        ClearCartEvent?.Invoke();
    }

    public bool IsEmpty()
    {
        return _items.Count == 0;
    }
}
