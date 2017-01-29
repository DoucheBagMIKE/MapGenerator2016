using UnityEngine;
using System.Collections;
using ClipperLib;

public static class TileBitMasking
{

    public static IntPoint[] dirs = new IntPoint[4]
        {
            new IntPoint(0,1),
            new IntPoint(-1, 0),
            new IntPoint(1, 0),
            new IntPoint(0,-1)
        };

    public static int GetTile(int[,] Map, IntPoint pos)
    {
        TilesetInfo info = ResourceManager.tilesetInfo[(int)ResourceManager.tileIdToTilesetId(Map[pos.Y, pos.X])] as TilesetInfo;

        int FirstGID = info.firstGid;
        int NumberOfTiles = info.tileCount;

        int NID = 0;

        IntPoint cPos;
        for (int i = 0; i < dirs.Length; i++)
        {
            cPos.X = pos.X + dirs[i].X;
            cPos.Y = pos.Y + dirs[i].Y;

            if (cPos.X < 0 || cPos.X >= Map.GetLength(1) || cPos.Y < 0 || cPos.Y >= Map.GetLength(0))// out of bounds.
            {
                continue;
            }
            if (Map[cPos.Y, cPos.X] != 0)//>= FirstGID || Map[cPos.Y, cPos.X] >= FirstGID + NumberOfTiles - 1)// current tile being looked at is part of this spritesheet.
            {
                switch (i)
                {
                    case 0:// up
                        NID += 1 * 1;
                        break;
                    case 1:// left
                        NID += 2 * 1;
                        break;
                    case 2:// right
                        NID += 4 * 1;
                        break;
                    case 3:// down
                        NID += 8 * 1;
                        break;
                }
            }
        }

        return NID + FirstGID;//global tile position.

    }

    public static void autoTileAllBaseLayers()
    {
        TmxFile Tmx = MapGenerator.instance.Tmx;

        foreach (LayerInfo layer in MapData.BaseLayers)
        {
            if (layer.AutoTile == false)
                continue;

            int[,] map = MapGenerator.instance.Map.layer[layer.name];

            for (int y = 0; y < map.GetLength(0); y++)
            {
                for (int x = 0; x < map.GetLength(1); x++)
                {
                    if (map[y, x] != 0)
                    {
                        map[y, x] = TileBitMasking.GetTile(map, new ClipperLib.IntPoint(x, y));
                    }

                }
            }
            if (layer.name == "Walls")
            {
                for (int y = 0; y < map.GetLength(0); y++)
                {
                    for (int x = 0; x < map.GetLength(1); x++)
                    {
                        TilesetInfo tsInfo = ResourceManager.tilesetInfo[(int)ResourceManager.tileIdToTilesetId(map[y, x])] as TilesetInfo;

                        if (
                    ((map[y, x] >= tsInfo.firstGid &&
                    map[y, x] <= 7 + tsInfo.firstGid))
                    && y - 1 >= 0 && map[y - 1, x] == 0)
                        {
                            tsInfo = ResourceManager.tilesetInfo["SideWalls"] as TilesetInfo;

                            map[y - 1, x] = tsInfo.firstGid;
                        }

                    }
                }
            }
        }
    }
}