using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace Xsolla.Login
{
	[Serializable]
	public class GetAttributesJson
	{
		public List<string> keys;
		public int publisher_project_id;
		public string user_id;

		public GetAttributesJson(List<string> attributeKeys, string projectId, string userId)
		{
			keys = attributeKeys != null && attributeKeys.Any() ? attributeKeys : new List<string>();
			publisher_project_id = Convert.ToInt32(projectId);
			user_id = !string.IsNullOrEmpty(userId) ? userId : null;
		}
	}
}