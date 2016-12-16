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

    public static bool Contains(Rect R1, Rect R2)
    {
        return (R2.x + R2.width) < (R1.x + R1.width)
            && (R2.x) > (R1.x)
            && (R2.y) > (R1.y)
            && (R2.y + R2.height) < (R1.y + R1.height);
    }

    public static bool Contains(Rect rect, Circle circle)
    {
        float dTL = Mathf.Sqrt((rect.xMin - circle.centerPos.x) * (rect.xMin - circle.centerPos.x) + (rect.yMin - circle.centerPos.y) * (rect.yMin - circle.centerPos.y));
        float dTR = Mathf.Sqrt((rect.xMax - circle.centerPos.x) * (rect.xMax - circle.centerPos.x) + (rect.yMin - circle.centerPos.y) * (rect.yMin - circle.centerPos.y));
        float dBL = Mathf.Sqrt((rect.xMin - circle.centerPos.x) * (rect.xMin - circle.centerPos.x) + (rect.yMax - circle.centerPos.y) * (rect.yMax - circle.centerPos.y));
        float dBR = Mathf.Sqrt((rect.xMax - circle.centerPos.x) * (rect.xMax - circle.centerPos.x) + (rect.yMax - circle.centerPos.y) * (rect.yMax - circle.centerPos.y));

        return rect.Contains(circle.centerPos) && dTL >= circle.radius && dTR >= circle.radius && dBL >= circle.radius && dBR >= circle.radius;
    }

    public static bool Intersects (Rect R1, Rect R2)
    {
        if (R1.left < R2.right && R1.right > R2.left && R1.top < R2.bottom && R1.bottom > R2.top)
        {
            return true;
        }
        return false;
    }

    public static bool Intersects (Circle circle, Rect rect)
    {
        // Find the closest point to the circle within the rectangle
        // Assumes axis alignment! ie rect must not be rotated
        var closestX = Mathf.Clamp(circle.centerPos.x, rect.x, rect.x + rect.width);
        var closestY = Mathf.Clamp(circle.centerPos.y, rect.y, rect.y + rect.height);

        // Calculate the distance between the circle's center and this closest point
        var distanceX = circle.centerPos.x - closestX;
        var distanceY = circle.centerPos.y - closestY;

        // If the distance is less than the circle's radius, an intersection occurs
        var distanceSquared = (distanceX * distanceX) + (distanceY * distanceY);

        return distanceSquared < (circle.radius * circle.radius);

    }


}
