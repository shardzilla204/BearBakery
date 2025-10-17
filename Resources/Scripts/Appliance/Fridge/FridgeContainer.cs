using Godot;
using GC = Godot.Collections;
using System;
using System.Collections.Generic;

namespace BearBakery;

public partial class FridgeContainer : Container
{
	public override void _ExitTree()
	{
		BearBakery.Signals.IngredientAddedToFridge -= AddSlot;
		BearBakery.Signals.ItemAddedToInventory -= RefreshSlots;
	}

	public override void _EnterTree()
	{
		BearBakery.Signals.IngredientAddedToFridge += AddSlot;
		BearBakery.Signals.ItemAddedToInventory += RefreshSlots;
	}

	public override void _Ready()
	{
		AddSlots();
	}

	public override bool _CanDropData(Vector2 atPosition, Variant data)
	{
		try
		{
			GC.Dictionary<string, Variant> dragDictionary = data.As<GC.Dictionary<string, Variant>>();
			Item item = dragDictionary["Item"].As<Item>();

			Node origin = dragDictionary["Origin"].As<Node>();
			if (origin is FridgeSlot) return false;

			// Check if item can cast to specified classes
			if (item is not Ingredient ingredient && item is not Food food) return false;

			int maxSlotCount = FridgeManager.MaxSlotCount;
			int ingredientCount = FridgeManager.Items.Count;
			return ingredientCount < maxSlotCount;
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
		GC.Dictionary<string, Variant> dragDictionary = data.As<GC.Dictionary<string, Variant>>();
		Ingredient ingredient = dragDictionary["Item"].As<Ingredient>();
		Node origin = dragDictionary["Origin"].As<Node>();

		BearBakery.Signals.EmitSignal(Signals.SignalName.IngredientAddedToFridge, ingredient);

		Item targetItem = BearBakery.Player.Inventory.Items.Find(item => item == ingredient);
		if (targetItem == null) return;

		BearBakery.Signals.EmitSignal(Signals.SignalName.ItemRemovedFromInventory, targetItem);
	}

	private void AddSlots()
	{
		int index = 0;
		foreach (Item item in FridgeManager.Items)
		{
			FridgeSlot fridgeSlot = BearBakery.PackedScenes.GetFridgeSlot(item);
			fridgeSlot.ID = index;
			AddChild(fridgeSlot);
			index++;
		}
	}

	private void AddSlot(Ingredient ingredient)
	{
		FridgeSlot fridgeSlot = BearBakery.PackedScenes.GetFridgeSlot(ingredient);
		AddChild(fridgeSlot);
	}

	private void RefreshSlots(Item item)
	{
		foreach (Node child in GetChildren())
		{
			child.QueueFree();
		}

		AddSlots();
	}

	public void RefreshItems()
	{
		string refreshingMessage = "Refreshing The Fridge";
		PrintRich.PrintLine(TextColor.Blue, refreshingMessage);

		FridgeManager.Items.Clear();

		foreach (FridgeSlot fridgeSlot in GetChildren())
		{
			Item fridgeSlotItem = fridgeSlot.Item;
			if (fridgeSlotItem is Ingredient fridgeSlotIngredient)
			{
				FridgeManager.Items.Add(fridgeSlotIngredient);
			}
			else if (fridgeSlotItem is Food fridgeSlotFood)
			{
				FridgeManager.Items.Add(fridgeSlotFood);
			}
		}

		PrintRich.PrintFridge();
	}
}
