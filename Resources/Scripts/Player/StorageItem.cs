using Godot;
using System.Collections.Generic;

namespace BearBakery;

public abstract partial class StorageItem : Item
{
    public int MaxIngredientCount = 1;
    public List<Ingredient> Ingredients = new List<Ingredient>();

    public bool IsFull()
    {
        return Ingredients.Count >= MaxIngredientCount;
    }

    public abstract void AddIngredient(Ingredient ingredient);

    public HFlowContainer GetIngredientsNode()
    {
        float sizeValue = 21;
        HFlowContainer ingredientContainer = new HFlowContainer()
        {
            CustomMinimumSize = new Vector2(sizeValue, sizeValue),
            Alignment = FlowContainer.AlignmentMode.Center,
            ReverseFill = true
        };
        ingredientContainer.AddThemeConstantOverride("h_separation", 0);
        ingredientContainer.AddThemeConstantOverride("v_separation", 0);
        ingredientContainer.ZIndex = 1;

        foreach (Ingredient ingredient in Ingredients)
        {
            TextureRect ingredientNode = GetIngredientNode(ingredient);
            ingredientContainer.AddChild(ingredientNode);
        }

        return ingredientContainer;
    }

    private TextureRect GetIngredientNode(Ingredient ingredient)
    {
        float sizeValue = 7.5f;
        TextureRect ingredientNode = new TextureRect()
        {
            CustomMinimumSize = new Vector2(sizeValue, sizeValue),
            ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
            StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
            Texture = ingredient.Texture
        };
        return ingredientNode;
    }
}