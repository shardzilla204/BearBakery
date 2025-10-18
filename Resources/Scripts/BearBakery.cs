using Godot;
using GC = Godot.Collections;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace BearBakery;

public partial class BearBakery : Control
{
    [Export]
    private Texture2D _alphaTexture;

    private static Texture2D _AlphaTexture;
    
    public static Debug Debug;
    public static PackedScenes PackedScenes;
    public static Signals Signals = new Signals();

    public static Player Player;

    private static List<GC.Dictionary<string, Variant>> _FoodDictionaries = new List<GC.Dictionary<string, Variant>>();

    public override void _EnterTree()
    {
        LoadFoodFile();

        _AlphaTexture = _alphaTexture;
    }

    public static Variant LoadFile(string fileName, string folderPath = "")
    {
        folderPath = folderPath != "" ? $"{folderPath}/" : "";
        string filePath = $"res://JSON/{folderPath}{fileName}.json";

        Json json = new Json();

		try
		{
			using FileAccess fileAccess = FileAccess.Open(filePath, FileAccess.ModeFlags.Read);
			string jsonString = fileAccess.GetAsText();

            if (json.Parse(jsonString) != Error.Ok) throw new Exception();

            string loadSuccessMesssage = $"{fileName}.json Successfully Loaded";
			PrintRich.PrintFileSuccess(loadSuccessMesssage);

			GC.Dictionary<string, Variant> dictionaries = (GC.Dictionary<string, Variant>) json.Data;
			return dictionaries;
		}
		catch
		{
			string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
			string message = $"Couldn't Parse File - {fileName}";
			string result = "Returning Default/Null";
			PrintRich.PrintError(className, message, result);

			return default;
		}
    }

    private void LoadFoodFile()
    {
        string fileName = "Food";
        GC.Dictionary<string, Variant> foodsDictionary = LoadFile(fileName).As<GC.Dictionary<string, Variant>>();
        if (foodsDictionary == null) return;

        GC.Array<GC.Dictionary<string, Variant>> foodDictionaries = foodsDictionary[fileName].As<GC.Array<GC.Dictionary<string, Variant>>>();

        _FoodDictionaries = foodDictionaries.ToList();
    }

    public static Texture2D GetTexture(string fileName, string folderPath = "")
    {
        folderPath = folderPath != "" ? $"{folderPath}/" : "";
        string filePath = $"res://Assets/Images/{folderPath}{fileName}.png";
        return ResourceLoader.Load<Texture2D>(filePath);
    }

    public static Food GetFood(string foodName, FoodOption foodOption, FoodAddon foodAddon)
    {
        GC.Dictionary<string, Variant> foodDictionary = _FoodDictionaries.Find(dictionary => FindFood(dictionary, foodName));
        Food food = new Food(foodDictionary);
        food.SetOption(foodOption);
        food.SetAddon(foodAddon);
        food.SetTexture();

        return food;
    }

    private static bool FindFood(GC.Dictionary<string, Variant> dictionary, string foodName)
    {
        return dictionary["Name"].As<string>() == foodName;
    }

    public static void ToggleCursorShapeVisibility(bool isHidden, Input.CursorShape cursorShape = Input.CursorShape.Arrow)
    {
        if (isHidden)
        {
            Input.SetCustomMouseCursor(_AlphaTexture, cursorShape);
        }
        else
        {
            Input.SetCustomMouseCursor(null, cursorShape);
        }
    }
}
