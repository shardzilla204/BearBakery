using Godot;

public partial class CustomButton : Button
{
	[Signal]
	public delegate void ExitedEventHandler();
	
	[Export]
	private NinePatchRect _texture;

	private Color _textureColor;

	public override void _Ready()
	{
		_textureColor = _texture.SelfModulate;
		MouseEntered += () => 
		{
			_texture.SelfModulate = _textureColor.Darkened(0.2f);
		};
		MouseExited += () =>
		{
			_texture.SelfModulate = _textureColor;
		};
	}
}
