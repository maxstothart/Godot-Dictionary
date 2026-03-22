using Godot;
using System;
using static GridScript;

public partial class TileMap : TileMapLayer
{
	[Export]
	public Vector2 TileCoords;
	[Export]
	public bool Touching;
	
	public Grid grid;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        grid = new GridScript.Grid(GetNode("/root/Grid").GetMeta("GridSize").AsInt32(), GetNode("/root/Grid").GetMeta("WorldSize").AsVector2());
	}

	public void updateTileChoords(int x, int y)
	{
        TileCoords = GetCellAtlasCoords(LocalToMap(ToLocal(toVec(grid.getMiddleOfBox(x, y)))));
        GD.Print(TileCoords);

        if (TileCoords == new Vector2(-1,-1)) { Touching = false; }
		else { Touching = true; }

		
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}
}
