using System.Collections.Generic;

namespace Xsolla.Core
{
	public class IntentFilterNode : BaseManifestNode
	{
		public IntentFilterNode(string parentTag) : base(AndroidManifestConstants.IntentFilterTag, parentTag)
		{
		}
	}
}