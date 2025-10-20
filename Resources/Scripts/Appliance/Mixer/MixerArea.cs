using Godot;

namespace BearBakery;

public partial class MixerArea : Sprite2D
{
	[Export]
	private InteractComponent _interactComponent;

	[Export]
	private LowlightComponent _lowlightComponent;

	public Bowl Bowl;

	private bool _hasInteracted = false;
	
    public override void _Ready()
    {
        base._Ready();

        _interactComponent.Interacted += OpenMixer;
		_interactComponent.AreaExited += (area) => CloseMixer();
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
		_hasInteracted = true;

		string openMessage = "Opening Mixer";
		PrintRich.PrintLine(TextColor.Blue, openMessage);

		// BearBakery.Signals.EmitSignal(Signals.SignalName.Mixer, this);
	}

	/// <see cref="PackedScenes.GetMixerInterface">
	public void CloseMixer()
	{
		if (!_hasInteracted) return;

		_hasInteracted = false;

		string closeMessage = "Closing Mixer";
		PrintRich.PrintLine(TextColor.Blue, closeMessage);

		BearBakery.Signals.EmitSignal(Signals.SignalName.MixerClosed);
	}
}