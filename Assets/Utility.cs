using UnityEngine;
using System.Collections.Generic;

public class Utility {
    public static MapPos[] dirs = new MapPos[4] { new MapPos(-1, 0), new MapPos(0, 1), new MapPos(1, 0), new MapPos(0, -1) };
    static Queue<MapPos> fringe = new Queue<MapPos>();

    //   Flood-fill(node, target-color, replacement-color):
    // 1. If target-color is equal to replacement-color, return.
    // 2. If color of node is not equal to target-color, return.
    // 3. Set Q to the empty queue.
    // 4. Add node to the end of Q.
    // 5. While Q is not empty: 
    // 6.     Set n equal to the first element of Q.
    // 7.     Remove first element from Q.
    // 8.     If color of n is equal to target-color:
    // 9.         Set the color of n to replacement-color.
    //10.         Add west node to end of Q if color of west is equal to target-color.
    //11.         Add east node to end of Q if color of east is equal to target-color.
    //12.         Add north node to end of Q if color of north is equal to target-color.
    //13.         Add south node to end of Q if color of south is equal to target-color.
    //14. Return.

    public static int[,] floodfill(int sx, int sy, int[,] Map, int targetColor = 1, int replaceColor = 1)
    {
        int[,] ret = new int[Map.GetLength(0), Map.GetLength(1)];
        //if (targetColor == replaceColor)
        //    return ret;
        if (Map[sx, sy] != targetColor)
            return ret;
        
        fringe.Enqueue(new MapPos(sx, sy));

        while (fringe.Count != 0)
        {
            MapPos n = fringe.Dequeue();
            if (Map[n.x, n.y] == targetColor)
            {
                ret[n.x, n.y] = replaceColor;
                Map[n.x, n.y] = -1;
                foreach (MapPos dir in dirs)
                {
                    int nx = n.x + dir.x;
                    int ny = n.y + dir.y;
                    if (nx < 0 || ny < 0 || nx > Map.GetLength(0) - 1 || ny > Map.GetLength(1) - 1)
                    {
                        continue;
                    }
                    if (Map[nx, ny] == targetColor)
                    {
                        fringe.Enqueue(new MapPos(nx, ny));
                    }
                }
            }
        }
        return ret;
    }
}
