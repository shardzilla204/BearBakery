using Godot;
using Godot.Collections;

namespace BearBakery;

public partial class IngredientObject : ItemObject
{
    public float CookTime = 5f;
    public bool IsBurnt = false;

    public override void _Ready()
    {
        base._Ready();
    }
}