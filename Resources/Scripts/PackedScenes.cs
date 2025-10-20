using Godot;

namespace BearBakery;

public partial class PackedScenes : Node
{
    [Export]
    private PackedScene _settingsCanvas;

    [Export]
    private PackedScene _gameCanvas;

    [Export]
    private PackedScene _lobbyCanvas;

    [Export]
    private PackedScene _menuCanvas;

    [Export]
    private PackedScene _itemObject;

    [Export]
    private PackedScene _itemContentInterface;

    [Export]
    private PackedScene _keybindAction;

    [Export]
    private PackedScene _player;

    [Export]
    private PackedScene _playerPlate;

    [Export]
    private PackedScene _tooltipLabel;

    [ExportGroup("Oven")]
    [Export]
    private PackedScene _ovenWindow;

    [Export]
    private PackedScene _ovenRack;

    [Export]
    private PackedScene _ovenSlot;

    [ExportGroup("Fridge")]
    [Export]
    private PackedScene _fridgeWindow;

    [Export]
    private PackedScene _fridgeSlot;

    [ExportGroup("Mixer")]
    [Export]
    private PackedScene _mixerWindow;

    [Export]
    private PackedScene _mixerSlot;
    
    public override void _EnterTree()
    {
        BearBakery.PackedScenes = this;
    }

    // Canvas
    public SettingsCanvas GetSettingsCanvas()
    {
        return _settingsCanvas.Instantiate<SettingsCanvas>();
    }

    public GameCanvas GetGameCanvas()
    {
        return _gameCanvas.Instantiate<GameCanvas>();
    }

    public MenuCanvas GetMenuCanvas()
    {
        return _menuCanvas.Instantiate<MenuCanvas>();
    }

    public LobbyCanvas GetLobbyCanvas()
    {
        return _lobbyCanvas.Instantiate<LobbyCanvas>();
    }

    public Label GetTooltipLabel()
    {
        return _tooltipLabel.Instantiate<Label>();
    }

    // Oven
    public OvenWindow GetOvenWindow(OvenArea ovenArea)
    {
        OvenWindow ovenWindow = _ovenWindow.Instantiate<OvenWindow>();
        ovenWindow.TreeExiting += ovenArea.CloseOven;
        return _ovenWindow.Instantiate<OvenWindow>();
    }

    public OvenRack GetOvenRack()
    {
        return _ovenRack.Instantiate<OvenRack>();
    }

    public OvenSlot GetOvenSlot()
    {
        return _ovenSlot.Instantiate<OvenSlot>();
    }

    // Fridge
    public FridgeWindow GetFridgeWindow(FridgeArea fridgeArea)
    {
        FridgeWindow fridgeWindow = _fridgeWindow.Instantiate<FridgeWindow>();
        fridgeWindow.TreeExiting += fridgeArea.CloseFridge;
        return fridgeWindow;
    }

    public FridgeSlot GetFridgeSlot(Item item)
    {
        FridgeSlot fridgeSlot = _fridgeSlot.Instantiate<FridgeSlot>();
        fridgeSlot.SetItem(item);
        return fridgeSlot;
    }

    // Mixer
    public MixerWindow GetMixerWindow(MixerArea mixerArea)
    {
        MixerWindow mixerWindow = _mixerWindow.Instantiate<MixerWindow>();
        mixerWindow.TreeExiting += mixerArea.CloseMixer;
        return mixerWindow;
    }

    public MixerSlot GetMixerSlot()
    {
        MixerSlot mixerSlot = _mixerSlot.Instantiate<MixerSlot>();
        return mixerSlot;
    }

    // Miscellaneous
    public ItemObject GetItemObject(Item item)
    {
        ItemObject itemObject = _itemObject.Instantiate<ItemObject>();
        itemObject.Item = item;
        itemObject.Name = item.Name;
        return itemObject;
    }

    public ItemContentInterface GetItemContentInterface(StorageItem storageItem)
    {
        ItemContentInterface itemContentInterface = _itemContentInterface.Instantiate<ItemContentInterface>();
        itemContentInterface.StorageItem = storageItem;
        return itemContentInterface;
    }

    public KeybindAction GetKeybindAction(Key keybind, string actionName)
    {
        KeybindAction keybindAction = _keybindAction.Instantiate<KeybindAction>();
        keybindAction.Set(keybind, actionName);
        return keybindAction;
    }

    public Player GetPlayer()
    {
        return _player.Instantiate<Player>();
    }

    public PlayerPlate GetPlayerPlate(int id, string text)
    {
        PlayerPlate playerPlate = _playerPlate.Instantiate<PlayerPlate>();
        playerPlate.Name = $"{id}";
        playerPlate.SetText(text);
        return playerPlate;
    }
}
