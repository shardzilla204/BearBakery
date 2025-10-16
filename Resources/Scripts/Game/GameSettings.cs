using Godot;

namespace BearBakery;

public partial class GameSettings : NinePatchRect
{
	[Export]
	private CustomButton _saveButton;

	[Export]
	private CustomButton _loadButton;

	[Export]
	private CustomButton _eraseButton;

	[Export]
	private CustomButton _quitButton;

	[Export]
	private QuitOptions _quitOptions;

	public override void _Ready()
	{
		_saveButton.Pressed += GameFileManager.Instance.Save;
		_loadButton.Pressed += GameFileManager.Instance.Load;
		_eraseButton.Pressed += GameFileManager.Instance.Erase;
		_quitButton.Pressed += () =>
		{
			_quitButton.Visible = false;
			_quitOptions.Visible = true;

			Timer timer = GetVisibilityTimer();
			AddChild(timer);
		};
	}

	private Timer GetVisibilityTimer()
	{
		Timer timer = new Timer()
		{
			WaitTime = 3f,
			Autostart = true
		};

		timer.Timeout += () => 
		{
			_quitButton.Visible = true;
			_quitOptions.Visible = false;
		};
		return timer;
	}
}
