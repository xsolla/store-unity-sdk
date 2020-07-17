using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Xsolla.Core;
using Xsolla.Core.Popup;
using Xsolla.Store;

public class UserCart : MonoSingleton<UserCart>
{
    public event Action<UserCartItem> AddItemEvent;
    public event Action<UserCartItem> RemoveItemEvent;
    public event Action<UserCartItem, int> UpdateItemEvent;
    public event Action PurchaseCartEvent;
    public event Action ClearCartEvent;
    
    private Cart _cartEntity;
    private UserCartModel _cart;

    public override void Init()
    {
        base.Init();
        
        _cart = new UserCartModel();
        XsollaStore.Instance.CreateNewCart(XsollaSettings.StoreProjectId, newCart =>
        {
            _cartEntity = newCart;
        }, ShowError);
    }

    public void AddItem(CatalogItemModel item)
    {
        UserCartItem cartItem = _cart.AddItem(item);
        if (cartItem != null)
        {
            AddItemEvent?.Invoke(cartItem);
        }
    }

    public void RemoveItem(CatalogItemModel item)
    {
        UserCartItem cartItem = _cart.GetItem(item);
        if(!_cart.RemoveItem(item)) return;
        RemoveItemEvent?.Invoke(cartItem);
    }

    public void IncreaseCountOf(CatalogItemModel item)
    {
        UserCartItem cartItem = _cart.IncreaseCountOf(item);
        if (cartItem != null)
        {
            UpdateItemEvent?.Invoke(cartItem, 1);
        }
    }
    
    public void DecreaseCountOf(CatalogItemModel item)
    {
        UserCartItem cartItem = _cart.DecreaseCountOf(item);
        if (cartItem == null) return;
        if (cartItem.Quantity == 0)
        {
            RemoveItemEvent?.Invoke(cartItem);
        }
        else
        {
            UpdateItemEvent?.Invoke(cartItem, -1);
        }
    }

    public List<UserCartItem> GetCartItems()
    {
        return _cart.GetCartItems();
    }
    
    public void Clear()
    {
        _cart.Clear();
        ClearCartEvent?.Invoke();
    }

    public bool IsEmpty()
    {
        return _cart.IsEmpty();
    }

    public float CalculateRealPrice()
    {
        if (IsEmpty())
        {
            return 0.0F;
        }
        return CalculateFullPrice() - CalculateCartDiscount();
    }
    
    public float CalculateFullPrice()
    {
        return GetCartItems().Sum(i => i.TotalPrice);
    }

    public float CalculateCartDiscount()
    {
        return GetCartItems().Sum(i => i.TotalDiscount);
    }
    
    public void Purchase(Action onSuccess = null, Action<Error> onError = null)
    {
        if (!GetCartItems().Any()) return;
        XsollaStore.Instance.ClearCart(XsollaSettings.StoreProjectId, _cartEntity.cart_id, () =>
        {
            List<UserCartItem> items = GetCartItems();
            if (!items.Any()) return;
            StartCoroutine(CartFillingCoroutine(items, onSuccess, onError));
        }, onError);
    }

    IEnumerator CartFillingCoroutine(List<UserCartItem> items, Action onSuccess = null, Action<Error> onError = null)
    {
        int addItemRequestsLeft = items.Count();
        items.ForEach(i =>
        {
            XsollaStore.Instance.UpdateItemInCart(
                XsollaSettings.StoreProjectId, _cartEntity.cart_id, i.Sku, i.Quantity, () => {
                    addItemRequestsLeft--;
                }, error => {
                    addItemRequestsLeft--;
                    onError?.Invoke(error);
                });
        });
        yield return new WaitWhile(() => addItemRequestsLeft > 0);
        PurchaseCart(onSuccess, onError);
    }
    
    void PurchaseCart(Action onSuccess = null, Action<Error> onError = null)
    {
        XsollaStore.Instance.CartPurchase(XsollaSettings.StoreProjectId, _cartEntity.cart_id, data =>
        {
            XsollaStore.Instance.OpenPurchaseUi(data);

            XsollaStore.Instance.ProcessOrder(XsollaSettings.StoreProjectId, data.order_id, () =>
            {
                onSuccess?.Invoke();
                PurchaseCartEvent?.Invoke();
            }, onError);
            
            Clear();
        }, onError);
    }
    
    private void ShowError(Error error)
    {
        Debug.LogError(error);
        PopupFactory.Instance.CreateError().SetMessage(error.ToString());
    }
}
