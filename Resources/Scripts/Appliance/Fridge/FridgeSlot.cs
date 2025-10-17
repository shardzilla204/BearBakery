using Godot;
using Godot.Collections;
using System;

namespace BearBakery;

public partial class FridgeSlot : Control
{
	[Export]
	private SlotComponent _slotComponent;

	public int ID = -1;
	public Item Item = null;

	private Color _originalColor;
	private bool _isDragging = false;

	public override void _Ready()
	{
		MouseEntered += () => SetLowlight(true);
		MouseExited += () => SetLowlight(false);

		_originalColor = SelfModulate;
	}

	public override void _Notification(int what)
	{
		if (!_isDragging || what != NotificationDragEnd) return;

		_isDragging = false;
	}

	public override Variant _GetDragData(Vector2 atPosition)
	{
		_isDragging = true;

		Control dragPreview = GameManager.GetDragPreview(Item);
		SetDragPreview(dragPreview);

		Dictionary<string, Variant> dragDictionary = new Dictionary<string, Variant>()
		{
			{ "Origin", this },
			{ "Item", Item }
		};

		return dragDictionary;
	}

	public override bool _CanDropData(Vector2 atPosition, Variant data)
	{
		try
		{
			float amount = 0.1f;
			Dictionary<string, Variant> dragDictionary = data.As<Dictionary<string, Variant>>();
			Item item = dragDictionary["Item"].As<Item>();

			SelfModulate = item != null ? _originalColor.Darkened(amount) : _originalColor;

			/// Check if item can cast to specified classes | <see cref="Ingredient"> && <see cref="Food">
			return item is Ingredient || item is Food;
		}
		catch (NullReferenceException)
		{
			Item item = data.As<Item>();
			string errorMessage = $"{item.Name} Is Not An Ingredient";
			PrintRich.Print(TextColor.Red, errorMessage);

			return false;
		}
	}

	public override void _DropData(Vector2 atPosition, Variant data)
	{
		Dictionary<string, Variant> dragDictionary = data.As<Dictionary<string, Variant>>();
		Item newItem = dragDictionary["Item"].As<Item>();
		Node origin = dragDictionary["Origin"].As<Node>();

		Item oldItem = Item;

		if (origin is FridgeSlot oldFridgeSlot)
		{
			SwapItems(newItem, oldItem, oldFridgeSlot);
		}
		else if (origin is PlayerInventoryInterface)
		{
			GD.Print($"Old Item: {oldItem.Name}");
			GD.Print($"New Item: {newItem.Name}");
			GD.Print("Origin is Player Inventory Interface");
		}
		SetItem(newItem);

		FridgeContainer fridgeContainer = GetParentOrNull<FridgeContainer>();
		fridgeContainer.RefreshItems();

		// Item targetItem = BearBakery.Player.Inventory.Items.Find(item => item == newItem);
		// if (targetItem == null) return;

		// BearBakery.Signals.EmitSignal(Signals.SignalName.ItemRemovedFromInventory, targetItem);
		// if (origin is PlayerInventoryInterface)
		// {
		// 	BearBakery.Signals.EmitSignal(Signals.SignalName.ItemAddedToInventory, oldItem);
		// }
	}

	public void SetItem(Item item)
	{
		Item = item;
		_slotComponent.SetItem(item);
	}

	private void SetLowlight(bool isHovering)
	{
		// Hover without object over while filled, highlights
		/// Hover with object over while not filled, highlights | <see cref="_CanDropData"/>

		float amount = 0.1f;
		SelfModulate = isHovering && Item != null ? _originalColor.Darkened(amount) : _originalColor;
	}

	/// Only swaps the items visually | <see cref="FridgeContainer.RefreshItems"/>
	private void SwapItems(Item newItem, Item oldItem, FridgeSlot oldFridgeSlot)
	{
		oldFridgeSlot.SetItem(oldItem);
	}
}
