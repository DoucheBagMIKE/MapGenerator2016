using UnityEngine;
using System.Collections.Generic;

public class SubLayer {
    static Dictionary<string, Material> Materials = new Dictionary<string, Material>();
    static int TILESIZE = 16;

    public GameObject gameobject;
    public List<Vector3> newVertices;
    public List<int> newTriangles;
    public List<Vector2> newUV;
    public int TileCount;
    public Mesh mesh;

    public int tWidth;//width in tiles.
    public int tHeight;// height in tiles.
    public float texUnitX;//width of one tile in Texture Units.
    public float texUnitY;//height of one tile in Texture Units.

    static Material GetMaterial (string MaterialName)
    {
        if (!Materials.ContainsKey(MaterialName))
        {
            Material mat = Resources.Load<Material>(MaterialName);
            
            if (mat == null)
            {
                Debug.Log(string.Format("No Material Named {0} found in resources.", MaterialName));
                return null;
            }

            Materials.Add(MaterialName, mat);
            return mat;
        }
        else
        {
            return Materials[MaterialName];
        }
    }

    public SubLayer (GameObject parent, string MaterialName)
    {
        TmxFile Tmx = MapGenerator.instance.Tmx;

        gameobject = new GameObject(MaterialName);
        gameobject.transform.parent = parent.transform;

        mesh = gameobject.AddComponent<MeshFilter>().mesh;
        Renderer rend = gameobject.AddComponent<MeshRenderer>();

        Material mat = GetMaterial(MaterialName);
        rend.material = mat;

        tileset t = Tmx.tileset[(int)Tmx.tilesetNametoID(MaterialName)];

        tWidth = t.image.width / TILESIZE;
        tHeight = t.image.height / TILESIZE;
        texUnitX = (float)TILESIZE / t.image.width;
        texUnitY = (float)TILESIZE / t.image.height;

        newVertices = new List<Vector3>();
        newTriangles = new List<int>();
        newUV = new List<Vector2>();
        TileCount = 0;
    }
}
