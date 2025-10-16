using Godot;
using Godot.Collections;
using System;

namespace BearBakery;

public partial class OvenSlot : HBoxContainer
{
	[Export]
	private TextureProgressBar _cookingBar;

	[Export]
	private SlotComponent _slotComponent;

	private Timer _cookTimer;
	private bool _isCooking = false;

	private Timer _burnTimer;
	private bool _isBurning = false;

	private bool _isDragging = false;

	private Ingredient _ingredient;

    public override void _Ready()
	{
		AddCookTimer();
		AddBurnTimer();
	}
	
    public override void _Process(double delta)
    {
        if (_isCooking) 
		{
			UpdateCookingBar(_cookTimer, BearBakery.OvenManager.CookColor);
		}
		else if (_isBurning) 
		{
			UpdateCookingBar(_burnTimer, BearBakery.OvenManager.BurnColor);
		}
    }
	
	public override void _Notification(int what)
	{
		if (!_isDragging || what != NotificationDragEnd) return;
		_isDragging = false;
    }

    public override Variant _GetDragData(Vector2 atPosition)
	{
		_isDragging = true;

		Control dragPreview = BearBakery.GameManager.GetDragPreview(_ingredient);
		SetDragPreview(dragPreview);

		Dictionary<string, Variant> dragDictionary = new Dictionary<string, Variant>()
		{
			{ "Origin", this },
			{ "Item", _ingredient }
		};

		return dragDictionary;
	}

	public override bool _CanDropData(Vector2 atPosition, Variant data)
    {
		try
		{
			Dictionary<string, Variant> dragDictionary = data.As<Dictionary<string, Variant>>();
			Ingredient ingredient = dragDictionary["Item"].As<Ingredient>();

			return _ingredient == null;
		}
		catch (NullReferenceException)
		{
			Item item = data.As<Item>();
			string errorMessage = $"{item.Name} Is Not An Ingredient";
			PrintRich.Print(TextColor.Red, errorMessage);

			return false;
		}
    }

	public override void _DropData(Vector2 atPosition, Variant data)
	{
		Dictionary<string, Variant> dragDictionary = data.As<Dictionary<string, Variant>>();
		Ingredient ingredient = dragDictionary["Item"].As<Ingredient>();
		_ingredient = ingredient;
    }

	private void StartCooking()
	{
		_isCooking = true;
		_cookingBar.MaxValue = _cookTimer.WaitTime;

		string startedCookingMessage = $"Started Cooking {_ingredient.Name}";
		PrintRich.Print(TextColor.Blue, startedCookingMessage);
	}

	private void StopCooking()
	{
		_isCooking = false;
		_cookingBar.Value = 0f;
		_cookTimer.Stop();

		string stoppedCookingMessage = $"Stopped Cooking {_ingredient.Name}";
		PrintRich.Print(TextColor.Blue, stoppedCookingMessage);
	}

	private void UpdateCookingBar(Timer timer, Color color)
	{
		_cookingBar.Value = timer.WaitTime - timer.TimeLeft;
		_cookingBar.TintProgress = color;
	}

	private void FinishCooking()
	{
		_isCooking = false;
		_cookingBar.Value = 0f;
		_cookTimer.Stop();

		string finishedCookingMessage = $"Finished Cooking {_ingredient.Name}";
		PrintRich.Print(TextColor.Blue, finishedCookingMessage);

		StartBurning();
	}

	private void StartBurning()
	{
		_isBurning = true;
		_cookingBar.MaxValue = _burnTimer.WaitTime;

		string startedBurningMessage = $"Started Burning {_ingredient.Name}";
		PrintRich.Print(TextColor.Blue, startedBurningMessage);
	}

	private void StopBurning()
	{
		_isBurning = false;
		_cookingBar.Value = 0f;
		_burnTimer.Stop();

		string stoppedBurningMessage = $"Stopped Burning {_ingredient.Name}";
		PrintRich.Print(TextColor.Blue, stoppedBurningMessage);
	}

	private void FinishBurning()
	{
		_isBurning = false;
		// _ingredient.IsBurnt = true;

		string burnedMessage = $"{_ingredient.Name} Has Burned";
		PrintRich.Print(TextColor.Blue, burnedMessage);
	}

	private void AddCookTimer()
	{
		Timer cookTimer = new Timer()
		{
			// WaitTime = _ingredient.CookTime
		};
		cookTimer.Timeout += FinishCooking;
		
		AddChild(cookTimer);
		// OvenManager.Oven.Timers.Add(cookTimer);
		_cookTimer = cookTimer;
	}

	private void AddBurnTimer()
	{
		Timer burnTimer = new Timer()
		{
			// WaitTime = OvenManager.Instance.BurnTime
		};
		burnTimer.Timeout += FinishBurning;
		
		AddChild(burnTimer);
		_burnTimer = burnTimer;
	}
}
