using Godot;

namespace BearBakery;

public partial class PlayerStamina : Node2D
{
	[Export]
	private Timer _rechargeDelay;

	[Export]
	private Timer _rechargeTimer;

	private Player _player;
	private bool _canRecharge = true;

    public override void _EnterTree()
    {
        BearBakery.Signals.PlayerSprinting += Sprinting;
		BearBakery.Signals.PlayerLeaped += Leaping;
		BearBakery.Signals.StaminaDepleted += DelayRecharge;
    }

    public override void _Ready()
	{
		_player = GetParent<Player>();

		_rechargeDelay.Timeout += () => _canRecharge = true;
		_rechargeTimer.Timeout += Recharge;
	}

    private void Recharge()
	{
		if (!_canRecharge || _player.IsSprinting || _player.IsLeaping) return;

		StaminaManager.Fill(StaminaManager.RechargeAmount);
	}

	private void Sprinting(float delta)
	{
		if (!_canRecharge || !_player.IsSprinting) return;

		float sprintCost = delta * StaminaManager.SprintCost;
		StaminaManager.Deplete(sprintCost);
	}

	private void Leaping()
	{
		if (!_canRecharge || !_player.IsLeaping) return;

		StaminaManager.Deplete(StaminaManager.GetLeapCost());
		DelayRecharge(1);
	}

	private void DelayRecharge(float waitTime)
	{
		_canRecharge = false;

		_rechargeDelay.WaitTime = waitTime;
		_rechargeDelay.Start();
	}
}
