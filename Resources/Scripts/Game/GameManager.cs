using System.Collections.Generic;
using Godot;

namespace BearBakery;

public partial class GameManager : Node
{
	[Export]
	private TileSet _buildingTileset;

	[Export(PropertyHint.Range, "100,250,5")]
	private float _dragPreviewSize = 100;

	// public static List<Player> Players = new List<Player>();
	public static List<PlayerInfo> PlayersInfo = new List<PlayerInfo>();

	public bool InGame = false;
	public PlayerInterface Interface;

	private bool _isCollisionOn = false;
	private Timer _timer;

	public override void _EnterTree()
	{
		BearBakery.GameManager = this;
	}

	public Control GetDragPreview(Item item)
	{
		if (item == null) return new Control();
		
		Vector2 dragPreviewSize = new Vector2(_dragPreviewSize, _dragPreviewSize);
		TextureRect textureRect = new TextureRect()
		{
			Texture = item.Texture,
			Size = dragPreviewSize,
			Position = -dragPreviewSize / 2,
			ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
			StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered
		};
		Control dragPreview = new Control();
		dragPreview.AddChild(textureRect);

		return dragPreview;
	}
}
