using UnityEngine;
using Xsolla.Core;

namespace Xsolla.ReadyToUseStore
{
	public class Config
	{
		public Config()
		{
			IsCheckEventSystemExists = true;
		}

		public Transform Parent { get; set; }

		public bool IsWorldSpace { get; set; }

		public bool IsCheckEventSystemExists { get; set; }

		public string Locale { get; set; }

		public TokenData TokenData { get; set; }
	}
}