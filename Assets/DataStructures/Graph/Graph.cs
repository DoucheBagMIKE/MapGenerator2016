using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Graph {
    int nextID = 0;
    Dictionary<int, List<int>> adjecencyList;
    public int Count
    {
        get
        {
            return adjecencyList.Count;
        }
    }
    public List<int> keys
    {
        get
        {
            List<int> keys = new List<int>();
            foreach(int k in adjecencyList.Keys)
            {
                keys.Add(k);
            }
            return keys;
        }
    }

    public Graph ()
    {
        adjecencyList = new Dictionary<int, List<int>>();
    }

    public List<int> GetAdjecent (int id)
    {
        return adjecencyList[id];
    }

    public void addConnection (int id1, int id2)
    {
        adjecencyList[id1].Add(id2);
        adjecencyList[id2].Add(id1);
    }
    public void removeConnection (int id1, int id2)
    {
        adjecencyList[id1].Remove(id2);
        adjecencyList[id2].Remove(id1);
    }

    public int createNode ()
    {
        nextID++;
        adjecencyList.Add(nextID, new List<int>());
        return nextID;

    }
    public int createNode(List<int> connections)
    {
        nextID++;
        adjecencyList.Add(nextID, new List<int>());
        foreach (int id2 in connections)
        {
            addConnection(nextID, id2);
        }
        return nextID;
    }

    public void removeNode (int id)
    {
        foreach (int id2 in adjecencyList[id])
        {
            adjecencyList[id2].Remove(id);
        }
        adjecencyList.Remove(id);
    }

    public int connectionCount(int id)
    {
        return adjecencyList[id].Count;
    }

    public bool hasConnection (int id1, int id2)
    {
        return GetAdjecent(id1).Contains(id2);
    }
}
