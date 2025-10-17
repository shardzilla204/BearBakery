using Godot;
using GC = Godot.Collections;

namespace BearBakery;

public partial class Ingredient : Item
{
    public Ingredient() { }

    public Ingredient(GC.Dictionary<string, Variant> ingredientDictionary)
    {
        Name = ingredientDictionary["Name"].As<string>(); ;
        Description = ingredientDictionary["Description"].As<string>();

        string textureFileName = Name.Replace(" ", "");
        Texture2D texture = BearBakery.GetTexture(textureFileName, "Items");
        Texture = texture;

        bool hasAddonsKey = ingredientDictionary.ContainsKey("Addons");
        if (hasAddonsKey)
        {
            Addons = ingredientDictionary["Addons"].As<GC.Dictionary<string, Variant>>();
        }
    }

    public GC.Dictionary<string, Variant> Addons = new GC.Dictionary<string, Variant>();

    public void SetTexture(Ingredient addon)
    {
        Image baseImage = Texture.GetImage();
        Image addonImage = GetImage(addon);

        Vector2I des = baseImage.GetSize() / 2 - addonImage.GetSize() / 2;
        Rect2I srcRect = new Rect2I()
        {
            Size = addonImage.GetSize()
        };
        baseImage.BlendRect(addonImage, srcRect, des);
        ImageTexture imageTexture = ImageTexture.CreateFromImage(baseImage);
        Texture = imageTexture;
    }

    private Image GetImage(Ingredient addon)
    {
        string addonFilePath = Addons[addon.Name].As<string>();
        Image baseImage = BearBakery.GetTexture(addonFilePath).GetImage();
       
        return baseImage;
    }
}