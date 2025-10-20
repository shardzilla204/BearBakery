using Godot;

namespace BearBakery;

public partial class InteractComponent : Area2D
{
	[Signal]
	public delegate void InteractedEventHandler();

	[Signal]
	public delegate void SecondaryActionEventHandler();

	private bool _canInteract = false;
	
	public bool IsInteracting = false;

	public override void _Ready()
	{
		AreaEntered += (area) => AreaDetection(true);
		AreaExited += (area) => AreaDetection(false);
	}

    public override void _Process(double delta)
    {
		// * Actions = { E }
		if (Input.IsActionJustPressed("Interact") && _canInteract) EmitSignal(SignalName.Interacted);
		if (Input.IsActionJustPressed("SecondaryAction") && _canInteract) EmitSignal(SignalName.SecondaryAction);
    }

    private void AreaDetection(bool value)
	{
		_canInteract = value;
	}
}
