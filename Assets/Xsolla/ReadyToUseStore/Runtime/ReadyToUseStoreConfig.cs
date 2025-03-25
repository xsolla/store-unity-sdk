using UnityEngine;
using Xsolla.Core;

namespace Xsolla.ReadyToUseStore
{
	public class ReadyToUseStoreConfig
	{
		public ReadyToUseStoreConfig()
		{
			PrefabPath = "Xsolla/ReadyToUseStoreDirector";
		}

		public string PrefabPath { get; set; }

		public bool IsDontDestroyOnLoad { get; set; }

		public Transform Parent { get; set; }

		public bool IsWorldSpace { get; set; }

		public string Locale { get; set; }

		public TokenData TokenData { get; set; }
	}
}