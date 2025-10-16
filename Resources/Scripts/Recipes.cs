using Godot;
using GC = Godot.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BearBakery;

public partial class Recipes : Node
{
	private GC.Dictionary<string, Variant> _recipesDictionary = new GC.Dictionary<string, Variant>();
	private List<Recipe> _recipes = new List<Recipe>();

	public override void _EnterTree()
	{
		BearBakery.Recipes = this;

		BearBakery.Signals.PlayerSecondaryAction += MixIngredients;

		LoadRecipesFile();
		SetRecipes();
	}

	private void LoadRecipesFile()
	{
		string filePath = "res://Resources/JSON/Recipes.json";

		using FileAccess itemsFile = FileAccess.Open(filePath, FileAccess.ModeFlags.Read);
		string jsonString = itemsFile.GetAsText();

		Json json = new Json();

		if (json.Parse(jsonString) != Error.Ok) return;

		_recipesDictionary = new GC.Dictionary<string, Variant>((GC.Dictionary) json.Data);

		// Print Messages To Console
		string loadSuccessMesssage = $"Recipes Successfully Loaded";
		if (PrintRich.AreFileMessagesEnabled) PrintRich.PrintLine(TextColor.Green, loadSuccessMesssage);
	}

	private void SetRecipes()
	{
		foreach (string recipeName in _recipesDictionary.Keys)
		{
			GC.Dictionary<string, Variant> recipeDictionary = _recipesDictionary[recipeName].As<GC.Dictionary<string, Variant>>();
			Recipe recipe = new Recipe(recipeName, recipeDictionary);
			_recipes.Add(recipe);
		}
	}

	public bool HasRecipe(List<Ingredient> ingredients)
	{
		List<Ingredient> ingredientsClone = new List<Ingredient>();
		ingredientsClone.AddRange(ingredients);

		List<string> ingredientNames = GetIngredientNames(ingredients);
		foreach (Recipe recipe in _recipes)
		{
			bool hasIngredients = recipe.IngredientNames.All(ingredientNames.Contains);
			bool hasSameCount = recipe.IngredientNames.Count == ingredientNames.Count;

			if (hasIngredients && hasSameCount) return true;
		}

		return false;
	}

	public Ingredient GetProduct(List<Ingredient> ingredients)
	{
		List<string> ingredientNames = GetIngredientNames(ingredients);

		for (int i = 0; i < _recipes.Count; i++)
		{
			bool hasIngredients = _recipes[i].IngredientNames.All(ingredientNames.Contains);
			bool hasSameCount = _recipes[i].IngredientNames.Count == ingredientNames.Count;

			if (!hasIngredients || !hasSameCount) continue;

			return BearBakery.Ingredients.GetIngredient(_recipes[i].Name);
		}
		return null;
	}

	public Ingredient GetAddon(List<Ingredient> ingredients)
	{
		Ingredient product = GetProduct(ingredients);

		foreach (string addonName in product.Addons.Keys)
		{
			Ingredient addon = ingredients.Find(ingredient => ingredient.Name == addonName);
			if (addon != null) return addon;
		}

		return null;
	}

	private List<string> GetIngredientNames(List<Ingredient> ingredients)
	{
		List<string> ingredientNames = new List<string>();
		foreach (Ingredient ingredient in ingredients)
		{
			ingredientNames.Add(ingredient.Name);
		}
		return ingredientNames;
	}

	private void MixIngredients()
	{
		Item item = BearBakery.Player.Inventory.Items[0];
		if (item is not Bowl bowl || HasRecipe(bowl.Ingredients)) return;		
	}
}