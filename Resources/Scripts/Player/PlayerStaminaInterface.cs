using Godot;

namespace BearBakery.Game;

public partial class PlayerStaminaInterface : Control
{
	[Export]
	private TextureProgressBar _textureProgressBar;

	public override void _Ready()
	{
		_textureProgressBar.Value = StaminaManager.MaxStamina;
		_textureProgressBar.MaxValue = StaminaManager.MaxStamina;

		BearBakery.Signals.StaminaFilled += Update;
	}

	private void Update(double value, double maxValue)
	{
		_textureProgressBar.Value = value;
		_textureProgressBar.MaxValue = maxValue;
	}
}
