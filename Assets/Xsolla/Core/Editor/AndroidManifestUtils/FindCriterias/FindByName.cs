using System;
using System.Xml;

namespace Xsolla.Core
{
	public class FindByName : IFindCriteria<XmlNode>
	{
		private readonly BaseManifestNode _node;

		public FindByName(BaseManifestNode node)
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
						var attributeNode = xmlNode.Attributes.GetNamedItem(AndroidManifestConstants.NameAttribute);
						return attributeNode != null
							&& _node.Attributes.ContainsKey(AndroidManifestConstants.NameAttribute)
							&& attributeNode.Value.Equals(_node.Attributes[AndroidManifestConstants.NameAttribute]);
					}
				}
			}

			return false;
		}
	}
}