using Godot;
using GC = Godot.Collections;

namespace BearBakery;

public partial class Item : Node
{
	public Item() { }

	public Item(GC.Dictionary<string, Variant> itemDictionary)
	{
		Name = itemDictionary["Name"].As<string>(); ;
		Description = itemDictionary["Description"].As<string>();

		string textureFileName = Name.Replace(" ", "");
		Texture2D texture = BearBakery.GetTexture(textureFileName, "Items");
		Texture = texture;
	}
	
	public new string Name;
	public string Description;
	public Texture2D Texture;
}
