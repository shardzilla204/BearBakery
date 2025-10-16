using Godot;

namespace BearBakery;

public partial class Plate : StorageItem
{
    public override void AddIngredient(Ingredient ingredient)
    {
        Ingredients.Add(ingredient);

        string addedIngredientMessage = $"Added {ingredient.Name} To Plate";
        PrintRich.Print(TextColor.Yellow, addedIngredientMessage);
    }
}