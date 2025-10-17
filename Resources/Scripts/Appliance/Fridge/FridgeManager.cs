using Godot;
using GC = Godot.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace BearBakery;

public partial class FridgeManager : Node
{
	[Export]
	private GC.Array<string> _startingIngredients = new GC.Array<string>()
	{
		"Milk",
		"Butter",
		"Egg",
		"Flour",
		"Sugar"
	};

	private GC.Array<GC.Dictionary<string, Variant>> _startingFood = new GC.Array<GC.Dictionary<string, Variant>>();

	[Export]
	private int _maxSlotCount = 25;

	public static List<Item> Items = new List<Item>();
	public static int MaxSlotCount = 25;

	public override void _EnterTree()
	{
		BearBakery.Signals.FridgeOpened += AddFridgeWindow;
		BearBakery.Signals.IngredientAddedToFridge += AddIngredient;
		BearBakery.Signals.IngredientRemovedFromFridge += RemoveIngredient;

		_startingFood = new GC.Array<GC.Dictionary<string, Variant>>()
		{
			new GC.Dictionary<string, Variant>(){{ "Cookie", new GC.Array<string>(){ "Plain", "White Chocolate Chips" } }},
			new GC.Dictionary<string, Variant>(){{ "Cheese Cake", new GC.Array<string>(){ "Plain", "Strawberry Sauce" } }},
			new GC.Dictionary<string, Variant>(){{ "Cookie", new GC.Array<string>(){ "Chocolate", "White Chocolate Chips" } }},
		};

		MaxSlotCount = _maxSlotCount;
	}

	public override void _Ready()
	{
		AddStartingIngredients();
		AddStartingFood();
	}

	private void AddFridgeWindow(FridgeArea fridgeArea)
	{
		FridgeWindow fridgeWindow = BearBakery.PackedScenes.GetFridgeWindow(fridgeArea);
		BearBakery.Signals.EmitSignal(Signals.SignalName.WindowOpened, fridgeWindow);
	}

	private void AddStartingIngredients()
	{
		List<string> startingIngredients = _startingIngredients.ToList();
		foreach (string startingIngredient in startingIngredients)
		{
			Ingredient ingredient = IngredientManager.GetIngredient(startingIngredient);
			Items.Add(ingredient);
		}
	}

	private void AddStartingFood()
	{
		foreach (GC.Dictionary<string, Variant> foodDictionary in _startingFood)
		{
			string foodName = foodDictionary.Keys.ToList().First();

			string foodOptionName = foodDictionary[foodName].As<GC.Array<string>>()[0];
			foodOptionName = foodOptionName.Replace(" ", "");
			FoodOption foodOption = Enum.Parse<FoodOption>(foodOptionName);

			string foodAddonName = foodDictionary[foodName].As<GC.Array<string>>()[1];
			foodAddonName = foodAddonName.Replace(" ", "");
			FoodAddon foodAddon = Enum.Parse<FoodAddon>(foodAddonName);

			Food food = BearBakery.GetFood(foodName, foodOption, foodAddon);
			Items.Add(food);
		}
	}

	private void AddIngredient(Item item)
	{
		if (item is Ingredient ingredient && ingredient != null)
		{
			Items.Add(ingredient);
		}
		else if (item is Food food && food != null)
		{ 
			Items.Add(food);
		}
	}

	private void RemoveIngredient(Ingredient ingredient)
	{
		Items.Remove(ingredient);
	}
}
