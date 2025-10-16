using Godot;

namespace BearBakery;

public partial class MultiplayerManager : Node
{
    public static MultiplayerManager Instance { get; private set; }

    [Export]
    public int ServerPortId = 8910;

    [Export]
    public string Address = "127.0.0.1";

    [Export]
    public ENetConnection.CompressionMode CompressionMode = ENetConnection.CompressionMode.RangeCoder;
    
    public static ENetMultiplayerPeer Peer;
    public static int HostId = 1;

    public static int PlayerCount = 0;

    public static MultiplayerSignals Signals = new MultiplayerSignals();

    public override void _EnterTree()
    {
        Instance = this;
    }

    public override void _Ready()
    {
        Multiplayer.ServerDisconnected += OnServerDisconnected;
        Multiplayer.PeerConnected += OnPeerConnected;
        Multiplayer.PeerDisconnected += OnPeerDisconnected;
        Multiplayer.ConnectedToServer += OnConnectedToServer;
        Multiplayer.ConnectionFailed += OnConnectionFailed;
    }

    /// <summary>
    /// Runs when the connection is successful and only runs on the client
    /// </summary>
    private async void OnConnectedToServer()
    {
        string message = "Connected To Server";
        PrintRich.PrintLine(TextColor.Pink, message);
        GD.Print($"UniqueID {Multiplayer.GetUniqueId()}");

        RpcId(HostId, MethodName.AddPlayerInformation, Multiplayer.GetUniqueId());
        // await ToSignal(Signals, MultiplayerSignals.SignalName.PlayerJoined);

        // Show host's lobby
        LobbyCanvas lobbyCanvas = BearBakery.PackedScenes.GetLobbyCanvas();
        GetTree().Root.AddChild(lobbyCanvas);
    }

    /// <summary>
    /// Runs when the connection fails and it runs only on the client
    /// </summary>
    private void OnConnectionFailed()
    {
        string message = "Connection Failed";
        PrintRich.PrintLine(TextColor.Pink, message);
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnServerDisconnected()
    {
        PrintRich.PrintLine(TextColor.Pink, "Server Has Been Disconnected");
    }
    
    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    public void DisconnectFromServer()
    {
        ClearPlayerInformation();

        if (Multiplayer.IsServer())
        {
            Rpc(MethodName.DisconnectFromServer);
        }

        Signals.EmitSignal(MultiplayerSignals.SignalName.ServerDisconnected);
    }

    /// <summary>
    /// Runs when a player connects and runs on all peers
    /// </summary>
    /// <param name="id"> id of the player that connected </param>
    private async void OnPeerConnected(long id)
    {
        string message = $"Player ({id}): Connected To {Multiplayer.GetUniqueId()}";
        PrintRich.PrintLine(TextColor.Pink, message);
    }

    /// <summary>
    /// Runs when a player disconnects and runs on all peers
    /// </summary>
    /// <param name="id"> Id of the player that disconnected </param>
    private async void OnPeerDisconnected(long id)
    {
        string message = $"Player ({id}): Disconnected From {Multiplayer.GetUniqueId()}";
        PrintRich.PrintLine(TextColor.Pink, message);
    }

    /// <summary>
    /// Starts the game for all peers
    /// </summary>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    public void StartGame()
    {
        if (Multiplayer.IsServer())
        {
            Rpc(MethodName.StartGame);
        }

        Signals.EmitSignal(MultiplayerSignals.SignalName.GameStarted);
    }

    /// <summary>
    /// Shares information of the player that joined on all peers
    /// </summary>
    /// <param name="id"></param>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    public void AddPlayerInformation(int id)
    {
        PlayerInfo targetPlayerInfo = GameManager.PlayersInfo.Find(player => player.Id == id);
        if (targetPlayerInfo != null) return;

        PlayerInfo playerInfo = new PlayerInfo()
        {
            Id = id,
            Name = $"Player {GameManager.PlayersInfo.Count + 1}"
        };
        
        GameManager.PlayersInfo.Add(playerInfo);

        GD.Print($"Player ({playerInfo.Id}) Added To Client ({Multiplayer.GetUniqueId()}):\n\tName: {playerInfo.Name}");
        GD.Print();

        if (Multiplayer.IsServer())
        {
            PrintRich.PrintServer();
            foreach (PlayerInfo player in GameManager.PlayersInfo)
            {
                Rpc(MethodName.AddPlayerInformation, player.Id);
            }
        }

        Signals.EmitSignal(MultiplayerSignals.SignalName.PlayerJoined, playerInfo);
    }

    /// <summary>
    /// Find the information of the player that left and removes it on all peers
    /// </summary>
    /// <param name="id"></param>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    public void RemovePlayerInformation(int id)
    {
        PlayerInfo targetPlayerInfo = GameManager.PlayersInfo.Find(player => player.Id == id);
        if (targetPlayerInfo == null) return;

        GameManager.PlayersInfo.Remove(targetPlayerInfo);

        GD.Print($"Player ({targetPlayerInfo.Id}) Removed To Client ({Multiplayer.GetUniqueId()}):\n\tName: {targetPlayerInfo.Name}");
        GD.Print();

        if (Multiplayer.IsServer())
        {
            PrintRich.PrintServer();
            foreach (PlayerInfo player in GameManager.PlayersInfo)
            {
                Rpc(MethodName.RemovePlayerInformation, id);
            }
        }

        Signals.EmitSignal(MultiplayerSignals.SignalName.PlayerLeft, targetPlayerInfo);
    }

    /// <summary>
    /// Clears all the player information for all peers whenever the host leaves the lobby
    /// </summary>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    private void ClearPlayerInformation()
    {
        GameManager.PlayersInfo.Clear();
        if (Multiplayer.IsServer())
        {
            PrintRich.PrintServer();
            Rpc(MethodName.ClearPlayerInformation);
        }
    }

    /// <summary>
    /// Update the player's name whenever they change their name
    /// </summary>
    /// <param name="id"></param>
    /// <param name="newName"></param>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    private void UpdatePlayerInformation(int id, string newName)
    {
        PlayerInfo targetPlayerInfo = GameManager.PlayersInfo.Find(player => player.Id == id);
        targetPlayerInfo.Name = newName;

        Signals.EmitSignal(MultiplayerSignals.SignalName.PlayerInformationUpdated, targetPlayerInfo);
    }

    /// <summary>
    /// Changes the lobby name based on the host's name
    /// </summary>
    /// <param name="lobbyName"></param>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    private void UpdateLobbyName(string lobbyName)
    {
        Signals.EmitSignal(MultiplayerSignals.SignalName.LobbyNamedUpdated, lobbyName);
    }

    /// <summary>
    /// Kicks out the player specifed by the id
    /// </summary>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    private void KickPlayer()
    {
        Signals.EmitSignal(MultiplayerSignals.SignalName.PlayerKicked);
        Multiplayer.EmitSignal(MultiplayerApi.SignalName.PeerDisconnected, Multiplayer.GetUniqueId());
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    private void UpdatePlayerAnimation(int id, string animationName)
    {
        PlayerInfo playerInfo = GameManager.PlayersInfo.Find(playerInfo => playerInfo.Id == id);
        Signals.EmitSignal(MultiplayerSignals.SignalName.PlayerAnimationChanged, playerInfo, animationName);
    }
}
