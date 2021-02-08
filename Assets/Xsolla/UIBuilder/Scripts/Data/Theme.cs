using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Xsolla.UIBuilder
{
	[Serializable]
	public class Theme
	{
		[SerializeField] private string _name;

		[SerializeField] private string _id;

		[SerializeField] private List<ColorProperty> _colors;

		[SerializeField] private List<SpriteProperty> _sprites;

		public string Name
		{
			get => _name;
			set => _name = value;
		}

		public string Id
		{
			get => _id;
			set => _id = value;
		}

		public List<ColorProperty> Colors
		{
			get => _colors;
			set => _colors = value;
		}

		public List<SpriteProperty> Sprites
		{
			get => _sprites;
			set => _sprites = value;
		}

		public ColorProperty GetColorProperty(string id)
		{
			return Colors.FirstOrDefault(x => x.Id == id);
		}

		public SpriteProperty GetSpriteProperty(string id)
		{
			return Sprites.FirstOrDefault(x => x.Id == id);
		}

		public Theme()
		{
			_id = Guid.NewGuid().ToString();
			_name = "New Theme";

			_colors = new List<ColorProperty>();
			_sprites = new List<SpriteProperty>();
		}

		public Theme(Theme source)
		{
			_id = Guid.NewGuid().ToString();
			_name = $"{source._name} (0)";

			_colors = new List<ColorProperty>();
			foreach (var colorProp in source.Colors)
			{
				var prop = new ColorProperty
				{
					Id = colorProp.Id,
					Name = colorProp.Name,
					Color = colorProp.Color
				};

				_colors.Add(prop);
			}

			_sprites = new List<SpriteProperty>();
			foreach (var spriteProp in source.Sprites)
			{
				var prop = new SpriteProperty
				{
					Id = spriteProp.Id,
					Name = spriteProp.Name,
					Sprite = spriteProp.Sprite
				};

				_sprites.Add(prop);
			}
		}
	}
}