using Godot;
using GC = Godot.Collections;
using System.Collections.Generic;

namespace BearBakery;

public partial class IngredientManager : Node
{
    private static List<Ingredient> _ingredients = new List<Ingredient>();

    public override void _EnterTree()
    {
        LoadIngredientsFile();
    }

    private void LoadIngredientsFile()
    {
        string fileName = "Ingredients";
        GC.Dictionary<string, Variant> ingredientsDictionary = BearBakery.LoadFile(fileName).As<GC.Dictionary<string, Variant>>();
        if (ingredientsDictionary == null) return;

        GC.Array<GC.Dictionary<string, Variant>> ingredientDictionaries = ingredientsDictionary[fileName].As<GC.Array<GC.Dictionary<string, Variant>>>();
        SetIngredients(ingredientDictionaries);
    }

    private void SetIngredients(GC.Array<GC.Dictionary<string, Variant>> ingredientDictionaries)
    {
        foreach (GC.Dictionary<string, Variant> ingredientDictionary in ingredientDictionaries)
        {
            Ingredient ingredient = new Ingredient(ingredientDictionary);
            _ingredients.Add(ingredient);
        }
    }

    public static Ingredient GetIngredient(string ingredientName)
    {
        return _ingredients.Find(ingredient => ingredient.Name == ingredientName);
    }
}