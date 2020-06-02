using System.Xml;

namespace Xsolla.Core
{
	public class FindByChildName : IFindCriteria<XmlNode>
	{
		private readonly string _tag;
		private readonly BaseManifestNode _childNode;

		public FindByChildName(string tag, BaseManifestNode childNode)
		{
			_tag = tag;
			_childNode = childNode;
		}

		public bool MatchesCriteria(XmlNode xmlNode)
		{
			if (xmlNode == null)
			{
				return false;
			}

			if (_tag.Equals(xmlNode.Name))
			{
				var childNode = xmlNode.FindNodeRecursive(new FindByName(_childNode));
				return childNode != null;
			}

			return false;
		}
	}
}