using System.Linq;
using Godot;

namespace BearBakery;

public partial class MultiplayerController : Control
{
    [Export]
    private CustomButton _hostButton;

    [Export]
    private CustomButton _joinButton;

    public override void _Ready()
    {
        _hostButton.Pressed += OnHostButtonPressed;
        _joinButton.Pressed += OnJoinButtonPressed;

        if (OS.GetCmdlineArgs().Contains("--server"))
        {
            HostGame();
        }
    }

    /// <summary>
    /// Creates a server with the provided server port id
    /// </summary>
    private void OnHostButtonPressed()
    {
        HostGame();
        MultiplayerManager.Instance.AddPlayerInformation(Multiplayer.GetUniqueId());
    }

    /// <summary>
    /// Finds an available server through the given server port id
    /// </summary>
    private void OnJoinButtonPressed()
    {
        MultiplayerManager.Peer = new ENetMultiplayerPeer();
        MultiplayerManager.Peer.CreateClient(MultiplayerManager.Instance.Address, MultiplayerManager.Instance.ServerPortId);

        MultiplayerManager.Peer.Host.Compress(MultiplayerManager.Instance.CompressionMode);
        Multiplayer.MultiplayerPeer = MultiplayerManager.Peer;

        string message = "Joining Game";
        PrintRich.PrintLine(TextColor.Pink, message);
    }

    private void HostGame()
    {
        MultiplayerManager.Peer = new ENetMultiplayerPeer();
        Error error = MultiplayerManager.Peer.CreateServer(MultiplayerManager.Instance.ServerPortId);

        string message;
        if (error != Error.Ok)
        {
            message = $"Error: {error}";
            GD.PrintErr(message);
            return;
        }
        
        MultiplayerManager.Peer.Host.Compress(MultiplayerManager.Instance.CompressionMode);

        Multiplayer.MultiplayerPeer = MultiplayerManager.Peer;
        message = "Waiting For Players...";
        PrintRich.PrintLine(TextColor.Pink, message);
    }
}
