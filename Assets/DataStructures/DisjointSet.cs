using UnityEngine;
using System.Collections;

public class DisjointSet {
    // Disjoint Set Forest.

    public static Node MakeSet(Node x)
    {
        x.Parent = x;
        return x;
    }

    public static void Union (Node x, Node y)
    {
        Node xRoot = Find(x);
        Node rRoot = Find(y);
        x.Parent = rRoot;
    }

    public static Node Find (Node x)
    {
        if (x.Parent == x)
        {
            return x;
        } else
        {
            return Find(x.Parent);
        }
    }
}
