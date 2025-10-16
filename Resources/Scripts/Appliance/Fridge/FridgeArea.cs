using Godot;
using System.Collections.Generic;

namespace BearBakery;

public partial class FridgeArea : ObjectArea
{
	public override void _Ready()
	{
		base._Ready();

		InteractComponent.Interacted += OpenFridge;
		InteractComponent.AreaExited += (area) => CloseFridge();
	}

    private void OpenFridge()
	{
		HasInteracted = true;

		string openMessage = "Opening Fridge";
		PrintRich.PrintLine(TextColor.Blue, openMessage);

		BearBakery.Signals.EmitSignal(Signals.SignalName.FridgeOpened, this);
	}

	/// <see cref="PackedScenes.GetFridgeInterface">
	public void CloseFridge()
	{
		if (!HasInteracted) return;

		HasInteracted = false;

		string closeMessage = "Closing Fridge";
		PrintRich.PrintLine(TextColor.Blue, closeMessage);

		PrintRich.PrintFridge();

		BearBakery.Signals.EmitSignal(Signals.SignalName.FridgeClosed);
	}
}
