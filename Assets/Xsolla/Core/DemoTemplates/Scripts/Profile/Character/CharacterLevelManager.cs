using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;
using Xsolla.Login;

public class CharacterLevelManager : MonoBehaviour
{
#pragma warning disable 0649
	[SerializeField] private string CurrencySkuOrName;
	[SerializeField] private bool TryUseDefaultCurrencyOnFailure;
	[SerializeField] private uint LevelUpPrice;
	[SerializeField] private VirtualCurrencyBalanceUI VirtualCurrencyPrefab;
	[SerializeField] private Transform PriceTagPlacement;
	[SerializeField] private SimpleButton LevelUpButton;
	[SerializeField] private Text LevelText;
#pragma warning restore 0649

	private VirtualCurrencyModel _targetCurrency;
	private string _characterLevelEntry;
	private int _characterLevel;

	private void Awake()
	{
		LevelUpButton.onClick += TryUpTheLevel;
	}

	private void Start()
	{
		var token = DemoController.Instance.GetImplementation().Token;

		Action<UserInfo> successCallback = info =>
		{
			var levelEntryHead = info.email ?? info.username ?? info.nickname ?? info.last_name;
			_characterLevelEntry = string.Format("{0}:character_level", levelEntryHead);
			InitializeUI();
		};

		Action<Error> errorCallback = error =>
		{
			Debug.LogError("Could not get info for character level save/load");
			InitializeUI();
		};

		DemoController.Instance.GetImplementation().GetUserInfo(token, successCallback, errorCallback);
	}

	private void InitializeUI()
	{
		_targetCurrency = GetTargetCurrency();

		if(_targetCurrency == null)
		{
			Debug.Log("Could not obtain target currency, disabling VC consumption UI");
			DisableUI();
			return;
		}

		_characterLevel = LoadLevel(_characterLevelEntry);
		ShowLevel(_characterLevel);
		DrawPrice(_targetCurrency, LevelUpPrice);
	}

	private void DisableUI()
	{
		LevelUpButton.gameObject.SetActive(false);
		LevelText.gameObject.SetActive(false);
		LevelUpButton.onClick -= TryUpTheLevel;
	}

	private void TryUpTheLevel()
	{
		var userCurrency = (UserInventory.Instance.Balance != null) ? UserInventory.Instance.Balance.Find(currency => _targetCurrency.Sku == currency.Sku) : null;

		if (userCurrency == null)
		{
			Debug.Log("UserInventory does not contain required currency");
			return;
		}

		if (userCurrency.Amount >= LevelUpPrice)
		{
			var levelUpPayment = new InventoryItemModel()
			{
				Name = _targetCurrency.Name,
				Sku = _targetCurrency.Sku,
			};

			Action onSuccessConsume = () =>
			{
				_characterLevel++;
				SaveLevel(_characterLevelEntry, _characterLevel);
				ShowLevel(_characterLevel);

				if (UserInventory.IsExist)
					UserInventory.Instance.Refresh();
			};

			DemoController.Instance.GetImplementation().ConsumeVirtualCurrency(
				levelUpPayment,
				LevelUpPrice,
				onSuccess: onSuccessConsume,
				onFailed: () =>
				{
					Debug.Log("Could not consume virtual currency");
				});
		}
		else
		{
			var errorMessage = "Not enough currency amount to up the level";
			Debug.Log(errorMessage);
			StoreDemoPopup.ShowError(new Error(errorMessage: errorMessage));
		}
	}

	private VirtualCurrencyModel GetTargetCurrency()
	{
		var allCurrencies = UserCatalog.Instance.VirtualCurrencies;
		if (allCurrencies == null)
		{
			Debug.LogWarning("UserCatalog returned null as VirtualCurrencies");
			return null;
		}

		VirtualCurrencyModel targetCurrency = null;

		if (!string.IsNullOrEmpty(CurrencySkuOrName))
		{
			targetCurrency = allCurrencies.Find(currency => { return CurrencySkuOrName == currency.Sku || CurrencySkuOrName == currency.Name; });

			if (/*still*/targetCurrency == null)
			{
				Debug.LogWarning(string.Format("Could not find specified virtual currency: {0}", CurrencySkuOrName));

				if (TryUseDefaultCurrencyOnFailure)
				{
					Debug.Log("Will try to get default project's currency");
					targetCurrency = allCurrencies.Count > 0 ? allCurrencies[0] : null;
				}
			}
		}
		else
			targetCurrency = allCurrencies.Count > 0 ? allCurrencies[0] : null;

		if (targetCurrency == null)
			Debug.LogWarning("Could not find virtual currency");

		return targetCurrency;
	}

	private void DrawPrice(VirtualCurrencyModel priceCurrency, uint price)
	{
		var currencyObject = Instantiate(VirtualCurrencyPrefab.gameObject, PriceTagPlacement);
		var currencyUI = currencyObject.GetComponent<VirtualCurrencyBalanceUI>();
		currencyUI.Initialize(priceCurrency);
		currencyUI.SetBalance(price);
	}

	private int LoadLevel(string levelEntry)
	{
		if (!string.IsNullOrEmpty(levelEntry))
			return PlayerPrefs.GetInt(levelEntry, 1);
		else
		{
			Debug.LogError("Could not load level. Level entry is null or empty");
			return 1;
		}
	}

	private void SaveLevel(string levelEntry, int characterLevel)
	{
		if (!string.IsNullOrEmpty(levelEntry))
			PlayerPrefs.SetInt(levelEntry, characterLevel);
		else
			Debug.LogError("Could not save level. Level entry is null or empty");
	}

	private void ShowLevel(int characterLevel)
	{
		LevelText.text = string.Format("{0} LVL", _characterLevel);
	}

#if UNITY_EDITOR
	[ContextMenu("Drop the level")]
	private void DropTheLevel()
	{
		if (EditorApplication.isPlaying && !string.IsNullOrEmpty(_characterLevelEntry))
		{
			_characterLevel = 1;
			SaveLevel(_characterLevelEntry, _characterLevel);
			ShowLevel(_characterLevel);
		}
	}
#endif
}
