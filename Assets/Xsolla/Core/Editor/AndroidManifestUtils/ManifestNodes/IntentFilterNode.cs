using System.Collections.Generic;

namespace Xsolla.Core.Editor
{
	public class IntentFilterNode : BaseManifestNode
	{
		public IntentFilterNode(string parentTag) : base(AndroidManifestConstants.IntentFilterTag, parentTag)
		{
		}
	}
}
