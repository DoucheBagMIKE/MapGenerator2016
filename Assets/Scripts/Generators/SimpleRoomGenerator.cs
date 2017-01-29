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

        TilesetInfo tsInfo = ResourceManager.tilesetInfo["Walls"] as TilesetInfo;
        int wallid = tsInfo.firstGid;

        if (pos.X < 0 || pos.X + size.X > Map.width || pos.Y < 0 || pos.Y + size.Y > Map.height)// Rect out of Bounds
        {
            return;
        }

        for (int y = (int)pos.Y; y < pos.Y + size.Y; y++)
        {
            for (int x = (int)pos.X; x < pos.X + size.X; x++)
            {
                floor[y, x] = 0;
                walls[y, x] = 0;

                if (y == pos.Y || y == pos.Y + size.Y - 1 || x == pos.X || x == pos.X + size.X - 1)// if x,y lies on the edge of the rect.
                {
                    walls[y, x] = wallid;
                    WallsList.Add(new IntPoint(x, y));
                } else
                {
                    
                    if (MapGenerator.instance.Rng.Next(0, 11) == 10)
                    {
                        walls[y, x] = wallid;
                        WallsList.Add(new IntPoint(x, y));
                    } else
                    {
                        floor[y, x] = 1;
                    }
                }
            }
        }
    }
}
