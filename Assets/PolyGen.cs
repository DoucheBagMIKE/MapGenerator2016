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
        map = MapGenerator.instance.Map.layer[MapData.BaseLayers[0]];
        string curSubLayerName;

        int cx = (int)chunk.pos.X * MapChunk.chunkSize;
        int cy = (int)chunk.pos.Y * MapChunk.chunkSize;
        for (int py = cy; py < cy + MapChunk.chunkSize; py++)
        {
            if (py >= map.GetLength(0))
                break;

            for (int px = cx; px < cx + MapChunk.chunkSize; px++)
            {
                if (px >= map.GetLength(1))
                    break;

                int? tID = Tmx.tileIdToTilesetId(map[py, px]);

                if (tID == null)
                {
                    Debug.Log("Tile ID doesnt belong to any tileset.");
                    return;
                }

                curSubLayerName = Tmx.tileset[(int)tID].name;

                SubLayer subLayer = chunk.layers[MapData.BaseLayers[0]].getSubLayer(curSubLayerName);

                if (map[py, px] == 0)
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
                    GenTile(px, py, map[py, px], subLayer);

                    List<IntPoint> collInfo = MapGenerator.instance.Tmx.getTileColliderInfo(map[py, px] - 1);
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

        foreach(string layerName in MapData.BaseLayers)
        {
            foreach (string name in chunk.layers[layerName].subLayerNames())
            {
                SubLayer sub = chunk.layers[layerName].getSubLayer(name);

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
        }

        solution.Clear();
        clipperObj.Clear();
    }

    static void GenTile(long x, long y, int ID, SubLayer sub)
    {
        int tID = (int)MapGenerator.instance.Tmx.tileIdToTilesetId(ID);

        sub.newVertices.Add(new Vector3(x, y, 0));// (0,0) 0
        sub.newVertices.Add(new Vector3(x, y + 1, 0));// (0,1) 1
        sub.newVertices.Add(new Vector3(x + 1, y + 1, 0));// (1,1) 2
        sub.newVertices.Add(new Vector3(x + 1, y, 0));// (1,0) 3

        sub.newTriangles.Add((sub.TileCount * 4) + 0);
        sub.newTriangles.Add((sub.TileCount * 4) + 1);
        sub.newTriangles.Add((sub.TileCount * 4) + 2);
        sub.newTriangles.Add((sub.TileCount * 4) + 0);
        sub.newTriangles.Add((sub.TileCount * 4) + 2);
        sub.newTriangles.Add((sub.TileCount * 4) + 3);

        Vector2 vMin = TileMin(ID, sub);
        sub.newUV.Add(new Vector2(vMin.x, vMin.y));
        sub.newUV.Add(new Vector2(vMin.x, vMin.y + sub.texUnitY));
        sub.newUV.Add(new Vector2(vMin.x + sub.texUnitX, vMin.y + sub.texUnitY));
        sub.newUV.Add(new Vector2(vMin.x + sub.texUnitX, vMin.y));

        sub.TileCount++;

        TmxFile tmx = MapGenerator.instance.Tmx;
        int firstGID = tmx.tileset[(int)tmx.tileIdToTilesetId(ID)].firstgid;
        int lID = ID - firstGID;
    }

    public static Vector2 TileMin(int id, SubLayer sub)
    {
        TmxFile tmx = MapGenerator.instance.Tmx;
        int firstGID = tmx.tileset[(int)tmx.tileIdToTilesetId(id)].firstgid;
        id = id - firstGID;
        return new Vector2((id % sub.tWidth) / (float)sub.tWidth, (id / sub.tWidth) / (float)sub.tHeight);
    }
}

