using System;
using Newtonsoft.Json;

namespace Xsolla.Core
{
	[Serializable]
	public class PayStationUI
	{
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string size;
		
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string theme;
		
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string version;
		
		public bool is_independent_windows;
	}
}
