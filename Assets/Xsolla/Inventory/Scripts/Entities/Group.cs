using System;
using System.Collections.Generic;

namespace Xsolla.Store
{
	[Serializable]
	public class Group
	{
		public int id;
		public string external_id;
		public string name;
		public string description;
		public string image_url;
		public int level;
		public int? order;
		public string parent_group_id;
		public List<Group> children;
		public string parent_external_id;
	}
}