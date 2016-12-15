using UnityEngine;
using System.Collections.Generic;

 class Circle
{
    public float radius;
    public Vector2 centerPos;

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
}

public class LayoutGenerator : MonoBehaviour {
    System.Random rng;
    Graph Connections;
    Dictionary<int, Circle> Zones;

    public int maxZones;
    public int maxConnectionsPerZone;
    public float minZoneRadius;
    public float maxZoneRadius;

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
            print(RVector.x.ToString() + " " + RVector.y.ToString());
            
        }

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

            int numberOfConnections = rng.Next(maxConnectionsPerZone);

            while(Connections.connectionCount(sID) != numberOfConnections)
            {
                Vector2 RVector = new Vector2(Rand(), Rand()).normalized;
                float radius = (float)(rng.NextDouble() * maxZoneRadius) + minZoneRadius;

                RVector =  Zones[sID].centerPos + (RVector * (Zones[sID].radius + radius));

                Circle c = new Circle(RVector, radius);


            }
        }


    }

    float Rand ()
    {
        return (float)System.Math.Round((rng.NextDouble()  * 2f) - 1f, 2); 
    }
}
