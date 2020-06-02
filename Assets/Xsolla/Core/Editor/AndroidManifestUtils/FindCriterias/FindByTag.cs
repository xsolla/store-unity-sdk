using System.Xml;

namespace Xsolla.Core
{
	public class FindByTag : IFindCriteria<XmlNode>
	{
		private readonly string _tag;

		public FindByTag(string tag)
		{
			_tag = tag;
		}

		public bool MatchesCriteria(XmlNode xmlNode)
		{
			if (xmlNode == null)
			{
				return false;
			}

			return _tag.Equals(xmlNode.Name);
		}
	}
}