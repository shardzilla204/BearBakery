using Godot;

namespace BearBakery;

public partial class PlayerCamera : Camera2D
{
    [Export]
    private MultiplayerSynchronizer _multiplayerSynchronizer;

    public override async void _Ready()
    {
        Player player = GetParent<Player>();
        await ToSignal(player, Node.SignalName.Ready);

        if (MultiplayerManager.Peer != null)
		{
            Enabled = _multiplayerSynchronizer.GetMultiplayerAuthority() == Multiplayer.GetUniqueId();
        }
    }
}
