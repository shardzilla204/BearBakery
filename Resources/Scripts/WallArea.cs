using Godot;

namespace BearBakery;

public partial class WallArea : Node2D
{
	[Export]
	private TileMapLayer _layerOne;

	[Export]
	private TileMapLayer _layerTwo;

	[Export]
	private Area2D _area;

	public override void _Ready()
	{
		_area.AreaEntered += (area) => SetLayerColors(true);
		_area.AreaExited += (area) => SetLayerColors(false);

		Visible = true; // Visibility is false in the editor
	}

	private void SetLayerColors(bool hasEntered)
	{
		Color color = Colors.White;

		color.A = hasEntered ? 0 : 1;
		TweenLayerOpacity(color, _layerOne);

		color.A = hasEntered ? 0.5f : 1f;
		TweenLayerOpacity(color, _layerTwo);
	}

	private void TweenLayerOpacity(Color color, TileMapLayer layer)
	{
		Tween tween = GetTree().CreateTween().SetTrans(Tween.TransitionType.Quad);
		tween.TweenProperty(layer, "modulate", color, 0.5f);
	}
}
