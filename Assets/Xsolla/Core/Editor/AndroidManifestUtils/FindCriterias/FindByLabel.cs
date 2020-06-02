using System;
using System.Xml;

namespace Xsolla.Core
{
	public class FindByLabel : IFindCriteria<XmlNode>
	{
		private readonly BaseManifestNode _node;

		public FindByLabel(BaseManifestNode node)
		{
			_node = node;
		}

		public bool MatchesCriteria(XmlNode xmlNode)
		{
			if (xmlNode != null)
			{
				if (xmlNode.Name.Equals(_node.Tag, StringComparison.InvariantCultureIgnoreCase))
				{
					if (xmlNode.Attributes != null)
					{
						var attributeNode = xmlNode.Attributes.GetNamedItem(AndroidManifestConstants.LabelAttribute);
						return attributeNode != null
							&& _node.Attributes.ContainsKey(AndroidManifestConstants.LabelAttribute)
							&& attributeNode.Value.Equals(_node.Attributes[AndroidManifestConstants.LabelAttribute]);
					}
				}
			}

			return false;
		}
	}
}