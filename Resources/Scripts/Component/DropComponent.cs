using Godot;
using Godot.Collections;

namespace BearBakery;

public partial class DropComponent : Control
{
	[Export(PropertyHint.Range, "100,250,5")]
	private float _dragPreviewSize = 100;

	public Control GetDragPreview(Item item)
	{
		Vector2 dragPreviewSize = new Vector2(_dragPreviewSize, _dragPreviewSize);
		TextureRect textureRect = new TextureRect()
		{
			Texture = item.Texture,
			Size = dragPreviewSize,
			Position = -dragPreviewSize / 2,
			ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
			StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered
		};
		Control dragPreview = new Control();
		dragPreview.AddChild(textureRect);

		return dragPreview;
	}
}
