using Godot;
using System.Collections.Generic;

namespace BearBakery;

public partial class OvenArea : Sprite2D
{
	[Export]
	private InteractComponent _interactComponent;

	[Export]
	private LowlightComponent _lowlightComponent;

	public List<Timer> Timers = new List<Timer>();
	public List<OvenRack> Racks = new List<OvenRack>();
	public List<OvenSlot> Slots = new List<OvenSlot>();

	private bool _hasInteracted = false;

	public override void _EnterTree()
	{
		OvenManager.Ovens.Add(this);
	}

	public override void _Ready()
	{
		_interactComponent.Interacted += OpenOven;
		_interactComponent.AreaExited += (area) => CloseOven();

		// SetOvenTimers();
	}

	private void OpenOven()
	{
		_hasInteracted = true;

		string openMessage = "Opening Oven";
		PrintRich.PrintLine(TextColor.Blue, openMessage);

		BearBakery.Signals.EmitSignal(Signals.SignalName.OvenOpened, this);
	}

	/// <see cref="PackedScenes.GetOvenInterface">
	public void CloseOven()
	{
		if (!_hasInteracted) return;

		_hasInteracted = false;

		string closeMessage = "Closing Oven";
		PrintRich.PrintLine(TextColor.Blue, closeMessage);

		BearBakery.Signals.EmitSignal(Signals.SignalName.OvenClosed);
	}

	private void SetOvenTimers()
	{
		foreach (OvenRack rack in Racks)
		{
			foreach (OvenSlot slot in rack.Slots)
			{
				Timer timer = new Timer();
				Timers.Add(timer);
				// slot.Timer = timer;
			}
		}
	}
}
