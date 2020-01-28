using UnityEngine;
using Xsolla.Core;
using Xsolla.PayStation;

public class PayStationController : MonoBehaviour
{
	[SerializeField]
	SimpleTextButton openPaystationButton;
	
	// Start is called before the first frame update
	void Awake()
	{
		openPaystationButton.onClick = XsollaPayStation.Instance.OpenPayStation;
	}
}