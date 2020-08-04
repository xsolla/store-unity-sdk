using System.Collections.Generic;

namespace Xsolla.Core
{
	public class DataNode : BaseManifestNode
	{
		public DataNode(string scheme, string host, string pathPrefix) : base(
			AndroidManifestConstants.DataTag, AndroidManifestConstants.IntentFilterTag,
			new Dictionary<string, string>
			{
				{"android:scheme", scheme},
				{"android:host", host},
				{"android:pathPrefix", pathPrefix}
			})
		{
		}
	}
}