using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    [SerializeField]
    private Image _item_Image;
    [SerializeField]
    private Text _item_Name;
    [SerializeField]
    private Text _item_Description;
    [SerializeField]
    private Text _item_Price;
    [SerializeField]
    private GameObject _item_Advertisement;
    [SerializeField]
    private Text _item_Advertisement_Text;

    private Coroutine _loading_Routine;
    private Xsolla.XsollaStore.ItemInformation _itemInformation;
    
    public void Initialize(Xsolla.XsollaStore.ItemInformation itemInformation)
    {
        _itemInformation = itemInformation;

        float price = (float)(typeof(Xsolla.XsollaStore.StoreItemPrices)).GetField(Xsolla.XsollaStore.Instance.CurrencyCode).GetValue(itemInformation.prices);
        _item_Price.text = Xsolla.XsollaStore.Instance.CurrencyCode + " "+ price.ToString()+" "+ Xsolla.XsollaStore.Instance.CurrencySymbol;

        _item_Name.text = _itemInformation.name.en;
        _item_Description.text = _itemInformation.description.en;
        if (itemInformation.advertisement_type != "")
            _item_Advertisement_Text.text = itemInformation.advertisement_type;
        else
            _item_Advertisement.SetActive(false);
    }
    private void OnEnable()
    {
        if (_loading_Routine == null && _item_Image.sprite == null)
            _loading_Routine = StartCoroutine(LoadImage(_itemInformation.image_url));
    }
    private IEnumerator LoadImage(string url)
    {
        using (WWW www = new WWW(url))
        {
            yield return www;
            _item_Image.sprite = Sprite.Create(www.texture, new Rect(0,0, www.texture.width, www.texture.height), new Vector2(0.5f, 0.5f));
        }
    }
}
