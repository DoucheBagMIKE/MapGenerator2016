using UnityEngine;
using System.Collections.Generic;

public class MazeGenerator : MonoBehaviour
{
    // Blockwise growing tree algorithom.
    int width;
    int height;
    System.Random Rng;

    public bool[,] Visited;
    List<MapPos> Fringe;

    [Range(0f,1f)]
    public float loopPercent;
    [Range(0f,1f)]
    public float windyOrRandomPercent;

    // Use this for initialization
    void Start()
    {
        Rng = MapGenerator.instance.Rng;
        Visited = new bool[width, height];
        Fringe = new List<MapPos>();
    }

    public void Generate(int sx, int sy, int[,] Map)
    {
        width = Map.GetLength(0);
        height = Map.GetLength(1);

        if (Map[sx,sy] == 1)
        {
            return;
        }

        MapPos start = new MapPos(sx, sy);
        
        Fringe.Add(start);

        while (Fringe.Count > 0)
        {

            MapPos selected = pickNextFringe();
            Map[selected.x, selected.y] = 1;
            Visited[selected.x, selected.y] = true;

            List<MapPos> adjacent = FindFringeNeighbors(selected, 0, Map);
            if (adjacent.Count > 0)
            {
                MapPos adjPos = adjacent[Rng.Next(0, adjacent.Count - 1)];
                if (Rng.NextDouble() >= loopPercent)
                {
                    Map[adjPos.x, adjPos.y] = 1;
                }
                setPassage(selected, adjPos, Map);
                Fringe.Add(adjPos);
            } else
            {
                Fringe.Remove(selected);
            }
        }
        return;
    }

    MapPos pickNextFringe()
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

    public List<MapPos> FindFringeNeighbors(MapPos Pos, int TileState, int[,] Map)
    {
        List<MapPos> ret = new List<MapPos>();

        foreach (MapPos index in Utility.dirs)
        {
            int x = Pos.x + (index.x * 2);
            int y = Pos.y + (index.y * 2);

            
            if (x < 0 || x > width - 1 || y < 0 || y > height - 1)
            {
                continue;
            }

            if (MapGenerator.instance.ca_mask && MapGenerator.instance.mask[x, y] == 0)
            {
                continue;
            }

            if (Map[x, y] == TileState && Visited[x,y] == false)
            {
                ret.Add(new MapPos(x, y));
            }
        }
        return ret;
    }

    void setPassage(MapPos a, MapPos b, int[,] Map)
    {
        int x = (a.x + b.x) / 2;
        int y = (a.y + b.y) / 2;

        Map[x, y] = 1;
    }

    public void Sparsify(int[,] Map)
    {
        int[,] ret = new int[width, height];
        int count;
        int state;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
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
                foreach (MapPos i in Utility.dirs)
                {
                    int nx = i.x + x;
                    int ny = i.y + y;

                    if (nx < 0 || nx > width - 1 || ny < 0 || ny > height - 1)
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