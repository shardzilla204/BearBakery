using Godot;

namespace BearBakery;

public partial class DisplayComponent : TextureRect
{
	public void SetTexture(Item item)
	{
		Texture = item == null ? null : item.Texture;
	}
}
