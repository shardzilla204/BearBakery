using System;
using Godot;

namespace BearBakery;

public partial class PlayerAnimation : AnimatedSprite2D
{
	public enum State
	{
		Idle,
		Walk,
		Leap
	}

	[Export]
	private AnimationTree _animationTree;

	[Export]
	private MultiplayerSynchronizer _multiplayerSynchronizer;

	private State _state = State.Idle;

	private AnimationNodeStateMachine _stateMachince;
	private AnimationNodeStateMachinePlayback _stateMachinePlayback;

	private Player _player;

	public override void _ExitTree()
	{
		MultiplayerManager.Signals.PlayerAnimationChanged -= OnPlayerAnimationChanged;
	}

    public override void _EnterTree()
    {
        MultiplayerManager.Signals.PlayerAnimationChanged += OnPlayerAnimationChanged;
    }

	public override void _Ready()
	{
		_player = GetParent<Player>();

		_stateMachince = (AnimationNodeStateMachine) _animationTree.TreeRoot;
		_stateMachinePlayback = _animationTree.Get("parameters/playback").As<AnimationNodeStateMachinePlayback>();
	}

	public override void _Process(double delta)
	{
		if (MultiplayerManager.Peer == null ||
			(MultiplayerManager.Peer != null && _multiplayerSynchronizer.GetMultiplayerAuthority() == Multiplayer.GetUniqueId()))
		{
			if (_player.Direction == Vector2.Right)
			{
				FlipH = false;
			}
			else if (_player.Direction == Vector2.Left)
			{
				FlipH = true;
			}

			if (_player.IsLeaping) EnterState(State.Leap, false);

			SprintSpeed();
		}
	}

	private void OnPlayerAnimationChanged(PlayerInfo playerInfo, string animationName)
    {
        if (_player.Info.Id == playerInfo.Id)
		{
			State state = Enum.Parse<State>(animationName);
			EnterState(state, true);
        }
    }
	
	// If the puppet is moving, then update it's state
    public void EnterState(State state, bool isRpcUpdate)
	{
		if (state == _state) return;

		_state = state;

		_stateMachinePlayback.Travel($"{state}");
		_stateMachinePlayback.GetCurrentNode();

		if (MultiplayerManager.Peer != null && _multiplayerSynchronizer.GetMultiplayerAuthority() == Multiplayer.GetUniqueId() && !isRpcUpdate)
        {
			MultiplayerManager.Instance.Rpc(MultiplayerManager.MethodName.UpdatePlayerAnimation, _player.Info.Id, $"{state}");
        }
	}

	private void SprintSpeed()
	{
		const float DefaultSpeed = 1f;
		const float SprintSpeed = 0.5f;

		AnimationNodeAnimation walkState = (AnimationNodeAnimation) _stateMachince.GetNode("Walk");
		walkState.TimelineLength = _player.IsSprinting ? SprintSpeed : DefaultSpeed;
	}
}
