using System;
using System.Collections.Generic;
using UnityEngine;
using Xsolla.Core;
using Xsolla.Login;

public class ExtraController : MonoBehaviour, IExtraPanelController
{
	[SerializeField]
	OldExtraPanelAccountButtons accountButtons;

	[SerializeField]
	ExtraPanelInfoButtons infoButtons;

	[SerializeField]
	AttributesSidePanel attributesSidePanel;

	public event Action LinkingAccountComplete;

	private void Start()
	{
		accountButtons.LinkingAccountComplete += () => LinkingAccountComplete?.Invoke();
		accountButtons.OpenUrlEvent += (string url) => BrowserHelper.Instance.Open(url);
		infoButtons.OpenUrlEvent += (string url) => BrowserHelper.Instance.Open(url);
	}

	public void Initialize()
	{
		accountButtons.Init();
		infoButtons.Init();
	}

	public void SetAttributesVisibility(bool isVisible)
	{
		attributesSidePanel.gameObject.SetActive(isVisible);
	}
}