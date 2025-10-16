using Godot;
using System.Collections.Generic;

namespace BearBakery;

public partial class OvenRack : HBoxContainer
{
	public List<OvenSlot> Slots = new List<OvenSlot>();

	public override void _Ready()
	{
		foreach (OvenSlot ovenSlot in GetChildren())
		{
			Slots.Add(ovenSlot);
		}
	}
}
