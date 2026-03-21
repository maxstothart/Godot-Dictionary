using Godot;
using System;
using static GridScript;

public partial class Player : Node2D
{
	public Grid grid;
	private Tween currTween;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        grid = new GridScript.Grid(GetNode("/root/Grid").GetMeta("GridSize").AsInt32(), GetNode("/root/Grid").GetMeta("WorldSize").AsVector2());
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}
	public void moveGrid(int X, int Y)
	{
		(X, Y) = grid.getMiddleOfBox(X, Y);
        move(X, Y, GetMeta("isInstant").AsBool());
    }

	public void move(float X, float Y, bool instant = false)
	{
        if (currTween != null && currTween.IsValid()) { currTween.Kill(); }

        //GD.Print($"Move To: {X}-{Y}");
        double distance = Math.Sqrt(Math.Pow(Math.Abs(Position.X - X), 2) + Math.Pow(Math.Abs(Position.Y - Y), 2)) / (DisplayServer.WindowGetSize().X / 2);
		currTween = CreateTween();
		currTween.TweenProperty(this, "position", new Vector2(X, Y), ((instant) ? 0 : GetMeta("Speed").AsDouble() * distance));
    }
}
