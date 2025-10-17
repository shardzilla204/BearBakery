using System.Collections.Generic;
using Godot;

namespace BearBakery;

public partial class GameManager : Node
{
	[Export]
	private TileSet _buildingTileset;

	[Export(PropertyHint.Range, "100,250,5")]
	private float _dragPreviewSize = 100;

	[Export]
	private PlayerInterface _playerInterface;

	// public static List<Player> Players = new List<Player>();
	public static List<PlayerInfo> PlayersInfo = new List<PlayerInfo>();

	private static Vector2 _DragPreviewSize = Vector2.Zero;

	private bool _isCollisionOn = false;
	private Timer _timer;

	public override void _ExitTree()
	{
		BearBakery.Signals.WindowOpened -= OnWindowOpened;
	}
	
	public override void _EnterTree()
	{
		BearBakery.Signals.WindowOpened += OnWindowOpened;
	}

    public override void _Ready()
    {
		_DragPreviewSize = new Vector2(_dragPreviewSize, _dragPreviewSize);
    }

	public static Control GetDragPreview(Item item)
	{
		if (item == null) return new Control();

		TextureRect textureRect = new TextureRect()
		{
			Texture = item.Texture,
			Size = _DragPreviewSize,
			Position = -_DragPreviewSize / 2,
			ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
			StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered
		};
		Control dragPreview = new Control();
		dragPreview.AddChild(textureRect);

		return dragPreview;
	}
	
	private void OnWindowOpened(Node window)
    {
		_playerInterface.AddChild(window);
    }
}
