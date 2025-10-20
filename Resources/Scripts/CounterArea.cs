using Godot;
using Godot.Collections;
using System.Threading.Tasks;

namespace BearBakery;

public partial class CounterArea : Node2D
{
	[Export]
	private DisplayComponent _displayComponent;

	[Export]
	private InteractComponent _interactComponent;

	[Export]
	private SwapComponent _swapComponent;

	[Export]
	private LowlightComponent _lowlightComponent;

	[Export]
	private string _startingItem;

	public Item Item;

	private HFlowContainer _ingredientsNode;
	private KeybindAction _keybindAction;

	public override void _Ready()
	{
		_interactComponent.Interacted += OnInteracted;
		_interactComponent.SecondaryAction += OnSecondaryAction;
		_interactComponent.AreaEntered += OnAreaEntered;
		_interactComponent.AreaExited += OnAreaExited;

		_swapComponent.Swapped += OnSwapped;

		AddStartingItem();
	}

	private void AddStartingItem()
    {
		if (string.IsNullOrEmpty(_startingItem)) return;

		Item item = ItemManager.GetItem(_startingItem);
		SetItem(item);

		if (item is StorageItem storageItem)
        {
			AddIngredientsNode(storageItem);
        }
    }

	/// <summary>
	/// Show the keybind and darken the self modulate
	/// </summary>
	/// <param name="area"></param>
	private void OnAreaEntered(Area2D area)
	{
		if (BearBakery.Player.Inventory.Items.Count != 0 || Item != null)
        {
			_lowlightComponent.Set(true);
			SetKeybindAction(true);
        }

		CanSwap();
	}
	
	/// <summary>
    /// Show the keybind and darken the self modulate
    /// </summary>
    /// <param name="area"></param>
	private void OnAreaExited(Area2D area)
	{
		if (BearBakery.Player.Inventory.Items.Count != 0 || Item != null)
		{
			_lowlightComponent.Set(false);
			SetKeybindAction(false);
		}
		
		CanSwap();
    }

	private void OnInteracted()
	{
		if (Item != null)
		{
			GetItem();
			SetKeybindAction(false);
		}
		else
		{
			SetItem();
		}
	}

	private async void OnSecondaryAction()
    {
        if (BearBakery.Player.Inventory.Items.Count != 0)
		{
			Item playerItem = BearBakery.Player.Inventory.Items[0];

			bool isSuccessful = await StoreItem(playerItem);
			if (isSuccessful) return;
		}
    }

	private void OnSwapped()
	{
		if (BearBakery.Player.Inventory.Items.Count == 0) return;

		Item itemToSwap = Item;
		Item playerItem = BearBakery.Player.Inventory.Items[0];

		BearBakery.Player.Inventory.Items.RemoveAt(0);
		BearBakery.Player.Inventory.Items.Insert(0, itemToSwap);

		SetItem(playerItem);
	}

	private void CanSwap()
	{
		_swapComponent.CanSwap = BearBakery.Player.Inventory.Items.Count == 0 || Item != null;
	}

	private void GetItem()
	{
		int playerInventoryCount = BearBakery.Player.Inventory.Items.Count;
		int maxInventorySize = InventoryManager.MaxSize;

		if (playerInventoryCount >= maxInventorySize) return;

		if (Item is Bowl)
		{
			_ingredientsNode.QueueFree();
		}

		BearBakery.Signals.EmitSignal(Signals.SignalName.ItemAddedToInventory, Item);
		SetItem(null);
	}

	private void SetItem()
	{
		if (BearBakery.Player.Inventory.Items.Count == 0) return;

		Item item = BearBakery.Player.Inventory.Items[0];
		SetItem(item);

		if (item is StorageItem storageItem) AddIngredientsNode(storageItem);

		BearBakery.Signals.EmitSignal(Signals.SignalName.ItemRemovedFromInventory, item);
	}

	private void SetItem(Item item)
	{
		Item = item;
		_displayComponent.SetTexture(item);
	}

	private void SetKeybindAction(bool isHovering)
	{
		Dictionary<string, int> keybindsDictionary = new Dictionary<string, int>()
		{
			{ "Interact", (int) Key.E }
		};

		if (Item is StorageItem && BearBakery.Player.Inventory.Items.Count != 0)
		{
			Item item = BearBakery.Player.Inventory.Items[0];
			if (item is Ingredient)
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

	private Task<bool> StoreItem(Item playerItem)
	{
		if (playerItem is not Ingredient ingredient || Item is not StorageItem storageItem) return Task.FromResult(false);

		if (storageItem.IsFull()) return Task.FromResult(false);

		storageItem.AddIngredient(ingredient);

		_ingredientsNode.QueueFree();
		AddIngredientsNode(storageItem);

		BearBakery.Signals.EmitSignal(Signals.SignalName.ItemRemovedFromInventory, ingredient);
		return Task.FromResult(true);
	}

	private void AddIngredientsNode(StorageItem storageItem)
	{
		_ingredientsNode = storageItem.GetIngredientsNode();
		_displayComponent.AddChild(_ingredientsNode);
		_ingredientsNode.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.CenterBottom);
		_ingredientsNode.Position = new Vector2(5.35f, -10);
	}
}
