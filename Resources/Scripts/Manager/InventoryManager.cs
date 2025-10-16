using Godot;
using System.Collections.Generic;

namespace BearBakery;

public partial class InventoryManager : Node
{
	[Export]
	private int _maxSize = 3;

	public int MaxSize = 3;
    public List<ItemObject> ItemQueue = new List<ItemObject>();

	public bool IsDragging = false; /// Do Not Remove | <see cref="FridgeSlot.SetLowlight"/>
    public List<SlotComponent> Slots = new List<SlotComponent>();

    public override void _EnterTree()
    {
        BearBakery.InventoryManager = this;
		
		BearBakery.Signals.ItemRemovedFromInventory += ItemRemovedFromInventory;
		BearBakery.Signals.PlayerItemUpdated += PlayerItemUpdated;

		MaxSize = _maxSize;
    }

	public override void _ExitTree()
	{
		BearBakery.Signals.ItemRemovedFromInventory -= ItemRemovedFromInventory;
		BearBakery.Signals.PlayerItemUpdated -= PlayerItemUpdated;
    }

	private void PlayerItemUpdated()
	{
		// Clear all the slots
		foreach (SlotComponent inventorySlot in Slots)
		{
			inventorySlot.SetItem(null);
		}

		// Sets items from the players inventory
		// List<Item> playerItems = BearBakery.Player.Inventory.Items;
		// for (int i = 0; i < playerItems.Count; i++)
		// {
		// 	SlotComponent inventorySlot = Slots[i];
		// 	Item item = playerItems[i];
		// 	inventorySlot.SetItem(item);
		// }
	}

	public ItemObject GetItemObject(Item item)
    {
		ItemObject itemObject = BearBakery.PackedScenes.GetItemObject(item);
        // itemObject.Position = BearBakery.Player.Position;
        
        return itemObject;
    }

	// When the item placed onto counters
	private void ItemRemovedFromInventory(Item item)
	{
		ItemObject itemObject = ItemQueue.Find(itemObject => itemObject.Item == item);
		ItemQueue.Remove(itemObject);
	}
}
