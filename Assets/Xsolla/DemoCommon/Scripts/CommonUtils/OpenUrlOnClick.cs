using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class OpenUrlOnClick : MonoBehaviour
	{
		[SerializeField] private SimpleButton Button;
		[SerializeField] private UrlType UrlType;

		private string _url;

		public string URL
		{
			get
			{
				return _url;
			}
			set
			{
				_url = value;
			}
		}

		private void Awake()
		{
			Button.onClick += OpenUrl;
			URL = DemoController.Instance.UrlContainer.GetUrl(UrlType);
		}

		private void OpenUrl()
		{
			BrowserHelper.Instance.Open(URL, true);
		}
	}
}
