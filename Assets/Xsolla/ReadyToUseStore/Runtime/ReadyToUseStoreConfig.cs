using UnityEngine;
using Xsolla.Core;

namespace Xsolla.ReadyToUseStore
{
	public class ReadyToUseStoreConfig
	{
		public string Locale { get; set; }

		public TokenData TokenData { get; set; }

		public Transform CatalogParent { get; set; }

		public bool IsDontDestroyOnLoad { get; set; }

		public bool IsCheckEventSystemExists { get; set; } = true;
	}
}