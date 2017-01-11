using UnityEngine;
using System.Collections;
using ClipperLib;
using System.Collections.Generic;

public class SimpleRoomGenerator {
    MapData Map;
	public SimpleRoomGenerator ()
    {
        Map = MapGenerator.instance.Map;
    }

    public void Generate(IntPoint pos, IntPoint size)
    {
        TmxFile Tmx = MapGenerator.instance.Tmx;
        List<IntPoint> WallsList = new List<IntPoint>();
        // generate a shitty room.
        int[,] floor = Map.layer["Floor"];
        int[,] walls = Map.layer["Walls"];

        if (pos.X < 0 || pos.X + size.X > Map.width || pos.Y < 0 || pos.Y + size.Y > Map.height)// Rect out of Bounds
        {
            return;
        }

        for (int y = (int)pos.Y; y < pos.Y + size.Y; y++)
        {
            for (int x = (int)pos.X; x < pos.X + size.X; x++)
            {
                floor[y, x] = 1;

                
                

                if (y == pos.Y || y == pos.Y + size.Y - 1 || x == pos.X || x == pos.X + size.X - 1)// if x,y lies on the edge of the rect.
                {
                    walls[y, x] = 9;
                    WallsList.Add(new IntPoint(x, y));
                } else
                {
                    if (MapGenerator.instance.Rng.Next(0, 11) == 10)
                    {
                        walls[y, x] = 9;
                        WallsList.Add(new IntPoint(x, y));
                    }
                }
            }
        }
        // autotle the walls tops and add in wall sides.
        foreach (IntPoint wPos in WallsList)
        {
            walls[wPos.Y, wPos.X] = TileBitMasking.GetTile(walls, wPos, 9);

            if (
                ((walls[wPos.Y, wPos.X] >= Tmx.tileset[(int)Tmx.tileIdToTilesetId(9)].firstgid &&
                walls[wPos.Y, wPos.X] <= 7 + Tmx.tileset[(int)Tmx.tileIdToTilesetId(9)].firstgid))
                && wPos.Y - 1 >= 0 && walls[wPos.Y - 1, wPos.X] == 0)
            {
                walls[wPos.Y - 1, wPos.X] = 25;
            }
        }
    }
}
