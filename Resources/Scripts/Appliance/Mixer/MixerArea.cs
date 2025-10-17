using Godot;

namespace BearBakery;

public partial class MixerArea : ObjectArea
{
    public Bowl Bowl;
    public override void _Ready()
    {
        base._Ready();

        InteractComponent.Interacted += OpenMixer;
		InteractComponent.AreaExited += (area) => CloseMixer();
    }

    private void UseMixer()
    {
        // Item item = BearBakery.Player.Inventory.Items[0];

        // if (item is not Bowl bowl || Bowl == null) return;

        // Bowl = bowl;

        // BearBakery.Signals.EmitSignal(Signals.SignalName.ItemRemovedFromInventory, bowl);
    }

    private void OpenMixer()
	{
		HasInteracted = true;

		string openMessage = "Opening Mixer";
		PrintRich.PrintLine(TextColor.Blue, openMessage);

		BearBakery.Signals.EmitSignal(Signals.SignalName.MixerOpened, this);
	}

	/// <see cref="PackedScenes.GetMixerInterface">
	public void CloseMixer()
	{
		if (!HasInteracted) return;

		HasInteracted = false;

		string closeMessage = "Closing Mixer";
		PrintRich.PrintLine(TextColor.Blue, closeMessage);

		BearBakery.Signals.EmitSignal(Signals.SignalName.MixerClosed);
	}
}