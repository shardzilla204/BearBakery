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

	public static float MaxStamina = 100;
	public static float RechargeAmount = 100;
	public static float SprintCost = 1;
	public static float LeapCostPercent = 100;

	private static float _playerStamina;

    public override void _EnterTree()
    {
		MaxStamina = _maxStamina;
		RechargeAmount = _rechargeAmount;
		SprintCost = _sprintCost;
		LeapCostPercent = _leapCostPercent;

		_playerStamina = MaxStamina;
    }

	public static void Fill(float amount)
	{
		if (_playerStamina >= MaxStamina) return;

		_playerStamina += amount;
		_playerStamina = Mathf.Clamp(_playerStamina, 0, MaxStamina);

		BearBakery.Signals.EmitSignal(Signals.SignalName.StaminaFilled, _playerStamina, MaxStamina);
	}

	public static void Deplete(float amount)
	{
		if (IsDepleted()) return;

		_playerStamina -= amount;
		_playerStamina = Mathf.Clamp(_playerStamina, 0, MaxStamina);

		BearBakery.Signals.EmitSignal(Signals.SignalName.StaminaFilled, _playerStamina, MaxStamina);

		if (IsDepleted()) BearBakery.Signals.EmitSignal(Signals.SignalName.StaminaDepleted, 2);
	}

	public static bool IsDepleted()
	{
		return _playerStamina <= 0;
	}

	public static float GetLeapCost()
	{
		return MaxStamina * (LeapCostPercent / 100f);
	}

	public static bool CanLeap()
	{
		return _playerStamina - MaxStamina * (LeapCostPercent / 100f) > 0;
	}
}
