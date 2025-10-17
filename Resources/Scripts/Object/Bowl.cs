using Godot;

namespace BearBakery;

public partial class Bowl : StorageItem
{
    public Bowl()
    {
        Item bowl = ItemManager.GetItem("Bowl");

        Name = bowl.Name;
		Description = bowl.Description;

		string textureFileName = Name.Replace(" ", "");
		Texture2D texture = BearBakery.GetTexture(textureFileName, "Items");
        Texture = texture;
        
        MaxIngredientCount = 6;
    }

    public float MixTime = 3;

    public override void AddIngredient(Ingredient ingredient)
    {
        Ingredients.Add(ingredient);

        string addedIngredientMessage = $"Added {ingredient.Name} To Bowl";
        PrintRich.Print(TextColor.Yellow, addedIngredientMessage);
    }

    public void MixIngredients()
    {
        Ingredient product = RecipeManager.GetProduct(Ingredients);
        Ingredient addon = RecipeManager.GetAddon(Ingredients);

        if (addon != null)
        {
            product.SetTexture(addon);
        }

        Ingredients.Clear();
        AddIngredient(product);
    }

    private void AddAddons()
    {
        
    }
}

/* Mind Map

    - Dough = Solid ingredient
    - Batter = Liquid ingredient

    - Ingredient = Liquid, Solid
    - Food = Product of cooking recipe
    - Recipe = Combination of mixtures and ingredients
    - Liquid = Is consumable (Yes = Is not an ingredient | No = Is an ingredient)

    Cup
    - Hold liquid that's consumable

    Baking Tray
    - Hold ingredient
    - Hold food after cooking recipe

    Baking Pan 
    - Hold ingredient
    - Hold food after cooking recipe

    Bowl
    - Hold multiple ingredients
    - Hold ingredient product after mixing
    
    Plate 
    - Hold food
*/