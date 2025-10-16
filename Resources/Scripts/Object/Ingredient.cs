using Godot;
using GC = Godot.Collections;
using System.Collections.Generic;

namespace BearBakery;

public partial class Ingredient : Item
{
    public override void SetItem(string ingredientName, GC.Dictionary<string, Variant> ingredientDictionary)
    {
        try
        {
            base.SetItem(ingredientName, ingredientDictionary);

            int tier = ingredientDictionary["Tier"].As<int>();
            Tier = tier;

            if (tier == 1) return;

            Addons = ingredientDictionary["Addons"].As<GC.Dictionary<string, Variant>>();
        }
        catch (KeyNotFoundException) { }
    }

    public int Tier = 1;
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