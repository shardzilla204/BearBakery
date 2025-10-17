using Godot;
using System.Collections.Generic;

namespace BearBakery;

public partial class OvenArea : ObjectArea
{
	public List<Timer> Timers = new List<Timer>();
	public List<OvenRack> Racks = new List<OvenRack>();
	public List<OvenSlot> Slots = new List<OvenSlot>();

	public override void _EnterTree()
	{
		OvenManager.Ovens.Add(this);
	}

	public override void _Ready()
	{
		base._Ready();

		InteractComponent.Interacted += OpenOven;
		InteractComponent.AreaExited += (area) => CloseOven();

		// SetOvenTimers();
	}

	private void OpenOven()
	{
		HasInteracted = true;

		string openMessage = "Opening Oven";
		PrintRich.PrintLine(TextColor.Blue, openMessage);

		BearBakery.Signals.EmitSignal(Signals.SignalName.OvenOpened, this);
	}

	/// <see cref="PackedScenes.GetOvenInterface">
	public void CloseOven()
	{
		if (!HasInteracted) return;

		HasInteracted = false;

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
