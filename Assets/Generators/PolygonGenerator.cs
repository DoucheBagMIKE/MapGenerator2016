using UnityEngine;
using System.Collections;
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

	int[,] Tiles;
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
        //clipperObj.ReverseSolution = true;
        //clipperObj.StrictlySimple = true;
        subj = new List<IntPoint>();
        solution = new List<List<IntPoint>>();
        //clipperObj.AddPath(subj, PolyType.ptSubject, true);

        //Generate();
	
	}

    public void Generate ()
    {
        Tiles = MapGenerator.instance.Map;
        BuildMesh();
        BuildColliders();
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

        List<List<IntPoint>> clip = new List<List<IntPoint>>();
		for(int px=0;px<Tiles.GetLength(0);px++) {
			for(int py=0;py<Tiles.GetLength(1);py++){

                if (Tiles[px, py] == 0)
                {
                    List<IntPoint> points = new List<IntPoint>();
                    points.Add(new IntPoint(px, py - 1));
                    points.Add(new IntPoint(px, py));
                    points.Add(new IntPoint(px + 1, py));
                    points.Add(new IntPoint(px + 1, py - 1));

                    clipperObj.AddPath(points, PolyType.ptSubject, true);

                }

                else
                {
                    GenTile(px, py, 9);
                }

            }
		}

        clipperObj.Execute(ClipType.ctUnion, solution);
    }

    Vector2 TileMin (int id)
    {
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

    void ClipperTest ()
    {
        List<List<IntPoint>> subj = new List<List<IntPoint>>(2);
        subj.Add(new List<IntPoint>(4));
        subj[0].Add(new IntPoint(180, 200));
        subj[0].Add(new IntPoint(260, 200));
        subj[0].Add(new IntPoint(260, 150));
        subj[0].Add(new IntPoint(180, 150));

        subj.Add(new List<IntPoint>(3));
        subj[1].Add(new IntPoint(215, 160));
        subj[1].Add(new IntPoint(230, 190));
        subj[1].Add(new IntPoint(200, 190));

        List<List<IntPoint>> clip = new List<List<IntPoint>>(1);
        clip.Add(new List<IntPoint>(4));
        clip[0].Add(new IntPoint(190, 210));
        clip[0].Add(new IntPoint(240, 210));
        clip[0].Add(new IntPoint(240, 130));
        clip[0].Add(new IntPoint(190, 130));

        List<List<IntPoint>> solution = new List<List<IntPoint>>();

        Clipper c = new Clipper();
        c.AddPaths(subj, PolyType.ptSubject, true);
        c.AddPaths(clip, PolyType.ptClip, true);
        c.Execute(ClipType.ctUnion, solution,
          PolyFillType.pftEvenOdd, PolyFillType.pftEvenOdd);

        foreach(List<IntPoint> poly in solution)
        {
            PolygonCollider2D coll = gameObject.AddComponent<PolygonCollider2D>();
            List<Vector2> p = new List<Vector2>();

            foreach(IntPoint vert in poly)
            {
                p.Add(new Vector2(vert.X, vert.Y));
            }
            coll.points = p.ToArray();
        }

    }
}
