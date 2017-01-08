using UnityEngine;
using ClipperLib;
using System.Collections.Generic;

public class MazeGenerator : MonoBehaviour
{
    // Blockwise growing tree algorithom.
    MazeGen _MazeGen;

    [Range(0f,1f)]
    public float loopPercent;
    [Range(0f,1f)]
    public float windyOrRandomPercent;

    // Use this for initialization
    void Start()
    {
        _MazeGen = new MazeGen(MapGenerator.instance.Map.width, MapGenerator.instance.Map.height);
        _MazeGen.loopPercent = loopPercent;
        _MazeGen.windyOrRandomPercent = windyOrRandomPercent;
    }

    public void Generate(int sx, int sy, int[,] Map)
    {
        _MazeGen.Generate(sx, sy, Map);
    }

    public void Sparsify(int[,] Map)
    {
        _MazeGen.Sparsify(Map);
    }
}

public class MazeGen
{
    System.Random Rng;

    public bool[,] Visited;
    List<IntPoint> Fringe;

    public float loopPercent;
    public float windyOrRandomPercent;

    public MazeGen (int width, int height)
    {
        Rng = MapGenerator.instance.Rng;
        Visited = new bool[width, height];
        Fringe = new List<IntPoint>();
    }

    public void Generate(int sx, int sy, int[,] Map)
    {
        if (Map[sx, sy] == 1)
        {
            return;
        }

        IntPoint start = new IntPoint(sx, sy);

        Fringe.Add(start);

        while (Fringe.Count > 0)
        {

            IntPoint selected = pickNextFringe();
            Map[selected.X, selected.Y] = 1;
            Visited[selected.X, selected.Y] = true;

            List<IntPoint> adjacent = FindFringeNeighbors(selected, 0, Map);
            if (adjacent.Count > 0)
            {
                IntPoint adjPos = adjacent[Rng.Next(0, adjacent.Count - 1)];
                if (Rng.NextDouble() >= loopPercent)
                {
                    Map[adjPos.X, adjPos.Y] = 1;
                }
                setPassage(selected, adjPos, Map);
                Fringe.Add(adjPos);
            }
            else
            {
                Fringe.Remove(selected);
            }
        }
        return;
    }

    IntPoint pickNextFringe()
    {
        if (Rng.NextDouble() < windyOrRandomPercent)
        {
            return Fringe[Rng.Next(0, Fringe.Count - 1)];
        }
        else
        {
            return Fringe[Fringe.Count - 1];
        }

        //return Fringe[0];
        //return Fringe[Fringe.Count - 1];
    }

    public List<IntPoint> FindFringeNeighbors(IntPoint Pos, int TileState, int[,] Map)
    {
        List<IntPoint> ret = new List<IntPoint>();

        foreach (IntPoint index in Utility.dirs)
        {
            long x = Pos.X + (index.X * 2);
            long y = Pos.Y + (index.Y * 2);


            if (x < 0 || x > Map.GetLength(1) - 1 || y < 0 || y > Map.GetLength(0) - 1)
            {
                continue;
            }

            if (MapGenerator.instance.ca_mask && MapGenerator.instance.mask[x, y] == 0)
            {
                continue;
            }

            if (Map[x, y] == TileState && Visited[x, y] == false)
            {
                ret.Add(new IntPoint(x, y));
            }
        }
        return ret;
    }

    void setPassage(IntPoint a, IntPoint b, int[,] Map)
    {
        long x = (a.X + b.X) / 2;
        long y = (a.Y + b.Y) / 2;

        Map[x, y] = 1;
    }

    public void Sparsify(int[,] Map)
    {
        int[,] ret = new int[Map.GetLength(1), Map.GetLength(0)];
        int count;
        int state;

        for (int x = 0; x < Map.GetLength(1); x++)
        {
            for (int y = 0; y < Map.GetLength(0); y++)
            {
                //if (Visited[x,y] == true)
                //{
                //    continue;
                //}
                if (Map[x, y] == 1)
                {
                    state = 0;

                }
                else
                {
                    state = 1;
                }

                count = 0;
                foreach (IntPoint i in Utility.dirs)
                {
                    long nx = i.X + x;
                    long ny = i.Y + y;

                    if (nx < 0 || nx > Map.GetLength(1) - 1 || ny < 0 || ny > Map.GetLength(0) - 1)
                    {
                        continue;
                    }

                    if (Map[nx, ny] == state)
                    {
                        count++;
                    }
                }
                if (Map[x, y] == 1)
                {
                    if (count > 2)
                    {
                        Map[x, y] = 0;
                        //if (Rng.NextDouble() > 0.5f)
                        //{
                        //    Map[x, y] = 0;
                        //} else
                        //{
                        //    // pick a adjacent wall and dig it out to create loops.
                        //}

                    }

                }

            }
        }
    }
}