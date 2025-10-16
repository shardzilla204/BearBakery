using Godot;
using GC = Godot.Collections;
using System.Collections.Generic;

namespace BearBakery;

public partial class Item : Node
{
	public new string Name;
	public string Description;
	public Texture2D Texture;

	public virtual void SetItem(string itemName, GC.Dictionary<string, Variant> itemDictionary)
	{
		try
		{
			Name = itemName;
			Description = itemDictionary["Description"].As<string>();

			string textureFilePath = itemDictionary["Texture"].As<string>();
			Texture = BearBakery.GetTexture(textureFilePath);

		}
		catch (KeyNotFoundException) { }
	}

	public void SetItem(Item item)
	{
		Name = item.Name;
		Description = item.Description;
		Texture = item.Texture;
	}
}
