using UnityEngine;
using UnityEngine.UI;

public class CartMenuButtonCounter : MonoBehaviour
{
    [SerializeField] private CartMenuButton button;
    [SerializeField] private Text counterText;

    private int Counter
    {
        get => int.Parse(counterText.text);
        set => counterText.text = value.ToString();
    }

    private void Awake()
    {
        Counter = 0;
    }
    
    void Start()
    {
        UserCart.Instance.AddItemEvent += _ => Counter++;
        UserCart.Instance.RemoveItemEvent += _ => Counter--;
        UserCart.Instance.UpdateItemEvent += (_, deltaValue) => Counter += deltaValue;
        UserCart.Instance.ClearCartEvent += () => Counter = 0;
        button.onClick = () => DemoController.Instance.SetState(MenuState.Cart);
    }
}
