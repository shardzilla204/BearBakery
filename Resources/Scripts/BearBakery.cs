using System;
using System.Reflection;
using Godot;
using GC = Godot.Collections;

namespace BearBakery;

public partial class BearBakery : Control
{
    public static Debug Debug;
    public static PackedScenes PackedScenes;
    public static Signals Signals = new Signals();

    public static Player Player;

    private static GC.Dictionary<string, Variant> _FoodDictionary;

    public override void _EnterTree()
    {
        LoadFoodFile();
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
        GC.Dictionary<string, Variant> foodDictionary = LoadFile(fileName).As<GC.Dictionary<string, Variant>>();
        if (foodDictionary == null) return;

        _FoodDictionary = foodDictionary;
    }

    public static Texture2D GetTexture(string fileName, string folderPath = "")
    {
        folderPath = folderPath != "" ? $"{folderPath}/" : "";
        string filePath = $"res://Assets/Images/{folderPath}{fileName}";
        return ResourceLoader.Load<Texture2D>(filePath);
    }

    public static Food GetFood(string foodName, FoodOption foodOption, FoodAddon foodAddon)
    {
        GC.Dictionary<string, Variant> foodDictionary = _FoodDictionary[foodName].As<GC.Dictionary<string, Variant>>();
        Food food = new Food(foodDictionary);
        food.SetOption(foodOption);
        food.SetAddon(foodAddon);
        food.SetTexture();

        return food;
    }
}
