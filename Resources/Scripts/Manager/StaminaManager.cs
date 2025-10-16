using Godot;

namespace BearBakery;

public partial class StaminaManager : Node
{
	[Export]
	private float _maxStamina = 100;

	[Export]
	private float _rechargeAmount = 1.5f;

	[Export]
	private float _sprintCost = 1;

	[Export]
	private float _leapCostPercent = 25f;

	public float MaxStamina = 100;
	public float RechargeAmount = 100;
	public float SprintCost = 1;
	public float LeapCostPercent = 100;

	private float _playerStamina;

    public override void _EnterTree()
    {
		BearBakery.StaminaManager = this;

		MaxStamina = _maxStamina;
		RechargeAmount = _rechargeAmount;
		SprintCost = _sprintCost;
		LeapCostPercent = _leapCostPercent;

		_playerStamina = MaxStamina;
    }

	public void Fill(float amount)
	{
		if (IsFilled()) return;

		_playerStamina += amount;
		_playerStamina = Mathf.Clamp(_playerStamina, 0, MaxStamina);

		BearBakery.Signals.EmitSignal(Signals.SignalName.StaminaFilled, _playerStamina, MaxStamina);
	}

	private bool IsFilled()
	{
		return _playerStamina >= MaxStamina ? true : false;
	}

	public void Deplete(float amount)
	{
		if (IsDepleted()) return;

		_playerStamina -= amount;
		_playerStamina = Mathf.Clamp(_playerStamina, 0, MaxStamina);

		BearBakery.Signals.EmitSignal(Signals.SignalName.StaminaFilled, _playerStamina, MaxStamina);

		if (IsDepleted()) BearBakery.Signals.EmitSignal(Signals.SignalName.StaminaDepleted, 2);
	}

	public bool IsDepleted()
	{
		return _playerStamina <= 0 ? true : false;
	}

	public float GetLeapCost()
	{
		return MaxStamina * (LeapCostPercent / 100f);
	}

	public bool CanLeap()
	{
		return _playerStamina - MaxStamina * (LeapCostPercent / 100f) > 0 ? true : false;
	}
}
