using Godot;

namespace BearBakery;

public partial class SlotComponent : Control
{
	[Export]
	private DisplayComponent _displayComponent;

	public bool IsFilled = false;

	public void SetItem(Item item)
	{
		IsFilled = item != null;
		_displayComponent.SetTexture(item);
	}
}
