using Godot;

namespace BearBakery;

public enum CanvasType
{
    Game = -1,
    Settings,
    Menu,
    Lobby
}

public partial class CanvasManager : Node
{
    public static CanvasType CanvasType;
    public static Node Canvas;

    public override void _Ready()
    {
        BearBakery.Signals.CanvasOpened += OnCanvasOpened;
    }

    private void OnCanvasOpened(CanvasType canvasType, Node canvas)
    {
        CanvasType = canvasType;
        Canvas = canvas;
    }
}