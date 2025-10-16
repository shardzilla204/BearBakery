using Godot;

namespace BearBakery;

public partial class Signals : Node
{
    // Player
    [Signal]
    public delegate void PlayerIdleEventHandler();

    [Signal]
    public delegate void PlayerMovedEventHandler();

    [Signal]
    public delegate void PlayerSprintingEventHandler(float delta);

    [Signal]
    public delegate void PlayerLeapedEventHandler();

    [Signal]
    public delegate void PlayerInteractedEventHandler();

    [Signal]
    public delegate void PlayerSecondaryActionEventHandler();

    [Signal]
    public delegate void PlayerItemUpdatedEventHandler();

    [Signal]
    public delegate void PlayerPickedUpEventHandler(Item item);

    [Signal]
    public delegate void PlayerThrewEventHandler(Item item);

    [Signal]
    public delegate void PlayersSpawnedEventHandler();

    // Oven
    [Signal]
    public delegate void OvenOpenedEventHandler(OvenArea ovenArea);

    [Signal]
    public delegate void OvenClosedEventHandler();

    [Signal]
    public delegate void CookedEventHandler();

    [Signal]
    public delegate void BurntEventHandler();

    // Mixer
    [Signal]
    public delegate void MixerOpenedEventHandler(MixerArea mixerArea);

    [Signal]
    public delegate void MixerClosedEventHandler();

    [Signal]
    public delegate void MixedEventHandler();

    // Fridge
    [Signal]
    public delegate void FridgeOpenedEventHandler(FridgeArea fridgeArea);

    [Signal]
    public delegate void FridgeClosedEventHandler();

    [Signal]
    public delegate void IngredientAddedToFridgeEventHandler(Ingredient ingredient);

    [Signal]
    public delegate void IngredientRemovedFromFridgeEventHandler(Ingredient ingredient);

    // Stamina
    [Signal]
    public delegate void StaminaFilledEventHandler(double value, double maxValue);

    [Signal]
    public delegate void StaminaDepletedEventHandler(float rechargeTime);

    // Item
    [Signal]
    public delegate void ItemAddedToInventoryEventHandler(Item item);

    [Signal]
    public delegate void ItemRemovedFromInventoryEventHandler(Item item);

    [Signal]
    public delegate void ShowItemContentEventHandler(Item item);

    [Signal]
    public delegate void ShowKeybindEventHandler(Key key, string actionName);

    [Signal]
    public delegate void HideKeybindEventHandler(Key key);

    [Signal]
    public delegate void CanvasOpenedEventHandler(CanvasType canvasType, Node canvas);

    [Signal]
    public delegate void CanvasClosedEventHandler(CanvasType canvasType, Node canvas);
}
