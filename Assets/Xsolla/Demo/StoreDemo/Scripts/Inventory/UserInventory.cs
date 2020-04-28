using System;
using System.Collections.Generic;
using System.Linq;
using Xsolla.Core;

namespace Xsolla.Store
{
    public class UserInventory : MonoSingleton<UserInventory>
    {
        public event Action<List<InventoryItem>> UpdateItemsEvent;
        public event Action<List<VirtualCurrencyBalance>> UpdateVirtualCurrencyBalanceEvent;
        
        private const string VIRTUAL_CURRENCY_ITEM_TYPE = "virtual_currency";
        private List<InventoryItem> _items = new List<InventoryItem>();
        private List<VirtualCurrencyBalance> _balances = new List<VirtualCurrencyBalance>();
        
        public List<InventoryItem> GetItems() => _items;
        public List<VirtualCurrencyBalance> GetVirtualCurrencyBalance() => _balances;

        public bool IsEmpty()
        {
            return !GetItems().Any();
        }

        public void UpdateVirtualCurrencyBalance(Action<List<VirtualCurrencyBalance>> onSuccess = null, Action<Error> onError = null)
        {
            XsollaStore.Instance.GetVirtualCurrencyBalance(XsollaSettings.StoreProjectId, balances =>
                {
                    _balances = balances.items.ToList();
                    onSuccess?.Invoke(GetVirtualCurrencyBalance());
                    UpdateVirtualCurrencyBalanceEvent?.Invoke(GetVirtualCurrencyBalance());
                }, onError);
        }
        
        public void UpdateVirtualItems(Action<List<InventoryItem>> onSuccess = null, Action<Error> onError = null)
        {
            XsollaStore.Instance.GetInventoryItems(XsollaSettings.StoreProjectId, items =>
                {
                    RefreshCallback(items);
                    onSuccess?.Invoke(GetItems());
                    UpdateItemsEvent?.Invoke(GetItems());
                },
                onError);
        }

        private void RefreshCallback(InventoryItems items)
        {
            _items = FilterVirtualCurrency(items.items);
        }
        
        private List<InventoryItem> FilterVirtualCurrency(InventoryItem[] items)
        {
            return items.ToList().
                Where(i => i.type != VIRTUAL_CURRENCY_ITEM_TYPE).ToList();
        }
    }    
}
