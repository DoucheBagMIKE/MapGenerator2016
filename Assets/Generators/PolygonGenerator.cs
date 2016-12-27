using UnityEngine;
using System.Collections.Generic;
using ClipperLib;

public class PolygonGenerator : MonoBehaviour {

    [HideInInspector]
	public List<Vector3> newVertices = new List<Vector3>();
    [HideInInspector]
    public List<int> newTriangles = new List<int>();
    [HideInInspector]
    public List<Vector2> newUV = new List<Vector2>();

    Clipper clipperObj;
    const int TILESIZE = 16;
    int tilesX;
    int tilesY;
    float tUnitX;
    float tUnitY;

	private Mesh mesh;

	int TileCount;

	int[,] map;
    List<IntPoint> subj;
    List<List<IntPoint>> solution;

    // Use this for initialization
    void Start () {

		mesh = GetComponent<MeshFilter>().mesh;
        Texture tex = GetComponent<Renderer>().material.GetTexture("_MainTex");

        tilesX = tex.width / TILESIZE;//width in tiles.
        tilesY = tex.height / TILESIZE;// height in tiles.

        tUnitX = (float)TILESIZE / tex.width;//width of one tile in Texture Units.
        tUnitY = (float)TILESIZE / tex.height;//height of one tile in Texture Units.

        clipperObj = new Clipper();
        solution = new List<List<IntPoint>>();
	}

    public void Generate ()
    {
        map = MapGenerator.instance.Map;

        BuildMesh();
        BuildColliders();
        RenderMesh();
    }

    void GenTile(int x, int y, int ID)
    {

        newVertices.Add(new Vector3(x, y, 0));
        newVertices.Add(new Vector3(x + 1, y, 0));
        newVertices.Add(new Vector3(x + 1, y + 1, 0));
        newVertices.Add(new Vector3(x, y + 1, 0));

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

        List<List<IntPoint>> clip = new List<List<IntPoint>>();

		for(int px=0;px<map.GetLength(0);px++) {
			for(int py=0;py<map.GetLength(1);py++){

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
                    GenTile(px, py, map[px,py]);

                    List<IntPoint> collInfo = MapGenerator.instance.Tmx.getTileColliderInfo(map[px, py] - 1);
                    for(int i = 0; i < collInfo.Count; i++)
                    {
                        collInfo[i] = new IntPoint((collInfo[i].X / 16) + px, (collInfo[i].Y / 16) + (py));
                    }

                    if (collInfo.Count > 0)
                        clipperObj.AddPath(collInfo, PolyType.ptSubject, true);
                }

            }
		}

        clipperObj.Execute(ClipType.ctUnion, solution);
    }

    Vector2 TileMin (int id)
    {
        id = id - 1;
        return new Vector2((id % tilesX) / (float)tilesX, (id / tilesX) / (float)tilesY);
    }

    void BuildColliders ()
    {
        PolygonCollider2D coll = gameObject.AddComponent<PolygonCollider2D>();
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
    }
}
