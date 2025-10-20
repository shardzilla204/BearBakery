using Godot;
using GC = Godot.Collections;
using System.Collections.Generic;

namespace BearBakery;

public partial class ItemManager : Node
{
    public static List<Item> Items = new List<Item>();
    private GC.Dictionary<string, Variant> _itemsDictionary = new GC.Dictionary<string, Variant>();

    public override void _EnterTree()
    {
        LoadItemsFile();
    }

    private void LoadItemsFile()
    {
        string fileName = "Items";
        GC.Dictionary<string, Variant> itemsDictionary = BearBakery.LoadFile(fileName).As<GC.Dictionary<string, Variant>>();
        if (itemsDictionary == null) return;

        GC.Array<GC.Dictionary<string, Variant>> itemDictionaries = itemsDictionary[fileName].As<GC.Array<GC.Dictionary<string, Variant>>>();

        AddItems(itemDictionaries);
    }

    private void AddItems(GC.Array<GC.Dictionary<string, Variant>> itemDictionaries)
    {
        foreach (GC.Dictionary<string, Variant> itemDictionary in itemDictionaries)
        {
            Item item;
            string itemName = itemDictionary["Name"].As<string>();
            switch (itemName)
            {
                case "Bowl":
                    item = new Bowl(itemDictionary);
                    break;
                
                default:
                    item = new Item(itemDictionary);
                    break;
            }
            Items.Add(item);
        }
    }

    public static Item GetItem(string itemName)
    {
        return Items.Find(item => item.Name == itemName);
    }
}