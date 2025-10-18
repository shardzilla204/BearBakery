using Godot;
using System.Collections.Generic;
using System.Linq;

namespace BearBakery;

public partial class PlayerInterface : Node
{
	[Export]
	private CustomButton _settingsButton;

	[Export]
	private Container _interfaceContainer;

	[Export]
	private Container _keybindActionContainer;

	private ItemContentInterface _itemContentInterface;
	private List<KeybindAction> _keyQueue = new List<KeybindAction>();

    public override void _ExitTree()
    {
        BearBakery.Signals.PlayerItemUpdated -= PlayerItemUpdated;
		BearBakery.Signals.ShowKeybind -= ShowKeybind;
		BearBakery.Signals.HideKeybind -= HideKeybind;
    }

	public override void _EnterTree()
	{
		BearBakery.Signals.PlayerItemUpdated += PlayerItemUpdated;
		BearBakery.Signals.ShowKeybind += ShowKeybind;
		BearBakery.Signals.HideKeybind += HideKeybind;
	}

	public override async void _Ready()
	{
		_settingsButton.Pressed += OnSettingsButtonPressed;

		await ToSignal(BearBakery.Signals, Signals.SignalName.PlayersSpawned);
		PlayerItemUpdated();
	}

	private void OnSettingsButtonPressed()
    {
        SettingsCanvas settingsCanvas = BearBakery.PackedScenes.GetSettingsCanvas();
		AddSibling(settingsCanvas);
    }

	public void AddWindow(Node window)
	{
		_interfaceContainer.AddChild(window);
		_interfaceContainer.MoveToFront();
	}

	private void PlayerItemUpdated()
	{
		if (IsInstanceValid(_itemContentInterface)) _itemContentInterface.QueueFree();

		if (BearBakery.Player.Inventory.Items.Count == 0) return;

		BearBakery.Signals.EmitSignal(Signals.SignalName.HideKeybind, 70 /* Key.F */);
		Item playerItem = BearBakery.Player.Inventory.Items[0];

		if (playerItem is not StorageItem storageItem) return;

		_itemContentInterface = BearBakery.PackedScenes.GetItemContentInterface(storageItem);
		_interfaceContainer.AddChild(_itemContentInterface);

		if (storageItem is Bowl bowl)
		{
			bool hasRecipe = RecipeManager.HasRecipe(bowl.Ingredients);
			if (!hasRecipe) return;
			
			BearBakery.Signals.EmitSignal(Signals.SignalName.ShowKeybind, 70 /* Key.F */, "Mix");
		}
	}

	private void ShowKeybind(Key key, string actionName)
	{
		KeybindAction keybindAction = new KeybindAction(key, actionName);
		_keyQueue.Add(keybindAction);
		RefreshKeybinds();
	}

	private void HideKeybind(Key key)
	{
		KeybindAction keybindAction = _keyQueue.Find(keybind => keybind.Keybind == key);
		_keyQueue.Remove(keybindAction);
		RefreshKeybinds();
	}

	private void RefreshKeybinds()
	{
		foreach (Node child in _keybindActionContainer.GetChildren())
		{
			child.QueueFree();
		}

		// Only add keybinds that aren't the same by getting a distinct list
		IEnumerable<KeybindAction> distinctKeys = _keyQueue.DistinctBy(key => key.Keybind);
		foreach (KeybindAction key in distinctKeys)
		{
			KeybindAction keybindAction = BearBakery.PackedScenes.GetKeybindAction(key.Keybind, key.ActionName);
			_keybindActionContainer.AddChild(keybindAction);
		}
	}
}
