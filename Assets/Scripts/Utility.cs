using UnityEngine;
using ClipperLib;
using System.Collections.Generic;

public static class Utility {
    public static IntPoint[] dirs = new IntPoint[4] { new IntPoint(-1, 0), new IntPoint(0, 1), new IntPoint(1, 0), new IntPoint(0, -1) };
    static Queue<IntPoint> fringe = new Queue<IntPoint>();

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
        
        fringe.Enqueue(new IntPoint(sx, sy));

        while (fringe.Count != 0)
        {
            IntPoint n = fringe.Dequeue();
            if (Map[n.X, n.Y] == targetColor)
            {
                ret[n.X, n.Y] = replaceColor;
                Map[n.X, n.Y] = -1;
                foreach (IntPoint dir in dirs)
                {
                    long nx = n.X + dir.X;
                    long ny = n.Y + dir.Y;
                    if (nx < 0 || ny < 0 || nx > Map.GetLength(0) - 1 || ny > Map.GetLength(1) - 1)
                    {
                        continue;
                    }
                    if (Map[nx, ny] == targetColor)
                    {
                        fringe.Enqueue(new IntPoint(nx, ny));
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
        return Contains(rect, circle.Rect);
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

    // A* pathfinding
    public static void GetPath(int startID, int endID, Graph connections, Dictionary<int, Circle> Zones, ref List<int> path)
    {
        List<int> open = new List<int>(); // The "Fringe Nodes" the nodes that the algorithim is working with currently.
        List<int> closed = new List<int>(); // Nodes that have been Processed;
        Dictionary<int, int> moveCost = new Dictionary<int, int>(); // Distance in Nodes from the Start Location.
        Dictionary<int, float> hCost = new Dictionary<int, float>();// distance to the end Node fromt he curent node.
        Dictionary<int, float> score = new Dictionary<int, float>();// total score that the sort algorithim works with. (movecost+hcost)
        Dictionary<int, int> parent = new Dictionary<int, int>();// stores the ID of the Node that this node came from.

        moveCost.Add(startID, 0);

        Vector2 v = Zones[startID].centerPos - Zones[endID].centerPos;
        hCost.Add(startID, Mathf.Abs(v.x) + Mathf.Abs(v.y));

        score.Add(startID, hCost[startID]);

        parent.Add(startID, startID);// set the start nodes parent to itself;

        open.Add(startID);

        while (open.Count != 0 && !closed.Contains(endID))
        {
            // find the Node with the lowest score, remove it from the open list and add it to the closed list.
            BSort(score, open);
            int selectedID = open[0];
            closed.Add(selectedID);
            open.RemoveAt(0);

            // for each node connected to the selected node thats not in the open list already,
            //we add it to the open list and calculate its movement score.
            //If the connected node is already in the open list, we Check if the movement score plus the hursetic score is lower when we use the current generated path to get there,
            //If it is, update its score and update its parent as well.

            foreach (int adjacentID in connections.GetAdjecent(selectedID))
            {
                if(closed.Contains(adjacentID))
                {
                    continue;
                }

                if (!open.Contains(adjacentID))
                {
                    open.Add(adjacentID);

                    moveCost.Add(adjacentID, moveCost[selectedID] + 1);

                    v = Zones[adjacentID].centerPos - Zones[endID].centerPos;
                    hCost.Add(adjacentID, Mathf.Abs(v.x) + Mathf.Abs(v.y));

                    score[adjacentID] = moveCost[adjacentID] + hCost[adjacentID];

                    parent.Add(adjacentID, selectedID);
                }
                else // openlist already contains this id.
                {
                    if (score[adjacentID] > moveCost[selectedID] + 1 + hCost[adjacentID])
                    {
                        moveCost[adjacentID] = moveCost[selectedID] + 1;
                        score[adjacentID] = moveCost[adjacentID] + hCost[adjacentID];

                        parent[adjacentID] = selectedID;
                    }
                }
            }

        }
        // return the path by going through all the parent nodes until we find the start node.
        int sID = endID;

        while (!path.Contains(startID))
        {
            path.Add(sID);
            sID = parent[sID];
        }

    }

    //BubbleSort. an inefficient sorting algorithom, works for small datasets.
    public static void BSort (List<int> list)
    {
        BSort(list, list);
    }
    
    public static void BSort(List<int> cmpList, List<int> writeList)
    {
        //kindof a weird implementation, does more than it should. will have to look at this again.
        if (cmpList.Count != writeList.Count)
        {
            Debug.Log("Sort Failed.. listes were diffrent sizes.");
            return;
        }

        if (cmpList.Count <= 1)
        {
            return;
        }

        bool foundSwap = true;

        while (foundSwap)
        {
            foundSwap = false;
            for(int i = 0; i < cmpList.Count - 1; i++)
            {
                int a = cmpList[i];
                int b = cmpList[i + 1];
                int aa = writeList[i];
                int bb = writeList[i + 1];

                if (a > b)
                {
                    writeList[i] = bb;
                    writeList[i + 1] = aa;
                    cmpList[i] = b;
                    cmpList[i + 1] = a;
                    foundSwap = true;
                }
            }
        }
    }

    public static void BSort(Dictionary<int, float> cmpList, List<int> writeList)
    {
        if (cmpList.Count <= 1)
        {
            return;
        }

        bool foundSwap = true;

        while (foundSwap)
        {
            foundSwap = false;
            for (int i = 0; i < writeList.Count - 1; i++)
            {
                int a = writeList[i];
                int b = writeList[i + 1];

                if (cmpList[writeList[i]] > cmpList[writeList[i + 1]])
                {
                    writeList[i] = b;
                    writeList[i + 1] = a;

                    foundSwap = true;
                }
            }
        }
    }

    public static Vector2 DDALine (Circle p1, Circle p2)
    {
        Vector2 diff = p1.centerPos - p2.centerPos;
        float Steps;

        if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
            Steps = Mathf.Abs(diff.x);
        else
            Steps = Mathf.Abs(diff.y);

        float Xincrement = diff.x / Steps;
        float Yincrement = diff.y / Steps;

        float x = p1.centerPos.x;
        float y = p2.centerPos.y;

        float Distance = -1;

        while (Distance < 0)
        {
            x += Xincrement;
            y += Yincrement;
            Distance = Mathf.Sqrt(Mathf.Pow((p1.centerPos.x - x), 2) + Mathf.Pow((p1.centerPos.y - y), 2)) - (p1.radius + p2.radius);
        }
        //for (int v = 0; v < Steps; v++)
        //{
           
        //    x += Xincrement;
        //    y += Yincrement;
        //    //putpixel(Round(x), Round(y));
        //}
        return new Vector2(Mathf.Round(x), Mathf.Round(y));
    }

    public static Vector2 PointOnCircle (float radius)
    {
        float angle = (float)MapGenerator.instance.Rng.NextDouble() * Mathf.PI * 2;

        return new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
    }

    public static Vector2 LineIntersectionPoint(Vector2 ps1, Vector2 pe1, Vector2 ps2, Vector2 pe2)
    {
        // Get A,B,C of first line - points : ps1 to pe1
        float A1 = pe1.y - ps1.y;
        float B1 = ps1.x - pe1.x;
        float C1 = A1 * ps1.x + B1 * ps1.y;

        // Get A,B,C of second line - points : ps2 to pe2
        float A2 = pe2.y - ps2.y;
        float B2 = ps2.x - pe2.x;
        float C2 = A2 * ps2.x + B2 * ps2.y;

        // Get delta and check if the lines are parallel
        float delta = A1 * B2 - A2 * B1;
        if (delta == 0)
            throw new System.Exception("Lines are parallel");

        // now return the Vector2 intersection point
        return new Vector2(
            (B2 * C1 - B1 * C2) / delta,
            (A1 * C2 - A2 * C1) / delta
        );
    }
    public static bool LineIntersection(Vector2 ps1, Vector2 pe1, Vector2 ps2, Vector2 pe2)
    {
        // Get A,B,C of first line - points : ps1 to pe1
        float A1 = pe1.y - ps1.y;
        float B1 = ps1.x - pe1.x;
        float C1 = A1 * ps1.x + B1 * ps1.y;

        // Get A,B,C of second line - points : ps2 to pe2
        float A2 = pe2.y - ps2.y;
        float B2 = ps2.x - pe2.x;
        float C2 = A2 * ps2.x + B2 * ps2.y;

        // Get delta and check if the lines are parallel
        float delta = A1 * B2 - A2 * B1;
        if (delta == 0)
            return false;

        // now return the Vector2 intersection point
        return true;
    }
    public static bool LineIntersectsRect (Vector2 lStart, Vector2 lEnd, Rect rect)
    {
        bool bottom = LineIntersection(lStart, lEnd, new Vector2(rect.xMin, rect.yMin), new Vector2(rect.xMax, rect.yMin));
        bool  top = LineIntersection(lStart, lEnd, new Vector2(rect.xMin, rect.yMax), new Vector2(rect.xMax, rect.yMax));
        bool left = LineIntersection(lStart, lEnd, new Vector2(rect.xMin, rect.yMin), new Vector2(rect.xMin, rect.yMax));
        bool right = LineIntersection(lStart, lEnd, new Vector2(rect.xMax, rect.yMin), new Vector2(rect.xMax, rect.yMax));
        bool[] bools = new bool[4] { bottom, top, left, right };

        foreach (bool b in bools)
        {
            if (b == true)
                return true;
        }

        return false;
    }

}
