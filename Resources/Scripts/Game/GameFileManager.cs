using Godot;
using System;

namespace BearBakery;

public partial class GameFileManager : Node
{
	private static GameFileManager _instance;
	public static GameFileManager Instance
	{
		get => _instance;
		private set 
		{
			if (_instance == null)
			{
				_instance = value;
			}
			else if (_instance != value)
			{
				GD.PrintRich($"{nameof(GameFileManager)} has already been created");
			}
		}
	}

	public void Save()
	{

	}

	public void Load()
	{
		
	}

	public void Erase()
	{

	}
}
