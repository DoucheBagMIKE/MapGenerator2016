using UnityEngine;
using System.Collections.Generic;

enum Direction { Up, Down, Left, Right};


public class StructureGenerator : MonoBehaviour
{
    [HideInInspector]
    public string seed;

    public int structureWidth;
    public int structureHeight;

    public System.Random Rng;
    [HideInInspector]
    public MazeGenerator mazeGen;

    public int[,] Map;

    public int maxRooms;
    public int minRoomSize;
    public int maxRoomSize;
    List<Color32> rColors;
    public int[,] lables;
    public List<List<MapPos>> rooms;


    // Use this for initialization
    void Start()
    {
        Rng = MapGenerator.instance.Rng;
        mazeGen = MapGenerator.instance.mazeGen;
        Map = MapGenerator.instance.Map.layer[MapData.BaseLayers[0].name];

        rColors = new List<Color32>();
        rooms = new List<List<MapPos>>();
    }

    //void OnDrawGizmos()
    //{
    //    if (lables != null)
    //    {
    //        for (int x = 0; x < structureWidth; x++)
    //        {
    //            for (int y = 0; y < structureHeight; y++)
    //            {
    //                switch (Map[x, y])
    //                {
    //                    case 0:
    //                        Gizmos.color = Color.black;
    //                        break;
    //                    case 1:
    //                        if (lables[x,y] == 0)
    //                        {
    //                            Gizmos.color = Color.white;
    //                        }else
    //                        {
    //                            Gizmos.color = rColors[lables[x, y] - 1];
    //                        }
    //                        break;
    //                    default:
    //                        Gizmos.color = Color.white;
    //                        break;
    //                }
    //                Vector3 pos = new Vector3(-structureWidth / 2 + x + .5f, 0, -structureHeight / 2 + y + .5f);
    //                Gizmos.DrawCube(pos, Vector3.one);
    //            }
    //        }
    //    }
    //}

    public void Generate()
    {
        mazeGen = MapGenerator.instance.mazeGen;
        Map = MapGenerator.instance.Map.layer[MapData.BaseLayers[0].name];

        int roomIndex = 1;

        
        while (roomIndex <= maxRooms)
        {
            Rect room = new Rect(-1, -1, 0, 0);

            while (!RectIsValid(room))
            {
                room.width = RandomOdd(minRoomSize, maxRoomSize);
                room.height = RandomOdd(minRoomSize, maxRoomSize);
                room.x = RandomEven(0,structureWidth-Mathf.FloorToInt(room.width) - 1);
                room.y = RandomEven(0, structureHeight - Mathf.FloorToInt(room.height) -1);
                
            }
            for(int x = 0; x < room.width; x++)
            {
                for (int y = 0; y< room.height;y++)
                {
                    int xPos = Mathf.FloorToInt(room.x) + x;
                    int yPos = Mathf.FloorToInt(room.y) + y;

                    if(x == 0 || y == 0|| x == room.width - 1|| y == room.height - 1)
                    {
                        Map[xPos, yPos] = 0;
                    } else
                    {
                        Map[xPos, yPos] = 1;
                    }
                    //mazeGen.Visited[xPos, yPos] = true;
                }
            }
            roomIndex++;
        }
        connectedComponents();
        foreach(List<MapPos> room in rooms) {
            MapPos rPos = room[Rng.Next(0, room.Count - 1)];
            getAdjacentRomms(rPos.x, rPos.y);
        }
    }

    bool RectIsValid(Rect room)
    {

        // Valid Placement basicly checks to see if a room is contained by the map.

        if (room.x + room.width > structureWidth || room.y + room.height > structureHeight || room.x < 0 || room.y < 0)
        {
            return false;
        }
        if (MapGenerator.instance.ca_mask)
        {
            if (MapGenerator.instance.mask[(int)room.x, (int)room.y] == 0)
                return false;
        }

        return true;
    }

    int RandomEven(int min, int max)
    {
        int rVal = Rng.Next(min, max);
        while (rVal % 2 != 0)
        {
            rVal = Rng.Next(min, max);
        }
        return rVal;
    }

    int RandomOdd(int min, int max)
    {
        int rVal = Rng.Next(min, max);
        while (rVal % 2 == 0)
        {
            rVal = Rng.Next(min, max);
        }
        return rVal;
    }

     void getAdjacentRomms (int x, int y)
    {
        MapPos[] adjRooms = new MapPos[4]
        {
            findRoom(x,y,"up"),
            findRoom(x,y,"down"),
            findRoom(x,y,"left"),
            findRoom(x,y,"right")
        };
        string[] rDir = new string[4]
        {
            "up",
            "down",
            "left",
            "right"
        };
        for(int i = 0; i < 4; i++)
        {
            int nx = 0;
            int ny = 0;

            if (adjRooms[i].x == -1)
            {
                continue;
            }
            nx = adjRooms[i].x;
            ny = adjRooms[i].y;
            Map[nx, ny] = 1;
            lables[nx, ny] = 0;
            //  mazeGen.Visited[nx, ny] = false;
        }
    }

    MapPos findRoom(int x, int y, string dir)
    {
        int nx = x; 
        int ny = y;
        int xi = 0;
        int yi = 0;

        MapPos ret = new MapPos(-1, -1);

        switch (dir.ToLower())
        {
            case "up":
                ny++;
                yi++;
                break;
            case "down":
                ny--;
                yi--;
                break;
            case "left":
                nx--;
                xi--;
                break;
            case "right":
                nx++;
                xi++;
                break;
        }
        // if the ajdacent cell is out of the structure bounds return a negitive pos. signaling a fail.
        if (nx < 0 || ny < 0 || nx > structureWidth - 1|| ny > structureHeight - 1) {
            return ret;
        }
        // if the adjacent cell has the same id as the current cell then we just continue on to the next cell in the same direction.
        if (lables[nx,ny] == lables[x,y])
        {
            return findRoom(nx, ny, dir);
        } else
        {

            ret.x = nx;
            ret.y = ny;
            return ret;
        }

    }

    void connectedComponents()
    {
        lables = new int[structureWidth, structureHeight];

        List<MapPos> queue = new List<MapPos>();

        int curLabel = 0;
        List<MapPos> roomdata;

        for (int x = 0; x < structureWidth; x++)
        {
            for(int y = 0;y<structureHeight;y++)
            {
                
                if (Map[x,y] == 1 && lables[x,y] == 0)
                {
                    roomdata = new List<MapPos>();
                    rColors.Add(new Color32((byte)Rng.Next(0, 255), (byte)Rng.Next(0, 255), (byte)Rng.Next(0, 255), 255));
                    curLabel++;
                    lables[x, y] = curLabel;
                    MapPos pos = new MapPos(x, y);
                    queue.Add(pos);
                    roomdata.Add(pos);
                }
                else
                {
                    continue;
                }

                while(queue.Count > 0)
                {
                    MapPos nPos = queue[queue.Count - 1];
                    queue.RemoveAt(queue.Count - 1);

                    MapPos[] neibours = new MapPos[4]
                    {
                        new MapPos(nPos.x, nPos.y+1),
                        new MapPos(nPos.x+1, nPos.y),
                        new MapPos(nPos.x, nPos.y-1),
                        new MapPos(nPos.x-1, nPos.y)
                    };
                    foreach (MapPos neibour in neibours)
                    {
                        if (neibour.x < 0||neibour.y < 0 || neibour.x > structureWidth - 1|| neibour.y > structureHeight - 1)
                        {
                            continue;
                        }
                        if (Map[neibour.x, neibour.y] == 1 && lables[neibour.x, neibour.y] == 0)
                        {
                            lables[neibour.x, neibour.y] = curLabel;
                            queue.Add(neibour);
                            roomdata.Add(neibour);
                        }
                    }
                }
                rooms.Add(roomdata);
            }
        }
    }
}
