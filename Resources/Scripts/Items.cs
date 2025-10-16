using Godot;
using GC = Godot.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BearBakery;

public partial class Items : Node
{
    public List<Item> _items = new List<Item>();
    private GC.Dictionary<string, Variant> _itemsDictionary = new GC.Dictionary<string, Variant>();

    public override void _EnterTree()
    {
        BearBakery.Items = this;

        LoadItemsFile();
        SetItems();
    }

    private void LoadItemsFile()
    {
        string filePath = "res://Resources/JSON/Items.json";

        using FileAccess itemsFile = FileAccess.Open(filePath, FileAccess.ModeFlags.Read);
        string jsonString = itemsFile.GetAsText();

        Json json = new Json();

        if (json.Parse(jsonString) != Error.Ok) return;

        _itemsDictionary = new GC.Dictionary<string, Variant>((GC.Dictionary)json.Data);

        // Print Messages To Console
        string loadSuccessMesssage = $"Items Successfully Loaded";
        if (PrintRich.AreFileMessagesEnabled) PrintRich.PrintLine(TextColor.Green, loadSuccessMesssage);
    }

    private void SetItems()
    {
        List<string> itemNames = _itemsDictionary.Keys.ToList();
        foreach (string itemName in itemNames)
        {
            GC.Dictionary<string, Variant> itemDictionary = _itemsDictionary[itemName].As<GC.Dictionary<string, Variant>>();
            Item item = new Item();
            item.SetItem(itemName, itemDictionary);
            _items.Add(item);
        }
    }

    public Item GetItem(string itemName)
    {
        return _items.Find(item => item.Name == itemName);
    }
}