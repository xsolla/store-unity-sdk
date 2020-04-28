using System;
using System.Collections.Generic;
using System.Linq;
using Xsolla.Core;

namespace Xsolla.Store
{
    public class UserCatalog : MonoSingleton<UserCatalog>
    {
        public event Action<List<StoreItem>> UpdateItemsEvent;
        public event Action<List<VirtualCurrencyPackage>> UpdateVirtualCurrencyPackagesEvent;
        public event Action<List<VirtualCurrencyItem>> UpdateVirtualCurrenciesEvent;
        public event Action<List<Group>> UpdateGroupsEvent;
        
        private List<StoreItem> _items = new List<StoreItem>();
        private List<VirtualCurrencyItem> _currencies = new List<VirtualCurrencyItem>();
        private List<VirtualCurrencyPackage> _packages = new List<VirtualCurrencyPackage>();
        private List<Group> _groups = new List<Group>();

        public List<VirtualCurrencyItem> GetCurrencies() => _currencies;
        public List<StoreItem> GetItems() => _items;
        public List<VirtualCurrencyPackage> GetPackages() => _packages;
        public List<Group> GetGroups() => _groups;
        
        public bool IsEmpty()
        {
            return !GetItems().Any();
        }
        
        public void UpdateItems(Action<List<StoreItem>> onSuccess = null, Action<Error> onError = null)
        {
            XsollaStore.Instance.GetCatalog(XsollaSettings.StoreProjectId, items =>
            {
                _items = items.items.ToList();
                onSuccess?.Invoke(GetItems());
                UpdateItemsEvent?.Invoke(GetItems());
            }, onError);
        }
        
        public void UpdateVirtualCurrencies(Action<List<VirtualCurrencyItem>> onSuccess = null, Action<Error> onError = null)
        {
            XsollaStore.Instance.GetVirtualCurrencyList(XsollaSettings.StoreProjectId, currencies =>
                {
                    _currencies = currencies.items.ToList();
                    onSuccess?.Invoke(GetCurrencies());
                    UpdateVirtualCurrenciesEvent?.Invoke(GetCurrencies());
                }, onError);
        }

        public void UpdateVirtualCurrencyPackages(Action<List<VirtualCurrencyPackage>> onSuccess = null, Action<Error> onError = null)
        {
            XsollaStore.Instance.GetVirtualCurrencyPackagesList(XsollaSettings.StoreProjectId, packages =>
                {
                    _packages = packages.items;
                    onSuccess?.Invoke(GetPackages());
                    UpdateVirtualCurrencyPackagesEvent?.Invoke(GetPackages());
                }, onError);
        }

        public void UpdateGroups(Action<List<Group>> onSuccess = null, Action<Error> onError = null)
        {
            XsollaStore.Instance.GetItemGroups(XsollaSettings.StoreProjectId, groups =>
            {
                _groups = groups.groups.ToList();
                onSuccess?.Invoke(GetGroups());
                UpdateGroupsEvent?.Invoke(GetGroups());
            }, onError);
        }
    }
}
    