using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Demo
{
	public class DocumentationWidget : MonoBehaviour
	{
		[field: SerializeField] private Button DocumentationButton { get; set; }
		[field: SerializeField] private Button ShopButton { get; set; }

		private UrlService UrlService => ServiceLocator.Resolve<UrlService>();
		private LevelStateMachine LevelStateMachine => ServiceLocator.Resolve<LevelStateMachine>();

		private void Start()
		{
			DocumentationButton.onClick.AddListener(() => UrlService.OpenDocumentation());
			ShopButton.onClick.AddListener(() => LevelStateMachine.SetStoreLevel());
		}
	}
}