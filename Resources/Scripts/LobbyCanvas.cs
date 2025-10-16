using System.Collections.Generic;
using Godot;

namespace BearBakery;

public partial class LobbyCanvas : Control
{
    [Export]
    private CustomButton _leaveButton;

    [Export]
    private CustomButton _startButton;

    [Export]
    private LineEdit _nameLineEdit;

    [Export]
    private Label _lobbyNameLabel;

    [Export]
    private Container _playerPlateContainer;

    private List<PlayerPlate> _playerPlates = new List<PlayerPlate>();

    private CanvasType _canvasType = CanvasType.Lobby;

    public override void _ExitTree()
    {
        MultiplayerManager.Signals.ServerDisconnected -= OnServerDisconnected;
        MultiplayerManager.Signals.PlayerJoined -= OnPlayerJoined;
        MultiplayerManager.Signals.PlayerLeft -= OnPlayerLeft;
        MultiplayerManager.Signals.PlayerKicked -= OnPlayerKicked;
        MultiplayerManager.Signals.PlayerInformationUpdated -= OnPlayerInformationUpdated;
        MultiplayerManager.Signals.LobbyNamedUpdated -= UpdateLobbyText;
        MultiplayerManager.Signals.GameStarted -= OnGameStarted;

		BearBakery.Signals.EmitSignal(Signals.SignalName.CanvasClosed, (int) _canvasType, this);
    }

    public override void _EnterTree()
    {
        MultiplayerManager.Signals.ServerDisconnected += OnServerDisconnected;
        MultiplayerManager.Signals.PlayerJoined += OnPlayerJoined;
        MultiplayerManager.Signals.PlayerLeft += OnPlayerLeft;
        MultiplayerManager.Signals.PlayerKicked += OnPlayerKicked;
        MultiplayerManager.Signals.PlayerInformationUpdated += OnPlayerInformationUpdated;
        MultiplayerManager.Signals.LobbyNamedUpdated += UpdateLobbyText;
        MultiplayerManager.Signals.GameStarted += OnGameStarted;

        BearBakery.Signals.EmitSignal(Signals.SignalName.CanvasOpened, (int) _canvasType, this);
    }

    public override void _Ready()
    {
        _leaveButton.Pressed += OnLeaveButtonPressed;
        _startButton.Pressed += OnStartButtonPressed;
        _nameLineEdit.TextSubmitted += OnNameLineEditTextSubmitted;

        AddPlayerPlates();

        PlayerInfo playerInfo = GameManager.PlayersInfo.Find(playerInfo => playerInfo.Id == Multiplayer.GetUniqueId());
        if (playerInfo != null)
        {
            SetLineEditText(playerInfo.Name);
            GetWindow().Title = playerInfo.Name;
        }

        _startButton.Visible = Multiplayer.IsServer();
    }

    private void OnServerDisconnected()
    {
        MenuCanvas menuCanvas = BearBakery.PackedScenes.GetMenuCanvas();
        GetTree().Root.AddChild(menuCanvas);
        QueueFree();
    }

    /// <summary>
    /// Adds a plate when a player joins a lobby
    /// </summary>
    /// <param name="id">The id they're given when they join the lobby</param>
    private void OnPlayerJoined(PlayerInfo playerInfo)
    {
        // GD.Print($"Player Plate ({playerInfo.Id}) Has Been Added To Client ({Player.Id})");
        PlayerPlate playerPlate = BearBakery.PackedScenes.GetPlayerPlate(playerInfo.Id, playerInfo.Name);
        _playerPlateContainer.AddChild(playerPlate);
        _playerPlates.Add(playerPlate);

        // Update line edit for the connected player
        if (Multiplayer.GetUniqueId() != playerInfo.Id) return;

        GetWindow().Title = playerInfo.Name;
        SetLineEditText(playerInfo.Name);
    }

    private void OnGameStarted()
    {
        GameCanvas gameCanvas = BearBakery.PackedScenes.GetGameCanvas();
        GetTree().Root.AddChild(gameCanvas);
        QueueFree();
    }

    /// <summary>
    /// Removes the plate associated with the player that left
    /// </summary>
    /// <param name="id">The id they're associated with</param>
    private void OnPlayerLeft(PlayerInfo playerInfo)
    {
        PlayerPlate playerPlate = _playerPlates.Find(plate => plate.Name == playerInfo.Id.ToString());
        playerPlate.QueueFree();

        _playerPlates.Remove(playerPlate);
    }

    private void OnPlayerKicked()
    {
        MenuCanvas menuCanvas = BearBakery.PackedScenes.GetMenuCanvas();
        GetTree().Root.AddChild(menuCanvas);
        QueueFree();
    }

    /// <summary>
    /// When the host leaves, move all the clients back to the main menu, and close the server. 
    /// When a client leaves, have them move back to the main menu.
    /// </summary>
    private async void OnLeaveButtonPressed()
    {
        if (Multiplayer.IsServer())
        {
            MultiplayerManager.Instance.DisconnectFromServer();

            MultiplayerManager.Peer.Close();
            Multiplayer.MultiplayerPeer.Close();
        }
        else
        {
            MultiplayerManager.Instance.RpcId(MultiplayerManager.HostId, MultiplayerManager.MethodName.RemovePlayerInformation, Multiplayer.GetUniqueId());
            MenuCanvas menuCanvas = BearBakery.PackedScenes.GetMenuCanvas();
            GetTree().Root.AddChild(menuCanvas);
            QueueFree();
        }
    }

    /// <summary>
    /// Starts the game
    /// </summary>
    private void OnStartButtonPressed()
    {
        GD.Print("Starting Game");
        MultiplayerManager.Instance.StartGame();
    }

    /// <summary>
    /// Updates the name on the player's plate and if they're the host. It will change the room name
    /// </summary>
    /// <param name="text"></param>
    private void OnNameLineEditTextSubmitted(string text)
    {
        PlayerInfo playerInfo = GameManager.PlayersInfo.Find(playerInfo => playerInfo.Id == Multiplayer.GetUniqueId());
        PlayerPlate playerPlate = _playerPlates.Find(playerPlate => playerPlate.Name == Multiplayer.GetUniqueId().ToString());
        playerPlate.SetText(text);

        playerInfo.Name = text;

        MultiplayerManager.Instance.Rpc(MultiplayerManager.MethodName.UpdatePlayerInformation, playerInfo.Id, playerInfo.Name);

        if (Multiplayer.IsServer())
        {
            string lobbyName = $"{text}'s Lobby";
            MultiplayerManager.Instance.Rpc(MultiplayerManager.MethodName.UpdateLobbyName, lobbyName);
        }
    }

    private void UpdateLobbyText(string lobbyName)
    {
        _lobbyNameLabel.Text = $"{lobbyName}";
    }

    private void AddPlayerPlates()
    {
        foreach (PlayerInfo playerInfo in GameManager.PlayersInfo)
        {
            PlayerPlate playerPlate = BearBakery.PackedScenes.GetPlayerPlate(playerInfo.Id, playerInfo.Name);
            _playerPlateContainer.AddChild(playerPlate);
            _playerPlates.Add(playerPlate);
        }
    }

    private void OnPlayerInformationUpdated(PlayerInfo playerInfo)
    {
        PlayerPlate playerPlate = _playerPlates.Find(playerPlate => playerPlate.Name == playerInfo.Id.ToString());
        playerPlate.SetText(playerInfo.Name);
    }

    private void SetLineEditText(string text)
    {
        _nameLineEdit.Text = $"{text}";
    }
    
    private void ClearPlayerPlatesContainer()
    {
        foreach (PlayerPlate playerPlate in _playerPlateContainer.GetChildren())
        {
            playerPlate.QueueFree();
        }
        _playerPlates.Clear();
    }
}
