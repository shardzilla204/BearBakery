using Godot;
using GC = Godot.Collections;

namespace BearBakery;

/* Multiplayer Notes

*/

public partial class BearBakery : Control
{
    [Export(PropertyHint.Range, "3,5,1")]
    private int _maxInventorySize = 3;

    public static Debug Debug;
    public static PackedScenes PackedScenes;
    public static Signals Signals = new Signals();

    public static Player Player;
    public static GameManager GameManager;
    public static InventoryManager InventoryManager;
    public static StaminaManager StaminaManager;
    public static OvenManager OvenManager;
    public static FridgeManager FridgeManager;
    public static MixerManager MixerManager;

    public static Items Items;
    public static Ingredients Ingredients;
    public static Recipes Recipes;

    private static GC.Dictionary<string, Variant> _FoodDictionary;

    public override void _EnterTree()
    {
        LoadFoodFile();
    }

    private void LoadFoodFile()
    {
        string filePath = "res://Resources/JSON/Food.json";

        using FileAccess itemsFile = FileAccess.Open(filePath, FileAccess.ModeFlags.Read);
        string jsonString = itemsFile.GetAsText();

        Json json = new Json();

        if (json.Parse(jsonString) != Error.Ok) return;

        _FoodDictionary = new GC.Dictionary<string, Variant>((GC.Dictionary)json.Data);

        // Print Messages To Console
        string loadSuccessMesssage = $"Food Dictionary Successfully Loaded";
        if (PrintRich.AreFileMessagesEnabled) PrintRich.PrintLine(TextColor.Green, loadSuccessMesssage);
    }

    public static Texture2D GetTexture(string filePath)
    {
        // if (filePath == "") return null;
        return ResourceLoader.Load<Texture2D>(filePath);
    }

    public static Food GetFood(string foodName, FoodOption foodOption, FoodAddon foodAddon)
    {
        GC.Dictionary<string, Variant> foodDictionary = _FoodDictionary[foodName].As<GC.Dictionary<string, Variant>>();
        Food food = new Food();
        food.SetItem(foodName, foodDictionary);
        food.SetOption(foodOption);
        food.SetAddon(foodAddon);
        food.SetTexture();

        return food;
    }
}
