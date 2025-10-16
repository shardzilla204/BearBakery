using Godot;

namespace BearBakery;

public partial class MixerInterface : Control
{
    [Export]
    private CustomButton _exitButton;

    [Export]
    private Container _mixerSlotContainer;

    private int _maxMixerSlotCount = 2;

    public override void _ExitTree()
    {
        BearBakery.Signals.MixerOpened -= AddMixerSlots;
        BearBakery.Signals.MixerClosed -= QueueFree;
    }

    public override void _Ready()
    {
        _exitButton.Pressed += () => BearBakery.Signals.EmitSignal(Signals.SignalName.MixerClosed);

        BearBakery.Signals.MixerOpened += AddMixerSlots;
        BearBakery.Signals.MixerClosed += QueueFree;

        // ClearMixerSlots();
    }

    // private void ClearMixerSlots()
    // {
    //     foreach (MixerSlot mixerSlot in _mixerSlotContainer.GetChildren())
    //     {
    //         mixerSlot.QueueFree();
    //     }
    // }

    private void AddMixerSlots(MixerArea mixerArea)
    {
        for (int i = 0; i < _maxMixerSlotCount; i++)
        {
            MixerSlot mixerSlot = BearBakery.PackedScenes.GetMixerSlot();
            _mixerSlotContainer.AddChild(mixerSlot);
        }
    }
}