using Godot;

public partial class KeybindAction : Container
{
    [Export]
    private Label _keybind;

    [Export]
    private Label _actionName;

    public Key Keybind;
    public string ActionName;

    public KeybindAction() {}

    public KeybindAction(Key keybind, string actionName)
    {
        Keybind = keybind;
        ActionName = actionName;
    }

    public void Set(Key keybind, string actionName)
    {
        _keybind.Text = $"{keybind}";
        _actionName.Text = $"{actionName}";

        Keybind = keybind;
        ActionName = actionName;
    }
}
