using Godot;

namespace BearBakery;

public partial class PlayerCamera : Camera2D
{
    [Export]
    private MultiplayerSynchronizer _multiplayerSynchronizer;

    public override void _ExitTree()
    {
        BearBakery.Signals.CameraBoundsChanged -= OnCameraBoundsChanged;
    }

    public override void _EnterTree()
    {
        BearBakery.Signals.CameraBoundsChanged += OnCameraBoundsChanged;
    }

    public override async void _Ready()
    {
        Player player = GetParent<Player>();
        await ToSignal(player, Node.SignalName.Ready);

        if (MultiplayerManager.Peer != null)
        {
            Enabled = _multiplayerSynchronizer.GetMultiplayerAuthority() == Multiplayer.GetUniqueId();
        }
    }

    // Get the global position
    // half the scale
    // multiply the scale with the size of the shape
    // go in all directions using the product 
    // -X = Left
    // -Y = Top
    // X = Right
    // Y = Bottom

    // When moving to another room, append the size of the room with the length that is longer than that side
    // Afterwards transition to the target size

    // Get the current camera limts 
    // Get the next camera limts through the camera bounds
    // Compare between correseponding values 
    // Append limits that are larger
    // Store the unused limits as target limits
    // Use a tween to those target limits
    private void OnCameraBoundsChanged(CollisionShape2D cameraBounds)
    {
        LimitLeft = GetTargetLimits(cameraBounds).Left;
        LimitTop =  GetTargetLimits(cameraBounds).Top;
        LimitRight =  GetTargetLimits(cameraBounds).Right;
        LimitBottom =  GetTargetLimits(cameraBounds).Bottom;
    }

    private Vector2 GetOffset(CollisionShape2D cameraBounds)
    {
        RectangleShape2D rectangle = (RectangleShape2D)cameraBounds.Shape;
        Vector2 rectangleSize = rectangle.Size;
        Vector2 scale = cameraBounds.Scale;
        float offsetX = rectangleSize.X * (scale.X / 2);
        float offsetY = rectangleSize.Y * (scale.Y / 2);
        Vector2 offset = new Vector2(offsetX, offsetY);

        return offset;
    }

    private (int Left, int Top, int Right, int Bottom) GetTargetLimits(CollisionShape2D cameraBounds)
    {
        Vector2 offset = GetOffset(cameraBounds);

        int limitLeft = (int)(cameraBounds.GlobalPosition.X - offset.X);
        int limitTop = (int)(cameraBounds.GlobalPosition.Y - offset.Y);
        int limitRight = (int)(cameraBounds.GlobalPosition.X + offset.X);
        int limitBottom = (int)(cameraBounds.GlobalPosition.Y + offset.Y);

        return (limitLeft, limitTop, limitRight, limitBottom);
    }

    private void TweenCameraBounds(CollisionShape2D cameraBounds)
    {
        (int targetLeft, int targetTop, int targetRight, int targetBottom) = GetTargetLimits(cameraBounds);

        float duration = 1f;
        Tween tween = CreateTween().SetParallel(true).SetTrans(Tween.TransitionType.Quad);
        tween.TweenProperty(this, "limit_left", targetLeft, duration);
        tween.TweenProperty(this, "limit_top", targetTop, duration);
        tween.TweenProperty(this, "limit_right", targetRight, duration);
        tween.TweenProperty(this, "limit_bottom", targetBottom, duration);
    }
}
