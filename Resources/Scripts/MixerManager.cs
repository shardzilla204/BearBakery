using Godot;

namespace BearBakery;

public partial class MixerManager : Node
{
	[Export]
	private float _mixWaitTime = 3;

	public float MixWaitTime = 3;
	
    public override void _EnterTree()
	{
		BearBakery.MixerManager = this;
		MixWaitTime = _mixWaitTime;

		BearBakery.Signals.MixerOpened += AddMixerInterface;
	}

	private void AddMixerInterface(MixerArea mixerArea)
	{
		MixerInterface mixerInterface = BearBakery.PackedScenes.GetMixerInterface(mixerArea);
		BearBakery.GameManager.Interface.AddInterface(mixerInterface);
	}
}