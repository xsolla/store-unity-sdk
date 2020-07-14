using System;
using System.Collections.Generic;
using System.Linq;
using Xsolla.Store;

public class UserCartModel
{
    private readonly List<UserCartItem> _items;

    public UserCartModel()
    {
        _items = new List<UserCartItem>();
    }

    private Predicate<UserCartItem> SearchPredicate(StoreItem item) => cartItem => cartItem.Sku.Equals(item.sku);

    private UserCartItem FindCartItemBy(StoreItem storeItem) => GetCartItems().Find(SearchPredicate(storeItem));
    
    public UserCartItem GetItem(StoreItem item)
    {
        return FindCartItemBy(item);
    }
    
    private bool Exist(StoreItem item)
    {
        return _items.Exists(SearchPredicate(item));
    }

    public UserCartItem AddItem(StoreItem item)
    {
        if (!Exist(item))
        {
            _items.Add(new UserCartItem(item));
        }
        return GetItem(item);
    }
    
    public bool RemoveItem(StoreItem item)
    {
        if(!Exist(item)) return false;
        UserCartItem cartItem = GetItem(item);
        _items.Remove(cartItem);
        return true;
    }

    public UserCartItem IncreaseCountOf(StoreItem item)
    {
        if (!Exist(item)) return null;
        UserCartItem cartItem = GetItem(item);
        cartItem.Quantity++;
        return cartItem;
    }
    
    public UserCartItem DecreaseCountOf(StoreItem item)
    {
        if (!Exist(item)) return null;
        
        UserCartItem cartItem = GetItem(item);
        cartItem.Quantity--;
        if (cartItem.Quantity == 0)
        {
            RemoveItem(cartItem.Item);
        }
        return cartItem;
    }

    public List<UserCartItem> GetCartItems()
    {
        return _items;
    }
    
    public List<StoreItem> GetItems()
    {
        return GetCartItems().Select(cartItem => cartItem.Item.DeepClone()).ToList();
    }

    public void Clear()
    {
        _items.Clear();
    }

    public bool IsEmpty()
    {
        return GetCartItems().Count == 0;
    }
}
