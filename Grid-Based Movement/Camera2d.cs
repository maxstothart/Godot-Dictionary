using Godot;
using System;

public partial class Camera2d : Camera2D
{
    Tween currTween;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        Position = GetNode<Node2D>("/root/Grid/Player").Position - new Vector2(-100, -100);
    }

    public override void _Input(InputEvent @event)
    {

    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        if (GetMeta("MovementScheme").AsInt32() == 0)
        {
            Position = GetNode<Node2D>("/root/Grid/Player").Position;
        }
        else if (GetMeta("MovementScheme").AsInt32() == 1)
        {
            var XAxis = Input.GetAxis("ui_left", "ui_right");
            var YAxis = Input.GetAxis("ui_up", "ui_down");

            Vector2 MoveDirection = new Vector2();
            if (XAxis != 0)
            {
                MoveDirection += new Vector2(GetMeta("Distance").AsSingle() * XAxis, 0);
            }
            if (YAxis != 0)
            {
                MoveDirection += new Vector2(0, GetMeta("Distance").AsSingle() * YAxis);
            }

            if (!MoveDirection.IsZeroApprox())
            {
                moveRelative(MoveDirection, false);
            }
        }
        else if (GetMeta("MovementScheme").AsInt32() == 2)
        {
            Vector2 wSize = DisplayServer.WindowGetSize();
            Vector2 PlayerPos = GetNode<Node2D>("/root/Grid/Player").Position;
            Vector2 offset = GetMeta("Offset").AsVector2();

            var clamped = clampVec((Position - PlayerPos), new(-wSize.X / 2 + offset.X, -wSize.Y / 2 + offset.Y), new(wSize.X / 2 - offset.X, wSize.Y / 2 - offset.Y));

            if (clamped.changed) { Position = PlayerPos + clamped.value; }
        }
    }
    public record clampedVector2(Vector2 value, bool changed);
    public clampedVector2 clampVec(Vector2 input, Vector2 Min, Vector2 Max)
    {
        int i = 0;
        if (input.X > Max.X) { input.X = Max.X; i++; }
        if (input.Y > Max.Y) { input.Y = Max.Y; i++; }
        if (input.X < Min.X) { input.X = Min.X; i++; }
        if (input.Y < Min.Y) { input.Y = Min.Y; i++; }

        return new clampedVector2(input, (i == 0) ? false : true);
    }
    public void moveRelative(Vector2 Offset, bool instant = false)
    {
        move(Position.X + Offset.X, Position.Y + Offset.Y, instant);
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
