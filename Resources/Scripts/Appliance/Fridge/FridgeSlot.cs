using Godot;
using Godot.Collections;
using System;

namespace BearBakery;

public partial class FridgeSlot : Control
{
	[Export]
	private SlotComponent _slotComponent;

	[Export]
	private LowlightComponent _lowlightComponent;

	public int ID = -1;
	public Item Item = null;

	private bool _isDragging = false;

	private Label _tooltipLabel;

	public override void _Ready()
	{
		MouseEntered += OnMouseEntered;
		MouseExited += OnMouseExited;
	}

    public override void _Process(double delta)
    {
		if (IsInstanceValid(_tooltipLabel))
        {
			_tooltipLabel.GlobalPosition = GetGlobalMousePosition() - GetOffsetPosition(_tooltipLabel);
        }
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
			Dictionary<string, Variant> dragDictionary = data.As<Dictionary<string, Variant>>();
			Item item = dragDictionary["Item"].As<Item>();

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

		Item targetItem = BearBakery.Player.Inventory.Items.Find(item => item == newItem);
		if (targetItem == null) return;

		BearBakery.Signals.EmitSignal(Signals.SignalName.ItemRemovedFromInventory, targetItem);
		if (origin is PlayerInventoryInterface)
		{
			BearBakery.Signals.EmitSignal(Signals.SignalName.ItemAddedToInventory, oldItem);
		}
	}

	/// <summary>
	/// Show the custom tooltip and darken the self modulate 
	/// Hover without object over while filled, lowlights
	/// </summary>
	private void OnMouseEntered()
	{
		if (Item == null) return;

		_tooltipLabel = BearBakery.PackedScenes.GetTooltipLabel();
		_tooltipLabel.Text = $"{Item.Name}";
		GetParent().GetOwner().GetParent().AddChild(_tooltipLabel);

		bool canDarken = Item != null;
		_lowlightComponent.Set(canDarken);

		BearBakery.ToggleCursorShapeVisibility(true);
	}
	
	/// <summary>
    /// Remove the custom tooltip and lighten the self modulate 
	/// Hover with object over while not filled, lowlights | <see cref="_CanDropData"/>
    /// </summary>
	private void OnMouseExited()
    {
		_tooltipLabel.QueueFree();

		_lowlightComponent.Set(false);

		BearBakery.ToggleCursorShapeVisibility(false);
    }

	public void SetItem(Item item)
	{
		Item = item;
		_slotComponent.SetItem(item);
	}

	/// Only swaps the items visually | <see cref="FridgeContainer.RefreshItems"/>
	private void SwapItems(Item newItem, Item oldItem, FridgeSlot oldFridgeSlot)
	{
		oldFridgeSlot.SetItem(oldItem);
	}

	private Vector2 GetOffsetPosition(Control control)
	{
		float margin = 10f;
		Vector2 offsetPosition = Vector2.Zero;
		Vector2 globalMousePosition = GetGlobalMousePosition();
		Vector2 windowCenter = GetWindow().Size / 2;

		if (globalMousePosition.X > windowCenter.X)
		{
			// Bottom Right
			if (globalMousePosition.Y > windowCenter.Y)
			{
				offsetPosition = new Vector2(control.Size.X - margin, control.Size.Y + margin);
			}
			// Top Right
			else if (globalMousePosition.Y <= windowCenter.Y)
			{
				offsetPosition = new Vector2(control.Size.X - margin, margin);
			}
		}
		else if (globalMousePosition.X <= windowCenter.X)
		{
			// Bottom Left
			if (globalMousePosition.Y > windowCenter.Y)
			{
				offsetPosition = new Vector2(margin, control.Size.Y + margin);
			}
			// Top Left
			else if (globalMousePosition.Y <= windowCenter.Y)
			{
				offsetPosition = -new Vector2(margin, margin);
			}
		}
		return offsetPosition;
    }
}
