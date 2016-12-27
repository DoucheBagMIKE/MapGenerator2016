using UnityEngine;
using System.Collections.Generic;
using ClipperLib;

public static class PolyGen
{
    public static Dictionary<int, List<Vector3>> newVertices = new Dictionary<int, List<Vector3>>();
    public static Dictionary<int, List<int>> newTriangles = new Dictionary<int, List<int>>();
    public static Dictionary<int, List<Vector2>> newUV = new Dictionary<int, List<Vector2>>();
    static Dictionary<int, int> TileCount = new Dictionary<int, int>();

    static Clipper clipperObj = new Clipper();

    const int TILESIZE = 16;
    static int  tilesX;
    static int tilesY;
    static float tUnitX;
    static float tUnitY;

    static Mesh mesh;

    static int[,] map;

    static List<List<IntPoint>> solution = new List<List<IntPoint>>();

    public static void Generate(MapChunk chunk)
    {
        map = MapGenerator.instance.Map;

        for (long px = chunk.pos.X; px < chunk.pos.X + MapChunk.chunkSize; px++)
        {
            for (long py = chunk.pos.Y; px < chunk.pos.Y + MapChunk.chunkSize; py++)
            {
                if(px >= map.GetLength(0) || py >= map.GetLength(1))
                {
                    continue;
                }

                int? tID = MapGenerator.instance.Tmx.tileIdToTilesetId(map[px, py]);
                
                if (tID == null)
                {
                    Debug.Log("Tile ID doesnt belong to any tileset.");
                    return;
                }

                if (!TileCount.ContainsKey((int)tID))
                {
                    TileCount.Add((int)tID, 0);
                    newVertices.Add((int)tID, new List<Vector3>());
                    newTriangles.Add((int)tID, new List<int>());
                    newUV.Add((int)tID, new List<Vector2>());
                }


                mesh = chunk.getMesh((int)tID);
                Texture tex = chunk.getRenderer((int)tID).material.GetTexture("_MainTex");

                tilesX = tex.width / TILESIZE;//width in tiles.
                tilesY = tex.height / TILESIZE;// height in tiles.

                tUnitX = (float)TILESIZE / tex.width;//width of one tile in Texture Units.
                tUnitY = (float)TILESIZE / tex.height;//height of one tile in Texture Units.


                if (map[px, py] == 0)
                {
                    List<IntPoint> points = new List<IntPoint>();
                    points.Add(new IntPoint(px, py + 1));
                    points.Add(new IntPoint(px, py));
                    points.Add(new IntPoint(px + 1, py));
                    points.Add(new IntPoint(px + 1, py + 1));

                    clipperObj.AddPath(points, PolyType.ptSubject, true);

                }

                else
                {
                    GenTile(px, py, map[px, py]);

                    List<IntPoint> collInfo = MapGenerator.instance.Tmx.getTileColliderInfo(map[px, py] - 1);
                    for (int i = 0; i < collInfo.Count; i++)
                    {
                        collInfo[i] = new IntPoint((collInfo[i].X / 16) + px, (collInfo[i].Y / 16) + py);
                    }

                    if (collInfo.Count > 2)
                        clipperObj.AddPath(collInfo, PolyType.ptSubject, true);
                }
            }
        }

        clipperObj.Execute(ClipType.ctUnion, solution);

        PolygonCollider2D coll = chunk.getCollider();
        coll.pathCount = solution.Count;

        for (int i = 0; i < solution.Count; i++)
        {
            List<Vector2> p = new List<Vector2>();

            foreach (IntPoint vert in solution[i])
            {
                p.Add(new Vector2(vert.X, vert.Y));
            }

            coll.SetPath(i, p.ToArray());
        }

        foreach(int ID in chunk.getIDs())
        {
            mesh = chunk.getMesh(ID);

            mesh.Clear();
            mesh.vertices = newVertices[ID].ToArray();
            mesh.triangles = newTriangles[ID].ToArray();
            mesh.uv = newUV[ID].ToArray();
            mesh.Optimize();
            mesh.RecalculateNormals();
        }
       
        TileCount.Clear();
        newVertices.Clear();
        newTriangles.Clear();
        newUV.Clear();

        solution.Clear();
    }

    static void GenTile(long x, long y, int ID)
    {
        int tID = (int)MapGenerator.instance.Tmx.tileIdToTilesetId(ID);


        newVertices[tID].Add(new Vector3(x, y, 0));
        newVertices[tID].Add(new Vector3(x + 1, y, 0));
        newVertices[tID].Add(new Vector3(x + 1, y + 1, 0));
        newVertices[tID].Add(new Vector3(x, y + 1, 0));

        newTriangles[tID].Add(TileCount[tID] * 4);
        newTriangles[tID].Add((TileCount[tID] * 4) + 1);
        newTriangles[tID].Add((TileCount[tID] * 4) + 3);
        newTriangles[tID].Add((TileCount[tID] * 4) + 1);
        newTriangles[tID].Add((TileCount[tID] * 4) + 2);
        newTriangles[tID].Add((TileCount[tID] * 4) + 3);

        Vector2 vMin = TileMin(ID);

        newUV[tID].Add(new Vector2(vMin.x, vMin.y + tUnitY));
        newUV[tID].Add(new Vector2(vMin.x + tUnitX, vMin.y + tUnitY));
        newUV[tID].Add(new Vector2(vMin.x + tUnitX, vMin.y));
        newUV[tID].Add(new Vector2(vMin.x, vMin.y));

        TileCount[tID]++;
    }
    static Vector2 TileMin(int id)
    {
        id = id - 1;
        return new Vector2((id % tilesX) / (float)tilesX, (id / tilesX) / (float)tilesY);
    }
}
