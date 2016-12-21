using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PolygonGenerator : MonoBehaviour {

    [HideInInspector]
	public List<Vector3> newVertices = new List<Vector3>();
    [HideInInspector]
    public List<int> newTriangles = new List<int>();
    [HideInInspector]
    public List<Vector2> newUV = new List<Vector2>();

    const int TILESIZE = 16;
    int tilesX;
    int tilesY;
    float tUnitX;
    float tUnitY;

	private Mesh mesh;

	int TileCount;

	int[,] Tiles;
	
	// Use this for initialization
	void Start () {

		mesh = GetComponent<MeshFilter>().mesh;
        Texture tex = GetComponent<Renderer>().material.GetTexture("_MainTex");

        tilesX = tex.width / TILESIZE;//width in tiles.
        tilesY = tex.height / TILESIZE;// height in tiles.

        tUnitX = (float)TILESIZE / tex.width;//width of one tile in Texture Units.
        tUnitY = (float)TILESIZE / tex.height;//height of one tile in Texture Units.

        Generate();
	
	}

    public void Generate ()
    {
        Tiles = MapGenerator.instance.Map;
        BuildMesh();
        RenderMesh();
    }

    void GenTile(int x, int y, int ID)
    {

        newVertices.Add(new Vector3(x, y, 0));
        newVertices.Add(new Vector3(x + 1, y, 0));
        newVertices.Add(new Vector3(x + 1, y - 1, 0));
        newVertices.Add(new Vector3(x, y - 1, 0));

        newTriangles.Add(TileCount * 4);
        newTriangles.Add((TileCount * 4) + 1);
        newTriangles.Add((TileCount * 4) + 3);
        newTriangles.Add((TileCount * 4) + 1);
        newTriangles.Add((TileCount * 4) + 2);
        newTriangles.Add((TileCount * 4) + 3);

        Vector2 vMin = TileMin(ID);

        newUV.Add(new Vector2(vMin.x, vMin.y + tUnitY));
        newUV.Add(new Vector2(vMin.x + tUnitX, vMin.y + tUnitY));
        newUV.Add(new Vector2(vMin.x + tUnitX, vMin.y));
        newUV.Add(new Vector2(vMin.x, vMin.y));

        TileCount++;
    }

    void RenderMesh() {
		mesh.Clear();
		mesh.vertices = newVertices.ToArray();
		mesh.triangles = newTriangles.ToArray();
		mesh.uv = newUV.ToArray();
		mesh.Optimize();
		mesh.RecalculateNormals();

		TileCount=0;
		newVertices.Clear();
		newTriangles.Clear();
		newUV.Clear();
	}

	void BuildMesh(){
		for(int px=0;px<Tiles.GetLength(0);px++){
			for(int py=0;py<Tiles.GetLength(1);py++){

                if (Tiles[px, py] == 0)
                {
                    //GenTile(px,py,tStone);
                }
                else
                {
                    //GenTile(px, py, Tiles[px, py]);
                    GenTile(px, py, 11);
                }

            }
		}
	}

    Vector2 TileMin (int id)
    {
        return new Vector2((id % tilesX) / (float)tilesX, (id / tilesX) / (float)tilesY);
    }
}
