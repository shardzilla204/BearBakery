using Godot;

namespace BearBakery;

public partial class MixerManager : Node
{
	[Export]
	private float _mixWaitTime = 3;

	public static float MixWaitTime = 3;
	
    public override void _EnterTree()
	{
		BearBakery.Signals.MixerOpened += AddMixerWindow;

		MixWaitTime = _mixWaitTime;
	}

	private void AddMixerWindow(MixerArea mixerArea)
	{
		MixerWindow mixerWindow = BearBakery.PackedScenes.GetMixerWindow(mixerArea);
		BearBakery.Signals.EmitSignal(Signals.SignalName.WindowOpened, mixerWindow);
	}
}