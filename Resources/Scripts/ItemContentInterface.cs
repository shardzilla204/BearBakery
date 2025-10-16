using Godot;

namespace BearBakery;

public partial class ItemContentInterface : Control
{
    [Export]
    private TextureRect _itemSprite;

    [Export]
    private MarginContainer _marginContainer;

    [Export]
    private Container _content;

    private int _maxHeight = 340;

    public override void _Ready()
    {
        _marginContainer.Visible = false;
    }

    public void SetStorageItem(StorageItem storageItem)
    {
        _itemSprite.Texture = storageItem.Texture;
        _marginContainer.Visible = storageItem.Ingredients.Count != 0;

        if (storageItem.Ingredients.Count != 0) SetContent(storageItem);
    }

    private void RemoveContent()
    {
        foreach (Node child in _content.GetChildren())
        {
            child.QueueFree();
        }
    }

    private void SetContent(StorageItem storageItem)
    {
        int margin = 10;
        int baseSize = 65;
        int sizeIncrease = 55;
        float customMinSizeY = baseSize + margin;
        foreach (Ingredient ingredient in storageItem.Ingredients)
        {
            TextureRect ingredientNode = GetIngredientNode(ingredient);
            _content.AddChild(ingredientNode);
            customMinSizeY += sizeIncrease;
        }
        CustomMinimumSize = new Vector2(baseSize, customMinSizeY);
    }

    private TextureRect GetIngredientNode(Ingredient ingredient)
    {
        float sizeValue = 55;
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
