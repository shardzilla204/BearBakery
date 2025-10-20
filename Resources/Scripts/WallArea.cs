using Godot;

namespace BearBakery;

public partial class WallArea : Area2D
{
	[Export]
	private TileMapLayer _layer;

	[Export]
	private CollisionShape2D _cameraBounds;

	public override void _Ready()
	{
		AreaEntered += (area) => OnAreaEntered();
		AreaExited += (area) => SetLayerOpacity(false);

		Visible = true; // Visibility is false in the editor
	}

	private void OnAreaEntered()
	{
		BearBakery.Signals.EmitSignal(Signals.SignalName.CameraBoundsChanged, _cameraBounds);
		SetLayerOpacity(true);
    }

	private void SetLayerOpacity(bool hasEntered)
	{
		Color color = Colors.White;

		color.A = hasEntered ? 0 : 1;
		TweenLayerOpacity(color, _layer);
	}

	private void TweenLayerOpacity(Color color, TileMapLayer layer)
	{
		Tween tween = GetTree().CreateTween().SetTrans(Tween.TransitionType.Quad);
		tween.TweenProperty(layer, "modulate", color, 0.5f);
	}
}
