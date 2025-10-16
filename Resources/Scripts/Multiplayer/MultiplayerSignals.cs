using Godot;

namespace BearBakery;

public partial class MultiplayerSignals : Node
{
    [Signal]
    public delegate void PlayerCountUpdatedEventHandler();

    [Signal]
    public delegate void PlayerJoinedEventHandler(PlayerInfo playerInfo);

    [Signal]
    public delegate void PlayerLeftEventHandler(PlayerInfo playerInfo);

    [Signal]
    public delegate void PlayerKickedEventHandler();

    [Signal]
    public delegate void PlayerInformationUpdatedEventHandler(PlayerInfo playerInfo);

    [Signal]
    public delegate void PlayerAnimationChangedEventHandler(PlayerInfo playerInfo, string animationName);

    [Signal]
    public delegate void LobbyNamedUpdatedEventHandler(string lobbyName);

    [Signal]
    public delegate void ServerDisconnectedEventHandler();

    [Signal]
    public delegate void GameStartedEventHandler();
}