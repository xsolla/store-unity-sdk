using UnityEngine;

namespace Xsolla.Demo
{
	public class StorePageCommonButtonsController : BaseMenuController
	{
		[SerializeField] private SimpleButton publisherAccountButton;
		[SerializeField] private SimpleButton documentationButton;
		[SerializeField] private SimpleButton testCardsButton;
		[SerializeField] private SimpleButton backButton;
		[SerializeField] private SimpleButton tutorialButton;

		private void Start()
		{
			AttachUrlToButton(publisherAccountButton, DemoController.Instance.UrlContainer.GetUrl(UrlType.PublisherUrl));
			AttachUrlToButton(documentationButton, DemoController.Instance.UrlContainer.GetUrl(UrlType.DocumentationUrl));
			AttachUrlToButton(testCardsButton, DemoController.Instance.UrlContainer.GetUrl(UrlType.TestCardsUrl));
			AttachButtonCallback(backButton, () => DemoController.Instance.SetPreviousState());

			if (DemoController.Instance.IsTutorialAvailable && !DemoController.Instance.IsAccessTokenAuth)
			{
				tutorialButton.gameObject.SetActive(true);
				AttachButtonCallback(tutorialButton, () => DemoController.Instance.ShowTutorial(false));
			}
		}
	}
}
