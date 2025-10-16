using Godot;

namespace BearBakery.Game;

public partial class PlayerStaminaInterface : Control
{
	[Export]
	private TextureProgressBar _textureProgressBar;

	public override void _Ready()
	{
		_textureProgressBar.Value = BearBakery.StaminaManager.MaxStamina;
		_textureProgressBar.MaxValue = BearBakery.StaminaManager.MaxStamina;

		BearBakery.Signals.StaminaFilled += Update;
	}

	private void Update(double value, double maxValue)
	{
		_textureProgressBar.Value = value;
		_textureProgressBar.MaxValue = maxValue;
	}
}
