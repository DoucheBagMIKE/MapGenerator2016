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
	void Start () {
        rng = MapGenerator.instance.Rng;

        Connections = new Graph();
        Zones = new Dictionary<int, Circle>();

        int za = CreateZone(Vector2.zero, 1f);
        int zb = CreateZone(new Vector2(2f, 2f), 1f);

        Vector2 v = Zones[zb].centerPos - Zones[za].centerPos;
        v.Normalize();

        Connections.addConnection (za, zb);

        for(int i = 0; i < 1000;i++)
        {
            Vector2 RVector = new Vector2(Rand(), Rand()).normalized;
            //print(RVector.x.ToString() + " " + RVector.y.ToString());
            
        }
        //create the quadtree
        qTree = new QuadTree<Circle>(new Rect(Vector2.zero, new Vector2(MapGenerator.instance.width, MapGenerator.instance.height)));

        Generate();
	}

    void Generate ()
    {
        Queue<int> fringe = new Queue<int>();
        int width = MapGenerator.instance.width;
        int height = MapGenerator.instance.height;

        float sx = (float)rng.NextDouble() * width;
        float sy = (float)rng.NextDouble() * height;

        fringe.Enqueue(CreateZone(new Vector2(sx, sy), 1.5f));

        while (fringe.Count != 0)
        {
            int sID = fringe.Dequeue();

            int numberOfConnections = 1;//rng.Next(maxConnectionsPerZone);

            while (Connections.connectionCount(sID) != numberOfConnections)
            {
                Vector2 RVector = new Vector2(Rand(), Rand()).normalized;
                float radius = (float)(rng.NextDouble() * (maxZoneRadius - minZoneRadius)) + minZoneRadius;

                RVector = Zones[sID].centerPos + (RVector * (Zones[sID].radius + radius));

                Circle c = new Circle(RVector, radius);
                List<Circle> res = new List<Circle>();

                qTree.GetObjects(new Rect(RVector.x - radius, RVector.y - radius, radius, radius), ref res);
                bool collision = false;
                foreach (Circle circle in res)
                {
                    if (Circle.Intersect(c, circle))
                    {
                        collision = true;
                        break;
                    }
                } 
                 if (Utility.Contains(qTree.QuadRect, c) && !collision)
                {
                    qTree.Insert(c);

                    int nID = Connections.createNode();
                    Zones.Add(nID, c);

                    Connections.addConnection(sID, nID);
                }


            }
        }

        print(qTree.Count);


    }

    float Rand ()
    {
        return (float)System.Math.Round((rng.NextDouble()  * 2f) - 1f, 2); 
    }
}
