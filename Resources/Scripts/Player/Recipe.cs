using Godot;
using GC = Godot.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BearBakery;

public partial class Recipe : Node
{
    public Recipe(string recipeName, GC.Dictionary<string, Variant> recipeDictionary)
    {
        Name = recipeName;
        Methods = recipeDictionary["Methods"].As<GC.Dictionary<string, float>>();

        IngredientNames = recipeDictionary["Ingredients"].As<GC.Array<string>>().ToList();
        // AddonNames = recipeDictionary["Addons"].As<GC.Array<string>>().ToList();

        Container = recipeDictionary["Container"].As<string>();
    }

    public new string Name;
    public GC.Dictionary<string, float> Methods = new GC.Dictionary<string, float>();
    public List<string> IngredientNames = new List<string>();
    public List<string> AddonNames = new List<string>();
    public string Container;
}