using System.Threading.Tasks;
using Godot;

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
	private TextureRect _selfModulate;

	public Item Item;

	private Color _originalColor;
	private HFlowContainer _ingredientsNode;
	private KeybindAction _keybindAction;

	public override void _Ready()
	{
		_interactComponent.Interacted += Interacted;
		_interactComponent.AreaEntered += (area) =>
		{
			ChangeLowlight(true);
			SetKeybindAction(true);
			CanSwap();
		};
		_interactComponent.AreaExited += (area) =>
		{
			ChangeLowlight(false);
			SetKeybindAction(false);
			CanSwap();
		};

		_swapComponent.Swapped += Swapped;

		_originalColor = _displayComponent.SelfModulate;
	}

	private async void Interacted()
	{
		if (BearBakery.Player.Inventory.Items.Count != 0)
		{
			Item playerItem = BearBakery.Player.Inventory.Items[0];

			bool isSuccessful = await StoreItem(playerItem);
			if (isSuccessful) return;
		}

		if (Item != null)
		{
			GetItem();
		}
		else
		{
			SetItem();
		}
	}

	private void Swapped()
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
		int maxInventorySize = BearBakery.InventoryManager.MaxSize;

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
		_selfModulate.Texture = item == null ? null : item.Texture;
	}

	private void SetKeybindAction(bool isHovering)
	{
		if (!isHovering)
		{
			BearBakery.Signals.EmitSignal(Signals.SignalName.HideKeybind, 69 /* Key.E */);
		}
		else
		{
			BearBakery.Signals.EmitSignal(Signals.SignalName.ShowKeybind, 69 /* Key.E */, "Interact");
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

	private void ChangeLowlight(bool isHovering)
	{
		Color lowlight = Colors.Black;
		lowlight.A = isHovering ? 0.2f : 0;
		TweenOpacity(lowlight);
	}

	private void TweenOpacity(Color targetColor)
	{
		Tween tween = CreateTween().SetTrans(Tween.TransitionType.Quad);
		tween.TweenProperty(_selfModulate, "self_modulate", targetColor, 0.25f);
	}

	private void AddIngredientsNode(StorageItem storageItem)
	{
		_ingredientsNode = storageItem.GetIngredientsNode();
		_displayComponent.AddChild(_ingredientsNode);
		_ingredientsNode.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.CenterBottom);
		_ingredientsNode.Position = new Vector2(5.35f, -10);
	}
}
