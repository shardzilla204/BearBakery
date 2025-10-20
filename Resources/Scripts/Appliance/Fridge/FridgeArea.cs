using Godot;
using System.Collections.Generic;

namespace BearBakery;

public partial class FridgeArea : Sprite2D
{
	[Export]
	private InteractComponent _interactComponent;

	[Export]
	private LowlightComponent _lowlightComponent;

	private bool _hasInteracted = false;

	public override void _Ready()
	{
		_interactComponent.Interacted += OpenFridge;
		_interactComponent.SecondaryAction += StoreItem;
		_interactComponent.AreaEntered += OnAreaEntered;
		_interactComponent.AreaExited += OnAreaExited;
	}

	private void StoreItem()
    {
		Item item = BearBakery.Player.Inventory.Items[0];
		FridgeManager.Items.Add(item);
		BearBakery.Signals.EmitSignal(Signals.SignalName.ItemRemovedFromInventory, item);
    }

	private void OpenFridge()
	{
		_hasInteracted = true;

		string openMessage = "Opening Fridge";
		PrintRich.PrintLine(TextColor.Blue, openMessage);

		BearBakery.Signals.EmitSignal(Signals.SignalName.FridgeOpened, this);
	}

	private void OnAreaEntered(Area2D area)
	{
		_lowlightComponent.Set(true);
		SetKeybindAction(true);
	}

	private void OnAreaExited(Area2D area)
    {
		_lowlightComponent.Set(false);
		SetKeybindAction(false);
		CloseFridge();
    }

	/// <see cref="PackedScenes.GetFridgeInterface">
	public void CloseFridge()
	{
		if (!_hasInteracted) return;

		_hasInteracted = false;

		string closeMessage = "Closing Fridge";
		PrintRich.PrintLine(TextColor.Blue, closeMessage);

		PrintRich.PrintFridge();

		BearBakery.Signals.EmitSignal(Signals.SignalName.FridgeClosed);
	}

	private void SetKeybindAction(bool isHovering)
	{
		Dictionary<string, int> keybindsDictionary = new Dictionary<string, int>()
		{
			{ "Open Fridge", (int) Key.E },
		};

		if (BearBakery.Player.Inventory.Items.Count != 0)
        {
            Item item = BearBakery.Player.Inventory.Items[0];
			if (item is Ingredient || item is Food)
            {
				keybindsDictionary.Add("Store", (int) Key.F);
            }
        }
		
		foreach (string keybindName in keybindsDictionary.Keys)
		{
			int keyIndex = keybindsDictionary[keybindName];
            if (!isHovering)
			{
				BearBakery.Signals.EmitSignal(Signals.SignalName.HideKeybind, keyIndex);
			}
			else
			{
				BearBakery.Signals.EmitSignal(Signals.SignalName.ShowKeybind, keyIndex, keybindName);
			}
        }
	}
}
