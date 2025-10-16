using Godot;
using Godot.Collections;

namespace BearBakery;

public enum ToolType
{
	Bowl,
	Tray, 
	Pan,
	Plate,
}

public partial class Tool : Item
{
	public ToolType Type;
}
