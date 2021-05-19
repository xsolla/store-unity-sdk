using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CartMenuButtonCounter : MonoBehaviour
{
    [SerializeField] private CartMenuButton button;
    [SerializeField] private Text counterText;

    private int Counter
    {
		get
		{
			return int.Parse(counterText.text);
		}
		set
		{
			counterText.text = value.ToString();
		}
    }

    private void Start()
    {
        Counter = UserCart.Instance.GetItems().Sum(i => i.Quantity);
        UserCart.Instance.AddItemEvent += IncreaseCounter;
        UserCart.Instance.RemoveItemEvent += DecreaseCounter;
        UserCart.Instance.UpdateItemEvent += UpdateCounter;
        UserCart.Instance.ClearCartEvent += ResetCounter;
        button.onClick = () => DemoController.Instance.SetState(MenuState.Cart);
    }

    private void OnDestroy()
    {
        if (!UserCart.IsExist) return;
        UserCart.Instance.AddItemEvent -= IncreaseCounter;
        UserCart.Instance.RemoveItemEvent -= DecreaseCounter;
        UserCart.Instance.UpdateItemEvent -= UpdateCounter;
        UserCart.Instance.ClearCartEvent -= ResetCounter;
    }

    private void IncreaseCounter(UserCartItem item)
    {
        Counter++;
    }
    
    private void DecreaseCounter(UserCartItem item)
    {
        Counter--;
    }

    private void UpdateCounter(UserCartItem item, int delta)
    {
        Counter += delta;
    }

    private void ResetCounter()
    {
        Counter = 0;
    }
}