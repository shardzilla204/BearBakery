using Godot;
using GC = Godot.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BearBakery;

public partial class Ingredients : Node
{
    public List<Ingredient> _ingredients = new List<Ingredient>();
    private GC.Dictionary<string, Variant> _ingredientsDictionary = new GC.Dictionary<string, Variant>();

    public override void _EnterTree()
    {
        BearBakery.Ingredients = this;
        
        LoadIngredientsFile();
        SetIngredients();
    }

    private void LoadIngredientsFile()
    {
        string filePath = "res://Resources/JSON/Ingredients.json";

        using FileAccess itemsFile = FileAccess.Open(filePath, FileAccess.ModeFlags.Read);
        string jsonString = itemsFile.GetAsText();

        Json json = new Json();

        if (json.Parse(jsonString) != Error.Ok) return;

        _ingredientsDictionary = new GC.Dictionary<string, Variant>((GC.Dictionary) json.Data);

        // Print Messages To Console
        string loadSuccessMesssage = $"Ingredients Successfully Loaded";
        if (PrintRich.AreFileMessagesEnabled) PrintRich.PrintLine(TextColor.Green, loadSuccessMesssage);
    }

    private void SetIngredients()
    {
        List<string> ingredientNames = _ingredientsDictionary.Keys.ToList();
        foreach (string ingredientName in ingredientNames)
        {
            GC.Dictionary<string, Variant> ingredientDictionary = _ingredientsDictionary[ingredientName].As<GC.Dictionary<string, Variant>>();
            Ingredient ingredient = new Ingredient();
            ingredient.SetItem(ingredientName, ingredientDictionary);
            _ingredients.Add(ingredient);
        }
    }

    public Ingredient GetIngredient(string ingredientName)
    {
        return _ingredients.Find(ingredient => ingredient.Name == ingredientName);
    }
}