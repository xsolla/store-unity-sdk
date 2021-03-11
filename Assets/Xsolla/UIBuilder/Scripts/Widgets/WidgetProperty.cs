using System;
using UnityEngine;

namespace Xsolla.UIBuilder
{
	[Serializable]
	public class WidgetProperty
	{
		[SerializeField] private string _name;

		[SerializeField] private string _id;

		[SerializeField] private GameObject _prefab;

		public string Id
		{
			get => _id;
			set => _id = value;
		}

		public string Name
		{
			get => _name;
			set => _name = value;
		}

		public GameObject Prefab
		{
			get => _prefab;
			set => _prefab = value;
		}

		public WidgetProperty()
		{
			_id = Guid.NewGuid().ToString();
		}

		protected bool Equals(WidgetProperty other)
		{
			return _id == other._id;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;

			if (ReferenceEquals(this, obj))
				return true;

			if (obj.GetType() != GetType())
				return false;

			return Equals((WidgetProperty) obj);
		}

		public override int GetHashCode()
		{
			return _id != null ? _id.GetHashCode() : 0;
		}
	}
}