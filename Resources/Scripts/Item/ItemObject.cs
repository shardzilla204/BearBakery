using Godot;

namespace BearBakery;

public partial class ItemObject : StaticBody2D
{
	[Export]
    private InteractComponent _interactComponent;

	[Export]
	private Sprite2D _sprite;

	[Export]
	private Timer _pickUpTimer;

	[Export(PropertyHint.Range, "0.5, 1.5, 0.1")]
    private float _pickUpDelay = 0.5f;

	public Item Item;
	public bool InRange = false;

	private Tween _tween;

	public async override void _EnterTree()
	{
		await ToSignal(GetTree().CreateTimer(_pickUpDelay), SceneTreeTimer.SignalName.Timeout); // Prevents instant pick up when throwing

		_interactComponent.Interacted += PickUp;
		_interactComponent.AreaEntered += (area) => PlayerEntered();
		_interactComponent.AreaExited += (area) => PlayerExited();

		_pickUpTimer.Timeout += PickUp;
	}

    public override void _Ready()
    {
		_sprite.Texture = Item.Texture;
    }

	public override void _Process(double delta)
	{
		if (InRange) MoveTowardPlayer();
	}

	private void PlayerEntered()
	{
		if (IsInventoryFull()) return;

		InRange = true;
		InventoryManager.ItemQueue.Insert(InventoryManager.ItemQueue.Count, this);
	}

	private void PlayerExited()
	{	
		InRange = false;
		_pickUpTimer.Stop();
		InventoryManager.ItemQueue.Remove(this);
	}

	private void PickUp()
	{
		if (_tween != null) _tween.Kill();

		string pickedUpMessage = $"Player Picked Up {PrintRich.GetItemName(TextColor.Yellow, Item)}";
		PrintRich.Print(TextColor.Orange, pickedUpMessage);

		BearBakery.Signals.EmitSignal(Signals.SignalName.ItemAddedToInventory, Item);
		QueueFree();
	}

	private void TweenMovement()
	{
		if (!InRange) return;

		_tween = CreateTween().SetEase(Tween.EaseType.OutIn).SetTrans(Tween.TransitionType.Quad);
		_tween.TweenProperty(this, "position", BearBakery.Player.Position, 2);
	}

	private void MoveTowardPlayer()
	{
		float distanceTo = Position.DistanceTo(BearBakery.Player.Position);
		int distance = Mathf.RoundToInt(distanceTo); // Rounds the distance given between the item and player
		float distanceThreshold = 7.5f;
		if (distance <= distanceThreshold) PickUp();
		
		TweenMovement();
    }

	private bool IsInventoryFull()
	{
        return BearBakery.Player.Inventory.Items.Count >= InventoryManager.MaxSize;
    }
}