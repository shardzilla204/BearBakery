using Godot;

namespace BearBakery;

public partial class PlayerItem : Sprite2D
{
	[Export(PropertyHint.Range, "5, 15, 1")]
	private float _itemDistance = 5;

	[Export(PropertyHint.Range, "0.1, 1, 0.1")]
	private float _tweenDuration = 0.1f;

	private Vector2 _originalPosition;

	public override void _Ready()
	{
		_originalPosition = Position;
	}

	public override void _Process(double delta)
	{
		Vector2 targetPosition = _originalPosition + BearBakery.Player.Direction * _itemDistance;
		TweenPosition(targetPosition);
	}

	private void TweenPosition(Vector2 targetPosition)
	{
		Tween tween = CreateTween().SetTrans(Tween.TransitionType.Quad);
		tween.TweenProperty(this, "position", targetPosition, _tweenDuration);
	}
}
