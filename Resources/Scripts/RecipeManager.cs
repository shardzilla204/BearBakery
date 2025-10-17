using Godot;
using GC = Godot.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BearBakery;

public partial class RecipeManager : Node
{
	private static List<Recipe> _recipes = new List<Recipe>();

	public override void _EnterTree()
	{
		BearBakery.Signals.PlayerSecondaryAction += MixIngredients;

		LoadRecipesFile();
	}

	private void LoadRecipesFile()
	{
		string fileName = "Recipes";
		GC.Dictionary<string, Variant> recipesDictionary = BearBakery.LoadFile(fileName).As<GC.Dictionary<string, Variant>>();
		if (recipesDictionary == null) return;

		GC.Array<GC.Dictionary<string, Variant>> recipeDictionaries = recipesDictionary[fileName].As<GC.Array<GC.Dictionary<string, Variant>>>();
		AddRecipes(recipeDictionaries);
	}

	private void AddRecipes(GC.Array<GC.Dictionary<string, Variant>> recipeDictionaries)
	{
		foreach (GC.Dictionary<string, Variant> recipeDictionary in recipeDictionaries)
		{
			Recipe recipe = new Recipe(recipeDictionary);
			_recipes.Add(recipe);
		}
	}

	public static bool HasRecipe(List<Ingredient> ingredients)
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

	public static Ingredient GetProduct(List<Ingredient> ingredients)
	{
		List<string> ingredientNames = GetIngredientNames(ingredients);

		for (int i = 0; i < _recipes.Count; i++)
		{
			bool hasIngredients = _recipes[i].IngredientNames.All(ingredientNames.Contains);
			bool hasSameCount = _recipes[i].IngredientNames.Count == ingredientNames.Count;

			if (!hasIngredients || !hasSameCount) continue;

			return IngredientManager.GetIngredient(_recipes[i].Name);
		}
		return null;
	}

	public static Ingredient GetAddon(List<Ingredient> ingredients)
	{
		Ingredient product = GetProduct(ingredients);

		foreach (string addonName in product.Addons.Keys)
		{
			Ingredient addon = ingredients.Find(ingredient => ingredient.Name == addonName);
			if (addon != null) return addon;
		}

		return null;
	}

	private static List<string> GetIngredientNames(List<Ingredient> ingredients)
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