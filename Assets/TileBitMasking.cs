using UnityEngine;
using System.Collections;
using ClipperLib;

public static class TileBitMasking {

    public static IntPoint[] dirs = new IntPoint[4]
        {
            new IntPoint(0,1),
            new IntPoint(-1, 0),
            new IntPoint(1, 0),
            new IntPoint(0,-1)
        };

    public static int GetTile (int[,] Map, IntPoint pos, int ID)
    {
        TmxFile Tmx = MapGenerator.instance.Tmx;
        int tID = (int)Tmx.tileIdToTilesetId(ID);

        int FirstGID = Tmx.tileset[tID].firstgid;
        int NumberOfTiles = Tmx.tileset[tID].tilecount;

        int NID = 0;

        IntPoint cPos;
        for(int i = 0; i < dirs.Length; i++)
        {
            cPos.X = pos.X + dirs[i].X;
            cPos.Y = pos.Y + dirs[i].Y;

            if (cPos.X < 0 || cPos.X > Map.GetLength(1) || cPos.Y < 0 || cPos.Y > Map.GetLength(0))// out of bounds.
            {
                continue;
            }
            if (Map[cPos.Y, cPos.X] >= FirstGID || Map[cPos.Y, cPos.X] >= FirstGID + NumberOfTiles - 1)// current tile being looked at is part of this spritesheet.
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
}
