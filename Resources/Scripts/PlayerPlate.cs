using Godot;

namespace BearBakery;

public partial class PlayerPlate : Control
{
    [Export]
    private Label _nameLabel;

    [Export]
    private CustomButton _kickButton;

    public override void _Ready()
    {
        _kickButton.Pressed += OnKickButtonPressed;

        // Only have the host be able to kick people
        _kickButton.Visible = Multiplayer.IsServer() && int.Parse(Name) != MultiplayerManager.HostId;
    }

    public void SetText(string text)
    {
        _nameLabel.Text = $"{text}";
    }
    
    /// <summary>
    /// Kicks the player associated with the plate, from the lobby 
    /// </summary>
    private void OnKickButtonPressed()
    {
        MultiplayerManager.Instance.RpcId(int.Parse(Name), MultiplayerManager.MethodName.KickPlayer);
    }
}
