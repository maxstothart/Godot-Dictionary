using Godot;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class GridScript : Node2D
{
    public List<Line> Lines = new();
    public Grid grid;

    [Signal]
    public delegate void ClickGridEventHandler(int x, int y);


    public override void _Ready()
	{
        grid = new(GetMeta("GridSize").AsInt32(), GetMeta("WorldSize").AsVector2());
        getGridPos(Vector2.Zero);

        if (GetMeta("showGrid").AsBool())
        {
            Lines = grid.getLines(GetMeta("GridColor").AsColor(), GetMeta("LineWidth").AsInt32());
        }
        
        GD.Print(Lines.Count);

    }
    public override void _Input(InputEvent @event)
    {
		//GD.Print(@event.AsText());
		if (@event is InputEventMouseButton M)
		{
            if (M.Pressed && M.ButtonIndex == MouseButton.Left) 
            {
                Vector2 WSize = DisplayServer.WindowGetSize();
                Vector2 CPos = GetNode<Camera2D>("Camera2D").Position;

                (int, int) Pos = getGridPos((M.Position - WSize / 2) + CPos);
                GD.Print($"ClickGrid - {Pos.Item1}, {Pos.Item2}");
                EmitSignal(SignalName.ClickGrid, Pos.Item1, Pos.Item2);
            }
			
		}
    }

    public override void _Draw()
    {
		foreach (var line in Lines)
		{
            DrawPolyline(line.Points.ToArray(), line.Color, line.Width);
		}
		
    }
	public (int, int) getGridPos(Vector2 pos)
	{
        int size = GetMeta("GridSize").AsInt32();

        (int, int) Pos = ((int)(Math.Floor(pos[0] / size)), (int)(Math.Floor(pos[1] / size)));
        //GD.Print($"Grid Pos - X: {Pos.Item1}, Y: {Pos.Item2}");
        return Pos;
    }
	public struct Grid
	{
		public List<List<Vector2>> Points = new();
		public int gridSize;
		public (int, int) gridDimensions;

        public Grid(int size)
		{
            Vector2I WSize = DisplayServer.WindowGetSize();
            GD.Print($"Window Size - X: {WSize[0]}, Y: {WSize[1]}");

            gridSize = size;
			gridDimensions = ((WSize[0] / size)+1, (WSize[1] / size)+1);

            for (int y  = 0; y <= gridDimensions.Item2; y++)
            {
                List<Vector2> Store = new();
                for (int x = 0; x < gridDimensions.Item1;x++)
                {
                    Store.Add(new Vector2 { X = x*size, Y = y*size });
                }
                this.Points.Add(Store);
            }
        }

        public Grid(int size, Vector2 WSize)
        {
            gridSize = size;
            gridDimensions = (((int)WSize.X+1), ((int)WSize.Y+1));

            for (int y = 0; y < gridDimensions.Item2; y++)
            {
                List<Vector2> Store = new();
                for (int x = 0; x < gridDimensions.Item1; x++)
                {
                    Store.Add(new Vector2 { X = x * size, Y = y * size });
                }
                this.Points.Add(Store);
            }
        }

        public Vector2[][] asArray()
        {
            return Points.Select(item => item.ToArray()).ToArray();
        }

        public List<(Vector2, Vector2)> getRows()
        {
            List<(Vector2, Vector2)> output = new();
            foreach (var row in Points)
            {
                output.Add((row[0], row[^1]));
            }
            GD.Print(output.Count);
            return output;
        }
        public List<(Vector2, Vector2)> getCols()
        {
            List<(Vector2, Vector2)> output = new();
            for (int i = 0; i < gridDimensions.Item1; i++)
            {
                output.Add((Points[0][i], Points[^1][i]));
            }
            GD.Print(output.Count);
            return output;
        }
        public List<Line> getLines(Color color, int width)
        {
            //return Line.getLines(getRows().ToList(), color, width).ToList();
            return Line.getLines(getRows().Concat(getCols()).ToList(), color, width).ToList();
        }

        public (int, int) getMiddleOfBox(int x, int y)
        {
            x = clamp(x, gridDimensions.Item1-2) * gridSize + gridSize /2;
            y = clamp(y, gridDimensions.Item2-2) * gridSize + gridSize / 2;

            GD.Print(gridDimensions);
            GD.Print((x/gridSize, y/gridSize));

            return (x, y);
        }

        private int clamp(int x, int max)
        {
            if (x < 0)
            {
                return 0;
            }
            if (x > max)
            {
                return max;
            }
            return x;
        }


    }
    public struct Line
    {
        public List<Vector2> Points = new();
        public Godot.Color Color;
        public int Width;

        public Line(List<Vector2> points, Godot.Color color, int width)
        {
            Width = width;
            Color = color;
            Points = points;
        }
        public Line(Vector2 A, Vector2 B, Godot.Color color, int width)
        {
            Width = width;
            Color = color;
            Points.Add(A);
            Points.Add(B);
        }
        public static List<Line> getLines(List<(Vector2, Vector2)> Instances, Godot.Color color, int width)
        {
            List<Line> output = new();
            foreach (var line in Instances)
            {
                output.Add(new Line(line.Item1, line.Item2, color, width));
            }

            return output;
        }
    }
}
