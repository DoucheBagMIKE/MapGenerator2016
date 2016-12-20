using UnityEngine;
using System.Collections.Generic;
using QuadTree;

  public class Circle : QuadTree.IHasRectangle
{
    public int ID; 
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

    //public int maxZones;
    public int maxConnectionsPerZone;
    public float minZoneRadius;
    public float maxZoneRadius;
    public float MAXDISTFORCONNECTION;
    public int MINLENGTHOFLOOP;

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

        float sx = (float)rng.Next(0, width + 1);
        float sy = (float)rng.Next(0, height + 1);

        // create and add the first zone to the qTree
        int id = Connections.createNode();
        Circle sC = new Circle(-Vector2.one, Mathf.RoundToInt((float)(rng.NextDouble() * (maxZoneRadius - minZoneRadius)) + minZoneRadius));
        sC.ID = id;

        while (!Utility.Contains(qTree.QuadRect, sC)) {
            sC.centerPos.Set(rng.Next(0, width + 1), rng.Next(0, height + 1));
        }

        qTree.Insert(sC);
        Zones.Add(id, sC);
        fringe.Enqueue(id);

        while (fringe.Count != 0)
        {
            int sID = fringe.Dequeue();
            int trys = 30;

            while (Connections.connectionCount(sID) < maxConnectionsPerZone && trys > 0)
            {
                trys--;
                Vector2 RVector = new Vector2(Rand(), Rand()).normalized;
                float radius = Mathf.RoundToInt((float)(rng.NextDouble() * (maxZoneRadius - minZoneRadius)) + minZoneRadius);

                RVector = Zones[sID].centerPos + (RVector * (Zones[sID].radius + radius));

                Circle c = new Circle(new Vector2(Mathf.RoundToInt(RVector.x), Mathf.RoundToInt(RVector.y)), radius);

                List<Circle> res = new List<Circle>();;
                qTree.GetObjects(new Rect((RVector.x - radius) - MAXDISTFORCONNECTION, (RVector.y - radius) - MAXDISTFORCONNECTION, radius * 2, radius * 2), ref res);

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
                    c.ID = nID;
                    Zones.Add(nID, c);

                    Connections.addConnection(sID, nID);

                    fringe.Enqueue(nID);
                }


            }
        }

        List<int> pathIDs = new List<int>();
        

        foreach(int nodeID in Connections.keys)
        {
            if(Connections.GetAdjecent(nodeID).Count == maxConnectionsPerZone)
            {
                continue;
            }

            Vector2 pos = Zones[nodeID].centerPos;
            float r = Zones[nodeID].radius;

            List<Circle> objs = new List<Circle>();
            qTree.GetObjects(new Rect(pos.x - MAXDISTFORCONNECTION, pos.y - MAXDISTFORCONNECTION, (r * 2) + MAXDISTFORCONNECTION, (r * 2) + MAXDISTFORCONNECTION), ref objs);
            foreach (Circle c in objs)
            {
                if(nodeID == c.ID || Connections.GetAdjecent(nodeID).Contains(c.ID)|| Connections.GetAdjecent(c.ID).Count >= maxConnectionsPerZone)
                {
                    continue;
                }

                float dist = Mathf.Sqrt(Mathf.Pow((Zones[nodeID].centerPos.x - c.centerPos.x), 2) + Mathf.Pow((Zones[nodeID].centerPos.y - c.centerPos.y), 2)) - (Zones[nodeID].radius + c.radius);

                if (dist <= MAXDISTFORCONNECTION)
                {
                    pathIDs.Clear();
                    Utility.GetPath(nodeID, c.ID, Connections, Zones, ref pathIDs);
                    if (pathIDs.Count >= MINLENGTHOFLOOP)
                    {
                        Connections.addConnection(nodeID, c.ID);
                    }
                }
                
            }
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
        float radius = Mathf.Round(((float)rng.NextDouble() * (maxZoneRadius - minZoneRadius)) + minZoneRadius);

        RVector = c1.centerPos + (RVector * (c1.radius + radius));
        RVector.Set(Mathf.Round(RVector.x), Mathf.Round(RVector.y));

        Circle c2 = new Circle(RVector, radius);

        Vector2 v = c1.centerPos - c2.centerPos;

        //dist between 2 circles;
        float x = Mathf.Pow((c1.centerPos.x - c2.centerPos.x), 2);
        float y = Mathf.Pow((c1.centerPos.y - c2.centerPos.y), 2);
        float r = (c1.radius + c2.radius);

        float dist = Mathf.Sqrt(x+y) - r;

        print(dist);
        //c2.radius -= Mathf.Abs(dist);

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
        List<LineRenderer> lines = new List<LineRenderer>();
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
                GameObject lObj = new GameObject();
                LineRenderer lRend = lObj.AddComponent<LineRenderer>();
                lObj.transform.parent = c_Objs[cid].transform;

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
