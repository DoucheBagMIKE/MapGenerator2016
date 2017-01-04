using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ClipperLib;

public class MapChunk
{
    public static int chunkSize = 32;

    public Dictionary<string, Layer> layers;
    public IntPoint pos;
    public GameObject gameobject;
    public PolygonCollider2D collider;

    public MapChunk(int x, int y) { 
        gameobject = new GameObject(string.Format("{0},{1}", x, y));
        pos = new IntPoint(x, y); // in chunk corrs.
        gameobject.transform.position.Set(x * chunkSize, y * chunkSize, 0);

        GameObject go = new GameObject("Collision");
        collider = go.AddComponent<PolygonCollider2D>();
        go.transform.parent = gameobject.transform;

        layers = new Dictionary<string, Layer>();

        foreach(string layerName in MapData.BaseLayers) {
            AddLayer(layerName);
        }
    }

    public Layer AddLayer (string Name)
    {
        Layer L = new Layer(gameobject, Name);
        layers.Add(Name, L);
        return L;
    }

    public void SetTile(int x, int y, int TID, string layerName)
    {
        MapGenerator.instance.Map.layer[layerName][y, x] = TID;

        ChunkManager.Dirty(this);

    }

}