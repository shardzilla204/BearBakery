using Godot;

namespace BearBakery;

public partial class SettingsCanvas : Node
{
	[Export]
	private Button _exitButton;

	[Export]
	private GameSettings _gameSettings;

	private CanvasType _canvasType = CanvasType.Settings;

	public override void _ExitTree()
	{
		BearBakery.Signals.EmitSignal(Signals.SignalName.CanvasClosed, (int) _canvasType, this);
	}

    public override void _EnterTree()
    {
		BearBakery.Signals.EmitSignal(Signals.SignalName.CanvasOpened, (int) _canvasType, this);
    }

	public override void _Ready()
	{
		_exitButton.Pressed += QueueFree;

		_gameSettings.Visible = CanvasManager.CanvasType == CanvasType.Game;
	}
}
