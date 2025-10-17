using Godot;

namespace BearBakery;

public partial class GameCanvas : Node2D
{
	[Export]
	private Node2D _itemContainer;

	[Export]
	private Texture2D _alpha;

	private CanvasType _canvasType = CanvasType.Game;

	public override void _ExitTree()
	{
		BearBakery.Signals.PlayerThrew -= AddItemToWorld;

		BearBakery.Signals.EmitSignal(Signals.SignalName.CanvasClosed, (int) _canvasType, this);
	}

    public override void _EnterTree()
    {
		BearBakery.Signals.PlayerThrew += AddItemToWorld;

		BearBakery.Signals.EmitSignal(Signals.SignalName.CanvasOpened, (int)_canvasType, this);
    }

	public override void _Ready()
	{
		// Hides cursor when dragging
		Input.SetCustomMouseCursor(_alpha, Input.CursorShape.Forbidden);
		Input.SetCustomMouseCursor(_alpha, Input.CursorShape.CanDrop);

		AddPlayers();
	}

	private void AddItemToWorld(Item item)
	{
		ItemObject itemObject = BearBakery.PackedScenes.GetItemObject(item);
		itemObject.GlobalPosition = BearBakery.Player.GlobalPosition;

		_itemContainer.AddChild(itemObject);
        TweenThrow(itemObject);
	}

	private void TweenThrow(ItemObject itemObject)
	{
		Vector2 initialPosition = itemObject.GlobalPosition;

		Vector2 motion = BearBakery.Player.Velocity == Vector2.Zero ? BearBakery.Player.Direction : BearBakery.Player.Velocity.Normalized();
		float throwDistance = BearBakery.Player.Controller.ThrowDistance;
		itemObject.MoveAndCollide(motion * throwDistance);

		Vector2 finalPosition = itemObject.GlobalPosition;

		// Reset position after getting the desination after throwing
		itemObject.GlobalPosition = initialPosition;

		Tween tween = CreateTween().SetTrans(Tween.TransitionType.Quint).SetEase(Tween.EaseType.Out);
		tween.TweenProperty(itemObject, "global_position", finalPosition, 0.5f);
	}
	
	private void AddPlayers()
	{
		if (GameManager.PlayersInfo.Count == 0)
		{
			Player player = BearBakery.PackedScenes.GetPlayer();
			player.Position = new Vector2(500, 400);
			AddChild(player);

			BearBakery.Player = player;
		}
		else
		{
			GD.Print($"Players Count: {GameManager.PlayersInfo.Count}");
			foreach (PlayerInfo playerInfo in GameManager.PlayersInfo)
			{
				Player player = BearBakery.PackedScenes.GetPlayer();
				player.Name = playerInfo.Id.ToString();
				player.Info = playerInfo;
				player.Position = new Vector2(500, 400);
				AddChild(player);

				// Create as Rpc
				// GameManager.Players.Add(player);

				if (playerInfo.Id == Multiplayer.GetUniqueId())
				{
					GD.Print($"{Multiplayer.GetUniqueId()} Has Been Set soifjsieof");
					BearBakery.Player = player;
                }
			} 
        }

		BearBakery.Signals.EmitSignal(Signals.SignalName.PlayersSpawned);
    }
}
