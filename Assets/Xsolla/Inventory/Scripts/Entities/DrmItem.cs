using System;

namespace Xsolla.Store
{
	[Serializable]
	public class DrmItem
	{
		public string sku;
		public string name;
		public string image;
		public string link;
		public string redeem_instruction_link;
		public int drm_id;
	}
}