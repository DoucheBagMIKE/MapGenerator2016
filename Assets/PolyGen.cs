using UnityEngine;
using System.Collections.Generic;
using ClipperLib;

public static class PolyGen
{

    static Clipper clipperObj = new Clipper();

    static int[,] map;

    static List<List<IntPoint>> solution = new List<List<IntPoint>>();

    public static void Generate(MapChunk chunk)
    {
        TmxFile Tmx = MapGenerator.instance.Tmx;
        map = MapGenerator.instance.Map;
        string curSubLayerName;

        for (int px = (int)chunk.pos.X; px < chunk.pos.X + MapChunk.chunkSize; px++)
        {
            if (px >= map.GetLength(0))
                continue;

            for (int py = (int)chunk.pos.Y; py < chunk.pos.Y + MapChunk.chunkSize; py++)
            {
                if (py >= map.GetLength(1))
                    continue;

                int? tID = Tmx.tileIdToTilesetId(map[px, py]);

                if (tID == null)
                {
                    Debug.Log("Tile ID doesnt belong to any tileset.");
                    return;
                }

                curSubLayerName = Tmx.tileset[(int)tID].name;

                SubLayer subLayer = chunk.layers["Test"].getSubLayer(curSubLayerName);

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
                    GenTile(px, py, map[px, py], subLayer);

                    List<IntPoint> collInfo = MapGenerator.instance.Tmx.getTileColliderInfo(map[px, py] - 1);
                    for (int i = 0; i < collInfo.Count; i++)
                    {
                        collInfo[i] = new IntPoint((collInfo[i].X / 16) + px, (collInfo[i].Y / 16) + py);
                    }

                    if (collInfo.Count > 2)
                    {
                        clipperObj.AddPath(collInfo, PolyType.ptSubject, true);
                    }

                }
            }
        }

        clipperObj.Execute(ClipType.ctUnion, solution);

        chunk.collider.pathCount = solution.Count;

        for (int i = 0; i < solution.Count; i++)
        {
            List<Vector2> p = new List<Vector2>();

            foreach (IntPoint vert in solution[i])
            {
                p.Add(new Vector2(vert.X, vert.Y));
            }

            chunk.collider.SetPath(i, p.ToArray());
        }

        foreach (string name in chunk.layers["Test"].subLayerNames())
        {
            SubLayer sub = chunk.layers["Test"].getSubLayer(name);

            sub.mesh.Clear();
            sub.mesh.vertices = sub.newVertices.ToArray();
            sub.mesh.triangles = sub.newTriangles.ToArray();
            sub.mesh.uv = sub.newUV.ToArray();
            sub.mesh.Optimize();
            sub.mesh.RecalculateNormals();

            sub.newVertices.Clear();
            sub.newTriangles.Clear();
            sub.newUV.Clear();
            sub.TileCount = 0;
        }

        solution.Clear();
        clipperObj.Clear();
    }

    static void GenTile(long x, long y, int ID, SubLayer sub)
    {
        int tID = (int)MapGenerator.instance.Tmx.tileIdToTilesetId(ID);
        
        sub.newVertices.Add(new Vector3(x, y, 0));
        sub.newVertices.Add(new Vector3(x + 1, y, 0));
        sub.newVertices.Add(new Vector3(x + 1, y + 1, 0));
        sub.newVertices.Add(new Vector3(x, y + 1, 0));

        sub.newTriangles.Add(sub.TileCount * 4);
        sub.newTriangles.Add((sub.TileCount * 4) + 1);
        sub.newTriangles.Add((sub.TileCount * 4) + 3);
        sub.newTriangles.Add((sub.TileCount * 4) + 1);
        sub.newTriangles.Add((sub.TileCount * 4) + 2);
        sub.newTriangles.Add((sub.TileCount * 4) + 3);

        Vector2 vMin = TileMin(ID, sub);
        sub.newUV.Add(new Vector2(vMin.x, vMin.y + sub.texUnitY));
        sub.newUV.Add(new Vector2(vMin.x + sub.texUnitX, vMin.y + sub.texUnitY));
        sub.newUV.Add(new Vector2(vMin.x + sub.texUnitX, vMin.y));
        sub.newUV.Add(new Vector2(vMin.x, vMin.y));

        sub.TileCount++;

        TmxFile tmx = MapGenerator.instance.Tmx;
        int firstGID = tmx.tileset[(int)tmx.tileIdToTilesetId(ID)].firstgid;
        int lID = ID - firstGID;

        Debug.Log(string.Format("GID : {0}  TID : {1}", ID, tID));
        Debug.Log(string.Format("VMIN : {0},{1}", vMin.x, vMin.y));
        Debug.Log(string.Format("LocalID : {0}", lID));
    }

    public static Vector2 TileMin(int id, SubLayer sub)// this doesnt work right i dont think. needs to take into account the other tilesets.
    {
        TmxFile tmx = MapGenerator.instance.Tmx;
        int firstGID = tmx.tileset[(int)tmx.tileIdToTilesetId(id)].firstgid;
        id = id - firstGID;s
        return new Vector2((id % sub.tWidth) / (float)sub.tWidth, (id / sub.tWidth) / (float)sub.tHeight);
    }
}

