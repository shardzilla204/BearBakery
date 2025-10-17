using Godot;

namespace BearBakery;

public partial class SwapComponent : Area2D
{
    [Signal]
    public delegate void SwappedEventHandler();

    public bool CanSwap = false;
    public bool IsSwapping = false;

    public override void _Ready()
    {
        // AreaEntered += (area) => AreaDetection(true);
        // AreaExited += (area) => AreaDetection(false);
    }

    public override void _Process(double delta)
    {
        // * Actions = { Q }
        if (Input.IsActionJustPressed("Swap") && CanSwap) EmitSignal(SignalName.Swapped);
    }
}