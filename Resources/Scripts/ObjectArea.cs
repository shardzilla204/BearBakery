using Godot;

namespace BearBakery;

public partial class ObjectArea : Sprite2D
{
    [Export]
    private InteractComponent _interactComponent;

    public InteractComponent InteractComponent => _interactComponent;

	public bool HasInteracted = false;

    public override void _Ready()
	{
		InteractComponent.AreaEntered += (area) => ChangeLowlight(true);
		InteractComponent.AreaExited += (area) => ChangeLowlight(false);
	}

    private void ChangeLowlight(bool isHovering)
	{
		Color lowlight = Colors.Black;
		lowlight.A = isHovering ? 0.15f : 0;
		TweenOpacity(lowlight);
	}

	private void TweenOpacity(Color targetColor)
	{
		Tween tween = CreateTween().SetTrans(Tween.TransitionType.Quad);
		tween.TweenProperty(this, "self_modulate", targetColor, 0.25f);
	}
}
