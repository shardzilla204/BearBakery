using Godot;

namespace BearBakery;

public partial class QuitOptions : VBoxContainer
{
	[Export]
	private CustomButton _toMenuButton;

	[Export]
	private CustomButton _toDesktopButton;

	public override void _Ready()
	{
		Visible = false;

		_toMenuButton.Pressed += QuitToMenu;
		_toDesktopButton.Pressed += QuitToDesktop;
	}

	// ? Save the game first then quit
	private void QuitToMenu()
	{
		BearBakery.GameManager.InGame = false;
		
		MenuCanvas menuCanvas = BearBakery.PackedScenes.GetMenuCanvas();
		AddSibling(menuCanvas);
		// GameCanvas.Instance.QueueFree();
		QueueFree();
	}

	private void QuitToDesktop()
	{
		GetTree().Quit();
	}
}
