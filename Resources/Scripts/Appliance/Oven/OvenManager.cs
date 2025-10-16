using Godot;
using GC = Godot.Collections;
using System.Collections.Generic;

namespace BearBakery;

public partial class OvenManager : Node
{
	[Export]
	private int _maxCount = 4;

	[Export]
	public int _maxRackCount = 3;

	[Export]
	public int _maxSlotCount = 2;

	[Export]
	private float _burnTime = 30f;
	
	public List<OvenArea> Ovens = new List<OvenArea>();
	public int MaxCount; 
	public int MaxRackCount;
	public int MaxSlotCount;
	public float BurnTime;

	public Color CookColor = new Color(0.5f, 1f, 0.25f);
	public Color BurnColor = new Color(1f, 0.25f, 0.25f);

	public override void _EnterTree()
	{
		BearBakery.OvenManager = this;

        BearBakery.Signals.OvenOpened += AddOvenInterface;

		MaxCount = _maxCount;
		MaxRackCount = _maxRackCount;
		MaxSlotCount = _maxSlotCount;
		BurnTime = _burnTime;
	}

	private void AddOvenInterface(OvenArea ovenArea)
	{
		OvenInterface ovenInterface = BearBakery.PackedScenes.GetOvenInterface(ovenArea);
		BearBakery.GameManager.Interface.AddInterface(ovenInterface);
	}

	public List<OvenSlot> GetOvenSlots()
	{
		List<OvenSlot> ovenSlots = new List<OvenSlot>();
		for (int i = 0; i < _maxSlotCount; i++)
		{
			OvenSlot ovenSlot = BearBakery.PackedScenes.GetOvenSlot();
			ovenSlots.Add(ovenSlot);
		}
		return ovenSlots;
	}

	public GC.Dictionary<string, Variant> GetData()
	{
		return new GC.Dictionary<string, Variant>()
		{
			{ "Ovens", GetOvenDictionaries() },
		};
	}

	public GC.Dictionary<string, Variant> GetOvenDictionaries()
	{
		GC.Dictionary<string, Variant> ovenDictionaries = new GC.Dictionary<string, Variant>();
		for (int i = 0; i < Ovens.Count; i++)
		{
			ovenDictionaries.Add($"Oven{i}", GetOvenDictionary(Ovens[i]));
		}
		return ovenDictionaries;
	}

	public void SetData()
	{

	}
	
	private GC.Dictionary<string, Variant> GetOvenDictionary(OvenArea oven)
	{
		GC.Dictionary<string, Variant> ovenDictionary = new GC.Dictionary<string, Variant>();
		int rackCount = oven.Racks.Count == 0 ? 1 : oven.Racks.Count;
		for (int i = 0; i < rackCount; i++)
		{
			ovenDictionary.Add($"Rack{i}", GetRackDictionary(oven.Racks[i]));
		}
		return ovenDictionary;
	}
	
	private GC.Dictionary<string, Variant> GetRackDictionary(OvenRack ovenRack)
	{
		GC.Dictionary<string, Variant> rackDictionary = new GC.Dictionary<string, Variant>();
		for (int i = 0; i < ovenRack.Slots.Count; i++)
		{
			rackDictionary.Add($"Slot{i}", GetSlotData(ovenRack.Slots[i]));
		}
		return rackDictionary;
	}

	private GC.Dictionary<string, Variant> GetSlotData(OvenSlot ovenSlot)
	{
		return new GC.Dictionary<string, Variant>()
		{
			// { "IsCooking" }
		};
	}

}
