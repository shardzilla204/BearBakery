using Godot;
using GC = Godot.Collections;
using System;
using System.Linq;
using System.Collections.Generic;

namespace BearBakery;

public partial class PlayerInventoryInterface : Control
{
	[Export]
	private GC.Array<SlotComponent> _inventorySlots = new GC.Array<SlotComponent>();

	private bool _isHovering = false;

	public override async void _Ready()
	{
		MouseEntered += () => _isHovering = true;
		MouseExited += () => _isHovering = false;

		InventoryManager.Slots = _inventorySlots.ToList();

		await ToSignal(BearBakery.Signals, Signals.SignalName.PlayersSpawned);
		SetInventory();
	}

	public override void _Process(double delta)
    {
        if (BearBakery.Player.Inventory.Items.Count <= 1 || !_isHovering) return;
        
        int maxIndex = BearBakery.Player.Inventory.Items.Count - 1;
        // * Action { Right Arrow, Mouse Wheel Down }
        if (Input.IsActionJustPressed("CycleClockwise"))
		{
        	BearBakery.Player.Inventory.CycleItems(0, maxIndex);
        }

		// * Action { Left Arrow, Mouse Wheel Up }
		if (Input.IsActionJustPressed("CycleCounterClockwise"))
		{
			BearBakery.Player.Inventory.CycleItems(maxIndex, 0);
		}
    }

    public override void _Notification(int what)
    {
        if (InventoryManager.IsDragging && what == NotificationDragEnd) InventoryManager.IsDragging = false;
    }

	public override Variant _GetDragData(Vector2 atPosition)
	{
		InventoryManager.IsDragging = true;

		Item item = BearBakery.Player.Inventory.Items[0];
		Control dragPreview = GameManager.GetDragPreview(item);

		SetDragPreview(dragPreview);

		GC.Dictionary<string, Variant> dragDictionary = new GC.Dictionary<string, Variant>()
		{
			{ "Origin", this },
			{ "Item", item }
		};

		return dragDictionary;
	}

	public override bool _CanDropData(Vector2 atPosition, Variant data)
	{
		try
		{
			GC.Dictionary<string, Variant> dragDictionary = data.As<GC.Dictionary<string, Variant>>();
			Item item = dragDictionary["Item"].As<Item>();

			Node origin = dragDictionary["Origin"].As<Node>();
			if (origin is PlayerInventoryInterface) return false;

			return HasEmptySlot();
		}
		catch (NullReferenceException)
		{
			return false;
		}
	}

	public override void _DropData(Vector2 atPosition, Variant data)
	{
		GC.Dictionary<string, Variant> dragDictionary = data.As<GC.Dictionary<string, Variant>>();
		Item item = dragDictionary["Item"].As<Item>();

		Node origin = dragDictionary["Origin"].As<Node>();
		if (origin is FridgeSlot fridgeSlot)
		{
			fridgeSlot.SetItem(null);

			Ingredient ingredient = item as Ingredient;
			BearBakery.Signals.EmitSignal(Signals.SignalName.IngredientRemovedFromFridge, ingredient);
			
			PrintRich.PrintFridge();
		}

		SlotComponent emptySlot = GetEmptySlot();
		emptySlot.SetItem(item);

		BearBakery.Signals.EmitSignal(Signals.SignalName.ItemAddedToInventory, item);

	}

	private void SetInventory()
	{
		foreach (Item item in BearBakery.Player.Inventory.Items)
		{
			if (!HasEmptySlot()) return;

			SlotComponent emptySlot = GetEmptySlot();
			emptySlot.SetItem(item);
		}
	}

	private bool HasEmptySlot()
	{
		bool hasEmptySlot = false;
		foreach (SlotComponent inventorySlot in _inventorySlots)
		{
			hasEmptySlot = !inventorySlot.IsFilled;
		}
		return hasEmptySlot;
	}

	private SlotComponent GetEmptySlot()
	{
		List<SlotComponent> inventorySlots = _inventorySlots.ToList();
		return inventorySlots.Find(inventorySlot => !inventorySlot.IsFilled);
	}
}
