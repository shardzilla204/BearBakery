using Godot;

namespace BearBakery;

public partial class OvenWindow : Control
{
	[Export]
	private CustomButton _exitButton;

	[Export]
	private Container _ovenContainer;

    public override void _ExitTree()
    {
        BearBakery.Signals.OvenOpened -= AddOvenRacks;
		BearBakery.Signals.OvenClosed -= QueueFree;
    }

	public override void _Ready()
	{
		_exitButton.Pressed += () => BearBakery.Signals.EmitSignal(Signals.SignalName.OvenClosed);

		BearBakery.Signals.OvenOpened += AddOvenRacks;
		BearBakery.Signals.OvenClosed += QueueFree;
	}

	public void AddOvenRacks(OvenArea oven)
	{
		int ovenRackCount = oven.Racks.Count == 0 ? 1 : oven.Racks.Count;
		for (int i = 0; i < ovenRackCount; i++)
		{
			OvenRack ovenRack = BearBakery.PackedScenes.GetOvenRack();
			_ovenContainer.AddChild(ovenRack);
			oven.Racks.Add(ovenRack);
		}
	}
}
