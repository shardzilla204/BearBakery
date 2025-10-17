using Godot;
using Godot.Collections;
using System;

namespace BearBakery;

public partial class MixerSlot : HBoxContainer
{
    [Export]
    private TextureProgressBar _mixingBar;

    [Export]
    private SlotComponent _slotComponent;

    private Timer _mixTimer;

    private bool _isDragging = false;

    private Bowl _bowl;

    public override void _Ready()
    {
        AddMixTimer();
    }
    
    public override void _Process(double delta)
    {
        if (!_mixTimer.IsStopped()) 
		{
			UpdateMixingBar(OvenManager.CookColor);
		}
    }

    public override void _Notification(int what)
	{
		if (!_isDragging || what != NotificationDragEnd) return;
        _slotComponent.SetItem(null);
        _mixingBar.Value = 0; 
		_isDragging = false;
    }
    
    public override Variant _GetDragData(Vector2 atPosition)
    {
        _isDragging = true;

        Control dragPreview = GameManager.GetDragPreview(_bowl);
        SetDragPreview(dragPreview);

        Dictionary<string, Variant> dragDictionary = new Dictionary<string, Variant>()
        {
            { "Origin", this },
            { "Item", _bowl }
        };

        return dragDictionary;
    }

    public override bool _CanDropData(Vector2 atPosition, Variant data)
    {
		try
		{
			Dictionary<string, Variant> dragDictionary = data.As<Dictionary<string, Variant>>();
			Bowl bowl = dragDictionary["Item"].As<Bowl>();

			return _bowl == null;
		}
		catch (NullReferenceException)
		{
			Item item = data.As<Item>();
			string errorMessage = $"{item.Name} Is Not A Bowl";
			PrintRich.Print(TextColor.Red, errorMessage);

			return false;
		}
    }

    public override void _DropData(Vector2 atPosition, Variant data)
    {
        Dictionary<string, Variant> dragDictionary = data.As<Dictionary<string, Variant>>();
        Bowl bowl = dragDictionary["Item"].As<Bowl>();
        _bowl = bowl;

        _slotComponent.SetItem(_bowl);
        if (RecipeManager.HasRecipe(_bowl.Ingredients))
        {
            Ingredient product = RecipeManager.GetProduct(_bowl.Ingredients);
            if (product == null)
            {
                string errorMessage = $"Product Is Null";
                PrintRich.Print(TextColor.Red, errorMessage);
                return;
            }

            _mixTimer.Start();

            string startMixingMessage = $"Started Mixing Ingredients For {product.Name}";
            PrintRich.PrintLine(TextColor.Blue, startMixingMessage);
        }
        else
        {
            string errorMessage = $"Recipe Does Not Exist";
            PrintRich.Print(TextColor.Red, errorMessage);
        }

        BearBakery.Signals.EmitSignal(Signals.SignalName.ItemRemovedFromInventory, _bowl);
    }

    private void UpdateMixingBar(Color color)
	{
		_mixingBar.Value = _mixTimer.WaitTime - _mixTimer.TimeLeft;
		_mixingBar.TintProgress = color;
	}

    private void FinishMixing()
    {
        _bowl.MixIngredients();

        string finishMixingMessage = $"Finished Mixing Ingredients";
        PrintRich.PrintLine(TextColor.Blue, finishMixingMessage);
    }
    
    private void AddMixTimer()
    {
        Timer mixTimer = new Timer()
        {
            WaitTime = MixerManager.MixWaitTime,
            OneShot = true
        };
        mixTimer.Timeout += FinishMixing;
        _mixingBar.MaxValue = MixerManager.MixWaitTime;

        AddChild(mixTimer);
        _mixTimer = mixTimer;
    }
}
