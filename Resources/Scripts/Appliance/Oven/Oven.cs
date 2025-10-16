using Godot;
using System.Collections.Generic;

namespace BearBakery;

public partial class Oven : Node
{
	public int RackCount = 1;
	public List<Ingredient> Ingredients = new List<Ingredient>();
}
