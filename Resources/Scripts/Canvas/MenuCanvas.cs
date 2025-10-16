using Godot;

namespace BearBakery;

public partial class MenuCanvas : Control
{
	[Export]
	private CustomButton _startButton;

	[Export]
	private CustomButton _hostButton;

	[Export]
	private CustomButton _joinButton;

	[Export]
	private CustomButton _settingsButton;

	[Export]
	private CustomButton _quitButton;

	private CanvasType _canvasType = CanvasType.Menu;

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
		_startButton.Pressed += OnStartButtonPressed;
		_hostButton.Pressed += OnHostButtonPressed;
		_joinButton.Pressed += QueueFree;

		_settingsButton.Pressed += OnSettingsButtonPressed;
		_quitButton.Pressed += OnQuitButtonPressed;

		// Reset multiplayer values
		MultiplayerManager.PlayerCount = 0;
		GameManager.PlayersInfo.Clear();
	}

	/// <summary>
	/// Shows the game
	/// </summary>
	private void OnStartButtonPressed()
	{
		GameCanvas gameCanvas = BearBakery.PackedScenes.GetGameCanvas();
		AddSibling(gameCanvas);
		QueueFree();
	}

	/// <summary>
	/// Shows the lobby for the host
	/// </summary>
	private void OnHostButtonPressed()
	{
		LobbyCanvas lobbyCanvas = BearBakery.PackedScenes.GetLobbyCanvas();
		AddSibling(lobbyCanvas);
		QueueFree();
	}

	/// <summary>
	/// Shows the settings
	/// </summary>
	private void OnSettingsButtonPressed()
	{
		SettingsCanvas settingsCanvas = BearBakery.PackedScenes.GetSettingsCanvas();
		AddSibling(settingsCanvas);
		QueueFree();
	}
	
	/// <summary>
    /// Quits the game
    /// </summary>
	private void OnQuitButtonPressed()
    {
        GetTree().Quit();
    }
}
