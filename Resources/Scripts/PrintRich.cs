using Godot;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BearBakery;

public enum TextColor
{
   White,
   Red,
   Orange,
   Yellow,
   Green,
   Purple,
   Blue,
   Pink,
   LightBlue,
   Brown,
}

public partial class PrintRich : Node
{
   [Export]
   private bool _areConsoleMessagesEnabled = true;

   [Export]
   private bool _areFileMessagesEnabled = false;

   [Export]
   private bool _areFilePathsVisible = false;

   public static bool AreConsoleMessagesEnabled;
   public static bool AreFileMessagesEnabled;
   public static bool AreFilePathsVisible;

   public override void _EnterTree()
   {
      AreConsoleMessagesEnabled = _areConsoleMessagesEnabled;
      AreFileMessagesEnabled = _areFileMessagesEnabled;
      AreFilePathsVisible = _areFilePathsVisible;
   }

   public static void Print(TextColor textColor, string text)
   {
      if (!AreConsoleMessagesEnabled) return;

      string textColorString = GetColorHex(textColor);
      GD.PrintRich($"[color={textColorString}]{text}[/color]");
   }

   public static void PrintLine(TextColor textColor, string text)
   {
      if (!AreConsoleMessagesEnabled) return;

      string textColorString = GetColorHex(textColor);
      GD.PrintRich($"[color={textColorString}]{text}[/color]");
      GD.Print(); // Spacing
   }

   public static void PrintServer()
   {
      Print(TextColor.Pink, $"Players In Server:");
      foreach (PlayerInfo playerInfo in GameManager.PlayersInfo)
      {
         string playerInfoString = $"\tPlayer ({playerInfo.Id}):\n\t\tName: {playerInfo.Name}";
         PrintLine(TextColor.Pink, playerInfoString);
      }
   }

   public static void PrintInventory()
   {
      Print(TextColor.Blue, "Player Inventory:");
      foreach (Item item in BearBakery.Player.Inventory.Items)
      {
         Print(TextColor.Yellow, $"\t{item.Name}");
      }
      GD.Print(); // Print Spacing
   }

   public static void PrintFridge()
   {
      Print(TextColor.LightBlue, "Ingredients In The Fridge:");
      foreach (Item item in BearBakery.FridgeManager.Items)
      {
         if (item is Food food)
         {
            string option = food.Option == FoodOption.Plain ? "" : $"{GetOptionName(TextColor.Purple, food)} ";
            string addon = food.Addon == FoodAddon.None ? "" : $" {GetAddonName(TextColor.Purple, food)}";
            Print(TextColor.Yellow, $"\t{option}{food.Name}{addon}");
            continue;
         }
         Print(TextColor.Yellow, $"\t{item.Name}");
      }
      GD.Print(); // Print Spacing
   }

   public static string GetColorHex(TextColor textColor) => textColor switch
   {
      TextColor.Red => "FF4040",
      TextColor.Orange => "F88158",
      TextColor.Yellow => "E9D66B",
      TextColor.Green => "76CD26",
      TextColor.Purple => "CA9BF7",
      TextColor.Blue => "6495ED",
      TextColor.Pink => "FF69B4",
      TextColor.LightBlue => "ADD8E6",
      TextColor.Brown => "C4A484",
      _ => "FFFFFF",
   };

   public static string GetItemName(TextColor textColor, Item item)
   {
      string textColorString = GetColorHex(textColor);
      return $"[color={textColorString}]{item.Name}[/color]";
   }

   private static string GetOptionName(TextColor textColor, Food food)
   {
      string textColorString = GetColorHex(textColor);
      return $"[color={textColorString}]{food.Option}[/color]";
   }

   private static string GetAddonName(TextColor textColor, Food food)
   {
      Regex regex = new Regex(@"(?<=[A-Z])(?=[A-Z][a-z])|(?<=[^A-Z])(?=[A-Z])|(?<=[A-Za-z])(?=[^A-Za-z])");

      string foodAddonString = food.Addon.ToString();
      string foodAddonName = regex.Replace(foodAddonString, " ");

      string whiteColorString = GetColorHex(TextColor.White);
      string textColorString = GetColorHex(textColor);
      return $"[color={whiteColorString}]With [/color][color={textColorString}]{foodAddonName}[/color]";
   }
}