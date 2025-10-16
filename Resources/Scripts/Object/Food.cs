using Godot;
using GC = Godot.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Text.RegularExpressions;

namespace BearBakery;

public enum FoodOption
{
    Plain,
    Chocolate,
    Matcha
}

public enum FoodAddon
{
    None,
    ChocolateChips,
    WhiteChocolateChips,
    BlueberrySauce,
    StrawberrySauce,
    BlackberrySauce
}

public partial class Food : Item
{
    public FoodOption Option = FoodOption.Plain;
    public FoodAddon Addon = FoodAddon.None;

    public bool IsBurnt = false;

    private GC.Dictionary<string, Variant> _dictionary;
    
    public override void SetItem(string foodName, GC.Dictionary<string, Variant> foodDictionary)
    {
        try
        {
            Name = foodName;
            Description = foodDictionary["Description"].As<string>();

            _dictionary = foodDictionary; // Save dictionary for setting the Option, Addon and Texture variables
        }
        catch (KeyNotFoundException) { }
    }

    public void SetOption(FoodOption foodOption)
    {
        GC.Dictionary<string, Variant> options = _dictionary["Options"].As<GC.Dictionary<string, Variant>>();
        List<string> optionNames = options.Keys.ToList();
        List<FoodOption> foodOptions = GetList<FoodOption>(optionNames);
        FoodOption option = foodOptions.Find(option => option == foodOption);
        Option = option;
    }

    public void SetAddon(FoodAddon foodAddon)
    {
        GC.Dictionary<string, Variant> addons = _dictionary["Addons"].As<GC.Dictionary<string, Variant>>();
        List<string> addonNames = addons.Keys.ToList();
        List<FoodAddon> foodAddons = GetList<FoodAddon>(addonNames);
        FoodAddon addon = foodAddons.Find(addon => addon == foodAddon);
        Addon = addon;
    }

    public void SetTexture()
    {
        GC.Dictionary<string, Variant> options = _dictionary["Options"].As<GC.Dictionary<string, Variant>>();
        Image optionImage = GetImage(options, Option);

        GC.Dictionary<string, Variant> addons = _dictionary["Addons"].As<GC.Dictionary<string, Variant>>();
        Image addonImage = GetImage(addons, Addon);

        Vector2I des = optionImage.GetSize() / 2 - addonImage.GetSize() / 2;
        Rect2I srcRect = new Rect2I()
        {
            Size = addonImage.GetSize()
        };
        optionImage.BlendRect(addonImage, srcRect, des);
        ImageTexture imageTexture = ImageTexture.CreateFromImage(optionImage);
        Texture = imageTexture;
    }

    private Image GetImage(GC.Dictionary<string, Variant> dictionary, Enum foodEnum)
    {
        Image baseImage = null;
        if (foodEnum is FoodOption)
        {
            string key = foodEnum.ToString();
            List<string> filePaths = dictionary[key].As<GC.Array<string>>().ToList();

            baseImage = BearBakery.GetTexture(filePaths[0]).GetImage();
            if (filePaths.Count > 1)
            {
                foreach (string filePath in filePaths)
                {
                    Image image = BearBakery.GetTexture(filePath).GetImage();
                    Vector2I des = image.GetSize() / 2 - image.GetSize() / 2;
                    Rect2I srcRect = new Rect2I()
                    {
                        Size = image.GetSize()
                    };
                    baseImage.BlendRect(image, srcRect, des);
                }
            }
        }
        else if (foodEnum is FoodAddon)
        {
            Regex regex = new Regex(@"(?<=[A-Z])(?=[A-Z][a-z])|(?<=[^A-Z])(?=[A-Z])|(?<=[A-Za-z])(?=[^A-Za-z])");
            string key = foodEnum.ToString();
            key = regex.Replace(key, " ");
            string filePath = dictionary[key].As<string>();
            baseImage = BearBakery.GetTexture(filePath).GetImage();
        }

        return baseImage;
    }

    private List<T> GetList<T>(List<string> names) where T : struct
    {
        List<T> list = new List<T>();
        for (int i = 0; i < names.Count; i++)
        {
            names[i] = names[i].Replace(" ", "");
            T thing = Enum.Parse<T>(names[i]);
            list.Add(thing);
        }
        return list;
    }

    /* e.g 
        Food Name = "Cookie"
        Food Option = "Stuffed"
    */
    private void PsuedoGetImage(string foodName, FoodOption foodOption)
    {
        string fileDirectory = "res://Assets/Images/Food/";
        string fileName = $"{foodOption}foodName"; // "StuffedCookie"
        string filePath = $"{fileDirectory}{fileName}.png"; // "res://Assets/Images/Food/StuffedCookie.png"
    }
}