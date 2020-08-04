using System.Collections.Generic;

namespace Xsolla.Core
{
	public class CategoryNode : BaseManifestNode
	{
		public CategoryNode(string name) : base(
			AndroidManifestConstants.CategoryTag, AndroidManifestConstants.IntentFilterTag, 
			new Dictionary<string, string> {{AndroidManifestConstants.NameAttribute, name}})
		{
		}
	}
}