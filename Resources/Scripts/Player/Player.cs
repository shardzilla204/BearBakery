using System;
using Godot;

namespace BearBakery;

public enum PlayerDirection
{
	Up,
	Left,
	Down,
	Right
}

public partial class Player : CharacterBody2D
{
	[Export]
	private Area2D _interactArea;

	[Export]
	private Timer _collisionTimer;

	[Export]
	private Sprite2D _itemHeld;

	[Export]
	private MultiplayerSynchronizer _multiplayerSynchronizer;

	[ExportGroup("Player Nodes")]
	[Export]
	public PlayerController Controller;

	[Export]
	public PlayerAnimation Animation;

	[Export]
	public PlayerInventory Inventory;

	[Export]
	public PlayerStamina Stamina;

	public bool IsMoving = false;
	public bool IsSprinting = false;
	public bool IsLeaping = false;

	public Vector2 Direction = Vector2.Zero;

	public PlayerInfo Info;

	public override void _ExitTree()
	{
		BearBakery.Signals.PlayerMoved -= UpdateInteractCollision;
		BearBakery.Signals.PlayerLeaped -= _collisionTimer.Stop;
		BearBakery.Signals.PlayerItemUpdated -= SetPlayerItem;
	}

	public override void _EnterTree()
	{
		BearBakery.Signals.PlayerMoved += UpdateInteractCollision;
		BearBakery.Signals.PlayerLeaped += _collisionTimer.Stop;
		BearBakery.Signals.PlayerItemUpdated += SetPlayerItem;

		_collisionTimer.Timeout += () => SetCollisionMaskValue(8, true);
	}

	public override void _Ready()
	{
		_multiplayerSynchronizer.SetMultiplayerAuthority(int.Parse(Name));
		SetPlayerItem();
	}

	public override void _Process(double delta)
	{
		if (MultiplayerManager.Peer == null ||
			(MultiplayerManager.Peer != null && _multiplayerSynchronizer.GetMultiplayerAuthority() == Multiplayer.GetUniqueId()))
		{
			if (Input.IsActionJustPressed("Interact")) BearBakery.Signals.EmitSignal(Signals.SignalName.PlayerInteracted);
			if (Input.IsActionJustPressed("SecondaryAction")) BearBakery.Signals.EmitSignal(Signals.SignalName.PlayerSecondaryAction);
		}
	}

	private void UpdateInteractCollision()
	{
		float distance = 20;
		_interactArea.Position = Direction * distance;
	}

	// Get the physics layer that hold the passable tiles
	// Turn the layer off via mask
	// Wait a bit 
	// Turn the layer on via mask - SetPhysicsLayerCollisionMask(int layerIndex, int mask) (1, 6)
	public void TogglePhysicsLayerMask()
	{
		SetCollisionMaskValue(8, false);
		_collisionTimer.Start();
	}

	private void SetPlayerItem()
	{
		try
		{
			_itemHeld.Texture = Inventory.Items.Count <= 0 ? null : Inventory.Items[0].Texture;
		}
		catch (NullReferenceException)
		{
			_itemHeld.Texture = null;
		}
	}
}
