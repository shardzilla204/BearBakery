using Godot;

namespace BearBakery;

public partial class FridgeInterface : Control
{
	[Export]
	private CustomButton _exitButton;

	[Export]
	private Label _fridgeSlotCounter;

	[Export]
	private FridgeContainer _fridgeContainer;

	public override void _ExitTree()
	{
		BearBakery.Signals.FridgeClosed -= QueueFree;
		BearBakery.Signals.IngredientAddedToFridge -= IngredientAddedToFridge;
		BearBakery.Signals.IngredientRemovedFromFridge -= IngredientRemovedFromFridge;
	}

	public override void _Ready()
	{
		BearBakery.Signals.FridgeClosed += QueueFree;
		BearBakery.Signals.IngredientAddedToFridge += IngredientAddedToFridge;
		BearBakery.Signals.IngredientRemovedFromFridge += IngredientRemovedFromFridge;

		_exitButton.Pressed += () => BearBakery.Signals.EmitSignal(Signals.SignalName.FridgeClosed);

		SetCounter();
	}

	private void IngredientAddedToFridge(Ingredient ingredient)
	{
		SetCounter();

		string ingredientAddedMessage = $"{PrintRich.GetItemName(TextColor.Yellow, ingredient)} Was Added To The Fridge";
		PrintRich.PrintLine(TextColor.Orange, ingredientAddedMessage);
	}

	private void IngredientRemovedFromFridge(Ingredient ingredient)
	{
		SetCounter();

		string ingredientRemovedMessage = $"{PrintRich.GetItemName(TextColor.Yellow, ingredient)} Was Removed From The Fridge";
		PrintRich.PrintLine(TextColor.Orange, ingredientRemovedMessage);
	}

	private void SetCounter()
	{
		_fridgeSlotCounter.Text = $"{BearBakery.FridgeManager.Items.Count} / {BearBakery.FridgeManager.MaxSlotCount}";
	}
}
