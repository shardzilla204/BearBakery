using Godot;

namespace BearBakery;

public partial class LowlightComponent : TextureRect
{
	private const float _LowlightAmount = 0.2f;

	public void Set(bool isHovering)
	{
		Color lowlight = Colors.Black;
		lowlight.A = isHovering ? _LowlightAmount : 0;
		TweenOpacity(lowlight);
	}

	private void TweenOpacity(Color targetColor)
	{        
		float duration = 0.20f;
		Tween tween = CreateTween().SetTrans(Tween.TransitionType.Quad);
		tween.TweenProperty(this, "self_modulate", targetColor, duration);
	}
}
