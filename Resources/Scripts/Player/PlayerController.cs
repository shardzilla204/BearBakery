using Godot;

namespace BearBakery;

public partial class PlayerController : Node2D
{
	[Export]
	public float MovementSpeed = 200f;

	[Export]
	public float SprintMultiplier = 1.75f;

	[Export]
	public float LeapDistance = 50;

	[Export(PropertyHint.Range, "15, 150, 5")]
	public int ThrowDistance = 15;

	[Export]
	private float _friction = 250;

	[Export]
	private float _acceleration = 250f;

	[Export]
	private MultiplayerSynchronizer _multiplayerSynchronizer;

	private Player _player;
	private Vector2 _syncPosition;
	private Vector2 _previousPosition;

	public override void _ExitTree()
	{
		BearBakery.Signals.PlayerLeaped -= PlayerLeaped;
	}

	public override void _EnterTree()
	{
		BearBakery.Signals.PlayerLeaped += PlayerLeaped;
	}

    public override void _Ready()
    {
		_player = GetParent<Player>();
		_syncPosition = _player.GlobalPosition;
    }
	
	public override void _Process(double delta)
	{
		IsSprinting((float) delta);
		Movement((float) delta);
		_player.MoveAndSlide();

		// * Action { C }

		if (MultiplayerManager.Peer == null ||
			(MultiplayerManager.Peer != null && _multiplayerSynchronizer.GetMultiplayerAuthority() == Multiplayer.GetUniqueId()))
		{
			if (Input.IsActionJustPressed("Interact")) BearBakery.Signals.EmitSignal(Signals.SignalName.PlayerInteracted);
			if (Input.IsActionJustPressed("SecondaryAction")) BearBakery.Signals.EmitSignal(Signals.SignalName.PlayerSecondaryAction);
        	if (Input.IsActionJustPressed("Throw")) ThrowItem();
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (MultiplayerManager.Peer == null ||
			(MultiplayerManager.Peer != null && _multiplayerSynchronizer.GetMultiplayerAuthority() == Multiplayer.GetUniqueId()))
		{
			IsLeaping();
		}
	}

	private void Movement(float delta)
	{
		if (MultiplayerManager.Peer == null ||
			(MultiplayerManager.Peer != null && _multiplayerSynchronizer.GetMultiplayerAuthority() == Multiplayer.GetUniqueId()))
		{
			SetDirection();
			_player.IsMoving = _player.Direction == Vector2.Zero ? false : true;

			StringName playerSignal = _player.IsMoving ? Signals.SignalName.PlayerMoved : Signals.SignalName.PlayerIdle;
			BearBakery.Signals.EmitSignal(playerSignal);

			if (!_player.IsLeaping)
			{
				PlayerAnimation.State state = _player.IsMoving ? PlayerAnimation.State.Walk : PlayerAnimation.State.Idle;
				_player.Animation.EnterState(state, false);
			}

			Vector2 speed = _player.Direction * MovementSpeed;
			speed = _player.IsSprinting ? speed * SprintMultiplier : speed;

			Vector2 friction = _player.Velocity.MoveToward(Vector2.Zero, delta * _friction);
			Vector2 acceleration = _player.Velocity.MoveToward(speed, delta * _acceleration);
			_player.Velocity = _player.Direction == Vector2.Zero ? friction : acceleration;

			_syncPosition = _player.GlobalPosition;
		}
		else if (MultiplayerManager.Peer != null)
		{
			_player.GlobalPosition = _syncPosition;
		}
	}

	public void SetDirection()
	{
		_player.Direction = Vector2.Zero;
		if (Input.IsKeyPressed(Key.W))
		{
			_player.Direction += Vector2.Up;
		}

		if (Input.IsKeyPressed(Key.A))
		{
			_player.Direction += Vector2.Left;
		}

		if (Input.IsKeyPressed(Key.S))
		{
			_player.Direction += Vector2.Down;
		}

		if (Input.IsKeyPressed(Key.D))
		{
			_player.Direction += Vector2.Right;
		}
	}

	private void IsSprinting(float delta)
	{
		if (MultiplayerManager.Peer == null ||
			(MultiplayerManager.Peer != null && _multiplayerSynchronizer.GetMultiplayerAuthority() == Multiplayer.GetUniqueId()))
		{
			if (_player.Direction == Vector2.Zero || _player.IsLeaping) return;

			_player.IsSprinting = Input.IsActionPressed("Sprint") && !StaminaManager.IsDepleted() ? true : false;

			if (_player.IsSprinting) BearBakery.Signals.EmitSignal(Signals.SignalName.PlayerSprinting, delta);
		}
	}

	private void IsLeaping()
	{
		if (_player.IsLeaping) return;

		_player.IsLeaping = Input.IsActionJustPressed("Leap") && StaminaManager.CanLeap() ? true : false;
		if (_player.IsLeaping)
		{
			_player.ZIndex = 1;
			BearBakery.Signals.EmitSignal(Signals.SignalName.PlayerLeaped);
		}
	}

	private void PlayerLeaped()
	{
		Vector2 leapTargetVelocity = GetLeapTargetVelocity();
		TweenLeap(leapTargetVelocity);
		_player.TogglePhysicsLayerMask();
	}

	private Vector2 GetLeapTargetVelocity()
	{
		Vector2 leapVelocity = _player.IsMoving ? LeapDistance * _player.Direction : _player.Direction;
		Vector2 leapTargetVelocity = new Vector2(_player.Velocity.X + leapVelocity.X, _player.Velocity.Y + leapVelocity.Y);

		float sprintMultiplier = 1.15f;
		return _player.IsSprinting ? leapTargetVelocity * sprintMultiplier : leapTargetVelocity;
	}

	/// <summary>
    /// Have the sprite move up and down while changing the velocity of the player body
    /// </summary>
    /// <param name="targetVelocity"></param>
	/// 
	private void TweenLeap(Vector2 targetVelocity)
	{
		if (MultiplayerManager.Peer == null ||
			(MultiplayerManager.Peer != null && _multiplayerSynchronizer.GetMultiplayerAuthority() == Multiplayer.GetUniqueId()))
		{
			Tween tween = CreateTween().SetParallel(true).SetTrans(Tween.TransitionType.Circ);
			tween.TweenProperty(_player, "velocity", targetVelocity, 0.35f);

			Vector2 originalPosition = _player.Animation.Position;
			Vector2 targetPosition = new Vector2(originalPosition.X, originalPosition.Y - 10);
			tween.TweenProperty(_player.Animation, "position", targetPosition, 0.35f);
			tween.Chain().TweenProperty(_player.Animation, "position", originalPosition, 0.35f);
			tween.Finished += () =>
			{
				_player.ZIndex = 0;
				_player.IsLeaping = false;
				IncreaseFriction();
			};
		}
	}

	private async void IncreaseFriction()
	{
		float originalFriction = _friction;

		// GD.Print($"Before: {_friction}");
		float percentage = 0.5f;
		float increase = _friction * percentage;
		_friction += increase;
		// GD.Print($"After: {_friction}");

		await ToSignal(GetTree().CreateTimer(1), SceneTreeTimer.SignalName.Timeout);

		_friction = originalFriction;
	}

    /// Also calls | <see cref="RemoveItem"> through <see cref="Signals > ItemRemovedFromInventory">
    private void ThrowItem()
    {
        if (_player.Inventory.Items.Count == 0) return;

        Item itemToThrow = _player.Inventory.Items[0];
        BearBakery.Signals.EmitSignal(Signals.SignalName.ItemRemovedFromInventory, itemToThrow);
        BearBakery.Signals.EmitSignal(Signals.SignalName.PlayerThrew, itemToThrow);

        string playerThrewMessage = $"Player Throwing Away {PrintRich.GetItemName(TextColor.Yellow, itemToThrow)}";
        PrintRich.PrintLine(TextColor.Orange, playerThrewMessage);
		
    }
}
