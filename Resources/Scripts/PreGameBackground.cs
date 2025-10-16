using Godot;

namespace BearBakery;

public partial class PreGameBackground : ParallaxBackground
{
    [Export] 
	private ParallaxLayer _backgroundParallaxLayer;

    [Export]
    private float _parallaxSpeed = 50f;
    
    public override void _Process(double delta)
    {
        float positionX = _backgroundParallaxLayer.MotionOffset.X - _parallaxSpeed * (float)delta;
        float positionY = _backgroundParallaxLayer.MotionOffset.Y - _parallaxSpeed * (float)delta;
        _backgroundParallaxLayer.MotionOffset = new Vector2(positionX, positionY);
    }
}
