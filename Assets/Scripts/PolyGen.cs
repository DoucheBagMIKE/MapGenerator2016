using UnityEngine;
using System.Collections.Generic;
using ClipperLib;

public static class PolyGen
{
    static Dictionary<string, Clipper> clippers = new Dictionary<string, Clipper>();
    static Dictionary<string, List<List<IntPoint>>> solutions = new Dictionary<string, List<List<IntPoint>>>();
    static int[,] map;

    public static void Generate(MapChunk chunk)
    {
        TmxFile Tmx = MapGenerator.instance.Tmx;
        
        string curSubLayerName;

        int cx = (int)chunk.pos.X * MapChunk.chunkSize;
        int cy = (int)chunk.pos.Y * MapChunk.chunkSize;

        for (int py = cy; py < cy + MapChunk.chunkSize; py++)
        {
            if (py >= MapGenerator.instance.Map.height)
                break;

            for (int px = cx; px < cx + MapChunk.chunkSize; px++)
            {
                if (px >= MapGenerator.instance.Map.width)
                    break;

                foreach(string layername in MapGenerator.instance.Map.layer.Keys)
                {
                    map = MapGenerator.instance.Map.layer[layername];
                    int? tID = Tmx.tileIdToTilesetId(map[py, px]);

                    if (tID == null)
                    {
                        Debug.Log("Tile ID doesnt belong to any tileset.");
                        return;
                    }

                    curSubLayerName = Tmx.tileset[(int)tID].name;

                    SubLayer subLayer = null;

                    if(map[py, px] != 0)
                    {
                        subLayer = chunk.layers[layername].getSubLayer(curSubLayerName);
                    }

                    if (map[py, px] == 0)
                    {
                        if (chunk.layers[layername].Collisions && chunk.layers[layername].emptyTileCollisionRule)
                        {
                            if (!clippers.ContainsKey(layername))
                            {
                                clippers.Add(layername, new Clipper());
                                solutions.Add(layername, new List<List<IntPoint>>());
                            }
                            int x = px;// MapChunk.chunkSize;
                            int y = py;// MapChunk.chunkSize;
                            List<IntPoint> points = new List<IntPoint>();
                            points.Add(new IntPoint(x, y + 1));
                            points.Add(new IntPoint(x, y));
                            points.Add(new IntPoint(x + 1, y));
                            points.Add(new IntPoint(x + 1, y + 1));

                            clippers[layername].AddPath(points, PolyType.ptSubject, true);
                        }
                    }

                    else

                    {
                        if(subLayer != null)
                        {
                            GenTile(px, py, map[py, px], subLayer);
                        }
                        

                        if (chunk.layers[layername].Collisions)
                        {
                            if (!clippers.ContainsKey(layername))
                            {
                                clippers.Add(layername, new Clipper());
                                solutions.Add(layername, new List<List<IntPoint>>());
                            }

                            List<IntPoint> collInfo = MapGenerator.instance.Tmx.getTileColliderInfo(map[py, px]);
                            for (int i = 0; i < collInfo.Count; i++)
                            {
                                collInfo[i] = new IntPoint((collInfo[i].X) + (px * 16), (collInfo[i].Y) + (py * 16));
                            }

                            if (collInfo.Count > 2)
                            {
                                clippers[layername].AddPath(collInfo, PolyType.ptSubject, true);
                            }
                        }
                            
                    }
                }
                
            }
        }

        foreach (string layerName in clippers.Keys)
        {
            clippers[layerName].Execute(ClipType.ctUnion, solutions[layerName]);

            chunk.layers[layerName].collider.pathCount = solutions[layerName].Count;

            for (int i = 0; i < solutions[layerName].Count; i++)
            {
                List<Vector2> p = new List<Vector2>();

                foreach (IntPoint vert in solutions[layerName][i])
                {
                    p.Add(new Vector2((vert.X/16f) - chunk.gameobject.transform.position.x, (vert.Y/16f) - chunk.gameobject.transform.position.y));
                }

                chunk.layers[layerName].collider.SetPath(i, p.ToArray());
            }
        }

        foreach (string layerName in chunk.layers.Keys)
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

        solutions.Clear();
        clippers.Clear();
    }

    static void GenTile(long x, long y, int ID, SubLayer sub)
    {
        //float x = Mathf.Round(lx);
        //float y = Mathf.RoundToInt(ly);
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
    }

    public static Vector2 TileMin(int id, SubLayer sub)
    {
        TmxFile tmx = MapGenerator.instance.Tmx;

        int? idExists = tmx.globalIdToLocalId(id);
        if (idExists != null)
        {
            id = (int)idExists;
        }
        
        float x = (id % sub.tWidth) / (float)sub.tWidth;// X position in tilespace.
        float y = (id / sub.tWidth) / (float)sub.tHeight;// Y position in tilespace.

        y = (1f - y) - sub.texUnitY;// tiled's tilesets cordanates are top left while unity texture cordanates are bottom left
                                    // so  we need to flip the Y position and offset it by one tile.

        return new Vector2(x, y);
    }
}

