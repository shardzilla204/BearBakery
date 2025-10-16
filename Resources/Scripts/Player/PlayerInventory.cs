using Godot;
using GC = Godot.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BearBakery;

public partial class PlayerInventory : Node2D
{
    private GC.Array<GC.Dictionary<string, Variant>> _startingItems = new GC.Array<GC.Dictionary<string, Variant>>();

    public List<Item> Items = new List<Item>();

    public override void _ExitTree()
    {
        BearBakery.Signals.ItemAddedToInventory -= AddItem;
        BearBakery.Signals.ItemRemovedFromInventory -= RemoveItem;
    }

    public override void _EnterTree()
    {
        BearBakery.Signals.ItemAddedToInventory += AddItem;
        BearBakery.Signals.ItemRemovedFromInventory += RemoveItem;

        _startingItems = new GC.Array<GC.Dictionary<string, Variant>>()
        {
            // new GC.Dictionary<string, Variant>(){{ "Bowl", new GC.Array<string>(){ "Milk", "Butter", "Egg", "Flour", "Sugar", "Chocolate Chips" } }}
        };
    }

    public override void _Ready()
    {
        AddStartingItems();
    }

    public override void _Process(double delta)
    {
        int maxIndex = Items.Count - 1;
        // * Action { Right Arrow, Mouse Wheel Down }
        if (Input.IsActionJustPressed("CycleClockwise")) CycleItems(0, maxIndex);

        // * Action { Left Arrow, Mouse Wheel Up }
        if (Input.IsActionJustPressed("CycleCounterClockwise")) CycleItems(maxIndex, 0);
    }

    private void CycleItems(int itemIndex, int insertIndex)
    {
        if (Items.Count == 0) return;

        for (int i = 0; i < Items.Count - 1; i++)
        {
            Item item = Items[itemIndex];
            Items.RemoveAt(itemIndex);
            Items.Insert(insertIndex, item);
        }

        BearBakery.Signals.EmitSignal(Signals.SignalName.PlayerItemUpdated);
    }

    private void AddItem(Item itemToAdd)
    {
        if (itemToAdd == null) return;

        Items.Add(itemToAdd);
        BearBakery.Signals.EmitSignal(Signals.SignalName.PlayerItemUpdated);
    }

    private void RemoveItem(Item itemToRemove)
    {
        Items.Remove(itemToRemove);
        BearBakery.Signals.EmitSignal(Signals.SignalName.PlayerItemUpdated);
    }

    private void AddStartingItems()
    {
        foreach (GC.Dictionary<string, Variant> itemDictionary in _startingItems)
        {
            string itemName = itemDictionary.Keys.ToList().First();
            if (itemName == "Bowl")
            {
                GC.Array<string> ingredientNames = itemDictionary[itemName].As<GC.Array<string>>();
                Bowl bowl = GetBowl(ingredientNames);
                Items.Add(bowl);
            }
            else
            {
                Item item = BearBakery.Items.GetItem(itemName);
                Items.Add(item);
            }
        }
    }

    private Bowl GetBowl(GC.Array<string> ingredientNames)
    {
        Item bowlData = BearBakery.Items.GetItem("Bowl");
        Bowl bowl = new Bowl();
        bowl.SetItem(bowlData);

        foreach (string ingredientName in ingredientNames)
        {
            Ingredient ingredient = BearBakery.Ingredients.GetIngredient(ingredientName);
            bowl.AddIngredient(ingredient);
        }

        return bowl;
    }
}