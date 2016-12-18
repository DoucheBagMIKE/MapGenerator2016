using UnityEngine;
using System.Collections.Generic;
using QuadTree;

  public class Circle : QuadTree.IHasRectangle
{
    public float radius;
    public Vector2 centerPos;
    public UnityEngine.Rect Rect
    {
        get
        {
            return new Rect(centerPos.x - radius, centerPos.y - radius, radius * 2, radius * 2);
        }
    }

    public Circle (Vector2 CenterPos, float Radius)
    {
        centerPos = CenterPos;
        radius = Radius;
    }

    public static bool Intersect(Circle c1, Circle c2)
    {

        //ABS(R0 - R1) <= SQRT((x0 - x1) ^ 2 + (y0 - y1) ^ 2) <= (R0 + R1)
        float dist = Mathf.Sqrt(Mathf.Pow((c1.centerPos.x - c2.centerPos.x), 2) + Mathf.Pow((c1.centerPos.y - c2.centerPos.y), 2));
        if (dist > (c1.radius + c2.radius))
        {
            // No overlap
            return false;
        }
        else if (dist <= Mathf.Abs(c1.radius - c2.radius))
        {
            //inside circle
            return true;
        }
        else              // if (distance <= r1 + r2)
        {
            //overlaps circle
            return true;
        }
    }

    public static bool pointInCircle (Circle circle, float x, float y)
    {
        return Mathf.Pow(x - circle.centerPos.x, 2) + Mathf.Pow(y - circle.centerPos.y, 2) < Mathf.Pow(circle.radius, 2);
    }
}

public class LayoutGenerator : MonoBehaviour {
    System.Random rng;
    Graph Connections;
    Dictionary<int, Circle> Zones;

    public int maxZones;
    public int maxConnectionsPerZone;
    public float minZoneRadius;
    public float maxZoneRadius;

    public QuadTree<Circle> qTree;

    public int CreateZone(Vector2 cPos, float radius)
    {
        int nID = Connections.createNode();
        Zones.Add(nID, new Circle(cPos, radius));
        return nID;
    }
	// Use this for initialization
	void Awake () {
        rng = MapGenerator.instance.Rng;

        Connections = new Graph();
        Zones = new Dictionary<int, Circle>();

        //create the quadtree
        qTree = new QuadTree<Circle>(new Rect(Vector2.zero, new Vector2(MapGenerator.instance.width, MapGenerator.instance.height)));
	}

    public void Generate ()

    {
        // finge shoule be a list so we can randomly pick any of the items on the fringe. should effect how the levels look. worth a try,
        Queue<int> fringe = new Queue<int>();

        int width = MapGenerator.instance.width;
        int height = MapGenerator.instance.height;

        float sx = (float)rng.NextDouble() * width;
        float sy = (float)rng.NextDouble() * height;

        // create and add the first zone to the qTree
        int id = Connections.createNode();
        Circle sC = new Circle(-Vector2.one, (float)(rng.NextDouble() * (maxZoneRadius - minZoneRadius)) + minZoneRadius);

        while (!Utility.Contains(qTree.QuadRect, sC)) {
            sC.centerPos.Set(rng.Next(0, width), rng.Next(0, height));
        }

        qTree.Insert(sC);
        Zones.Add(id, sC);
        fringe.Enqueue(id);

        while (fringe.Count != 0)
        {
            int sID = fringe.Dequeue();
            int trys = 30;

            while (Connections.connectionCount(sID) != maxConnectionsPerZone && trys > 0)
            {
                trys--;
                Vector2 RVector = new Vector2(Rand(), Rand()).normalized;
                float radius = (float)(rng.NextDouble() * (maxZoneRadius - minZoneRadius)) + minZoneRadius;

                RVector = Zones[sID].centerPos + (RVector * (Zones[sID].radius + radius));

                Circle c = new Circle(RVector, radius);

                List<Circle> res = new List<Circle>();;
                qTree.GetObjects(new Rect(RVector.x - radius, RVector.y - radius, radius * 2, radius * 2), ref res);

                bool collision = false;
                foreach (Circle circle in res)
                {
                    if (Circle.Intersect(c, circle))
                    {
                        collision = true;
                        break;
                    }
                }
                // if the new circle is in the rect of the quad tree and threres no collisions with other circles,
                // we add the new circle to the quadtree, create a new node in the graph, and store the circle in the zones dictionary with the nodes id.
                // then finaly we add a connection between the 2 circles.(a connection stores each nodes id in the conections dictionary so we can look up what each nodes individual connections are.)
                 if (Utility.Contains(qTree.QuadRect, c) && !collision)
                {
                    qTree.Insert(c);

                    int nID = Connections.createNode();
                    Zones.Add(nID, c);

                    Connections.addConnection(sID, nID);

                    fringe.Enqueue(nID);
                }


            }
        }
        //A* Test
        List<int> pathIDs = new List<int>();
        Utility.GetPath(Connections.keys[rng.Next(0, Connections.Count + 1)] , Connections.keys[rng.Next(0, Connections.Count + 1)], Connections, Zones, ref pathIDs);
        foreach (int i in pathIDs)
        {
            print(i);
        }
        foreach(int nodeID in Connections.keys)
        {
            //foreach(int pID in qTree.GetObjects())
            //{
            //    Utility.GetPath(nodeID, pID, Connections, Zones, ref pathIDs);

            //}
        }
        DrawDebug();
    }

    float Rand ()
    {
        return (float)System.Math.Round((rng.NextDouble()  * 2f) - 1f, 2); 
    }

    public void Test()
    {
        Circle c1 = new Circle(Vector2.zero, 1f);
        Vector2 RVector = new Vector2(Rand(), Rand()).normalized;
        float radius = (float)(rng.NextDouble() * (maxZoneRadius - minZoneRadius)) + minZoneRadius;

        RVector = c1.centerPos + (RVector * (c1.radius + radius));

        Circle c2 = new Circle(RVector, radius);
        foreach (Circle c in new Circle[2]{c1, c2}) 
        {
            GameObject obj = Instantiate(Resources.Load("Circle", typeof(GameObject))) as GameObject;
            obj.transform.localScale = new Vector3(c.radius * 2, c.radius * 2, 1f);
            obj.transform.position = c.centerPos;


        }
    }

    void DrawDebug()
    {
        List<Circle> Res = new List<Circle>();
        //qTree.GetAllObjects(ref Res);
        Dictionary<int, GameObject> c_Objs = new Dictionary<int, GameObject>();
        foreach (int key in Zones.Keys)
        {
            Circle c = Zones[key];
            GameObject obj = Instantiate(Resources.Load("Circle", typeof(GameObject))) as GameObject;
            obj.transform.localScale = new Vector3(c.radius * 2, c.radius * 2, 1f);
            obj.transform.position = new Vector3(c.centerPos.x, c.centerPos.y, 0);
            obj.GetComponent<Renderer>().sortingOrder = 0;

            c_Objs.Add(key, obj);
        }

        List<int> visited = new List<int>();
        Queue<int> f = new Queue<int>();
        f.Enqueue(1);
        while (f.Count != 0)
        {
            int id = f.Dequeue();
            foreach (int cid in Connections.GetAdjecent(id))
            {
                if (visited.Contains(cid))
                {
                    continue;
                }
                LineRenderer lRend = c_Objs[cid].AddComponent<LineRenderer>();
                lRend.SetPositions(new Vector3[2] { Zones[id].centerPos, Zones[cid].centerPos });
                lRend.material = Resources.Load<Material>("DefaultMat");
                lRend.SetColors(Color.red, Color.red);
                lRend.SetWidth(1f, 1f);
                lRend.sortingOrder = 1;
                visited.Add(id);
                if (!f.Contains(cid) && !visited.Contains(cid))
                {
                    f.Enqueue(cid);
                }
            }
        }
    }
}
