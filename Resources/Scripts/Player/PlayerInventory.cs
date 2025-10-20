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

    public void CycleItems(int itemIndex, int insertIndex)
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
                Bowl bowl = ItemManager.GetItem(itemName) as Bowl;
                
                GC.Array<string> ingredientNames = itemDictionary[itemName].As<GC.Array<string>>();
                foreach (string ingredientName in ingredientNames)
                {
                    Ingredient ingredient = IngredientManager.GetIngredient(ingredientName);
                    bowl.AddIngredient(ingredient);
                }
                Items.Add(bowl);
            }
            else
            {
                Item item = ItemManager.GetItem(itemName);
                Items.Add(item);
            }
        }
    }
}