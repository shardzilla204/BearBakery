using Godot;

public partial class AreaCamera : Camera2D
{
    [Export]
    private bool _isEnabled = false;

    [Export]
    private Area2D _area;

    public override void _Ready()
    {
        _area.AreaEntered += (area) => Enabled = true;
        _area.AreaExited += (area) => Enabled = false;
        
        Enabled = _isEnabled;
    }
}
