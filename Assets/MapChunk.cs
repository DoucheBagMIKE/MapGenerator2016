using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ClipperLib;

//public class MapChunk {
//    public static int chunkSize = 32;

//    public IntPoint pos;
//    public GameObject gameobject;
//    Dictionary<int, MeshFilter> mesh;
//    Dictionary<int, MeshRenderer> renderer;
//    PolygonCollider2D collider;

//    public MapChunk (int x, int y)
//    {
//        pos = new IntPoint(x, y);
//        mesh = new Dictionary<int, MeshFilter>();
//        renderer = new Dictionary<int, MeshRenderer>();
//    }

//    public Dictionary<int, MeshFilter>.KeyCollection getIDs ()
//    {
//        return mesh.Keys;
//    }

//    void CreateSubLayer (int tID)
//    {
//        GameObject go = new GameObject(MapGenerator.instance.Tmx.tileset[tID].name);
//        go.transform.parent = gameobject.transform;

//        MeshFilter filter = go.AddComponent<MeshFilter>();

//        MeshRenderer rend = go.AddComponent<MeshRenderer>();
//        rend.material = Resources.Load<Material>(go.name);

//        mesh.Add(tID, filter);
//        renderer.Add(tID, rend); 
//    }

//    public Mesh getMesh (int ID)
//    {
//        if(!mesh.ContainsKey(ID))
//        {
//            CreateSubLayer(ID);
//        }
//        return mesh[ID].mesh;

//    }
//    public MeshRenderer getRenderer (int ID)
//    {
//        if (!renderer.ContainsKey(ID))
//        {
//            CreateSubLayer(ID);
//        }
//        return renderer[ID];
//    }
//    public PolygonCollider2D getCollider ()
//    {
//        if (collider == null)
//        {
//            GameObject go = new GameObject("Collision");
//            collider = go.AddComponent<PolygonCollider2D>();
//            go.transform.parent = gameobject.transform;
//        }

//        return collider;
//    }
//}

public class MapChunk
{
    public static int chunkSize = 32;

    public Dictionary<string, Layer> layers;
    public IntPoint pos;
    public GameObject gameobject;
    public PolygonCollider2D collider;

    public MapChunk(int x, int y)
    {
        gameobject = new GameObject(string.Format("{0},{1}", x, y));
        pos = new IntPoint(x, y);
        gameobject.transform.position.Set(x, y, 0);

        GameObject go = new GameObject("Collision");
        collider = go.AddComponent<PolygonCollider2D>();
        go.transform.parent = gameobject.transform;

        layers = new Dictionary<string, Layer>();

        AddLayer("Test");
    }

    public Layer AddLayer (string Name)
    {
        Layer L = new Layer(gameobject, Name);
        layers.Add(Name, L);
        return L;
    }

}