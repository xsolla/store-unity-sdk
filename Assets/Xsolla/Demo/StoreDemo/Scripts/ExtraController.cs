using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExtraController : MonoBehaviour
{
	[SerializeField]
	GameObject signOutButton;

	[SerializeField]
	GameObject attributesPanel;

	AttributesSidePanel attributesSidePanel;

	private void Awake()
	{
		attributesSidePanel = attributesPanel.GetComponent<AttributesSidePanel>();
	}

	public void Init()
	{
		signOutButton.SetActive(true);

		var btnComponent = signOutButton.GetComponent<SimpleTextButton>();
		btnComponent.onClick = () => { SceneManager.LoadScene("Login"); };
	}

	public void RefreshAttributesPanel()
	{
		attributesSidePanel.Refresh();
	}

	public void ShowAttributesPanel(bool bShow)
	{
		attributesPanel.SetActive(bShow);
	}
}