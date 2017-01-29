using UnityEngine;
using System.Collections.Generic;

public class Layer {

    public bool Collisions = false;
    public bool emptyTileCollisionRule = false;
    public bool AutoTile = false;

    Dictionary<string, SubLayer> subLayers;

    public GameObject gameobject;
    public GameObject collision;
    public PolygonCollider2D collider;

    public Layer (GameObject Parent, string Name, bool UseCollision= false, bool useEmptyTileCollisionRule = false, bool useAutoTile= false)
    {       
        subLayers = new Dictionary<string, SubLayer>();

        gameobject = new GameObject(Name);
        gameobject.transform.parent = Parent.transform;

        if (UseCollision)
        {
            Collisions = UseCollision;
            emptyTileCollisionRule = useEmptyTileCollisionRule;

            collision = new GameObject("Collision");
            collision.transform.parent = gameobject.transform;
            collider = collision.AddComponent<PolygonCollider2D>();
            collider.pathCount = 0;
            if (Name == "Walls")
            {
                collision.layer = 10;
            }
        }
        if (useAutoTile)
            AutoTile = useAutoTile;

    }

    public SubLayer getSubLayer (string Name)
    {
        if (!subLayers.ContainsKey(Name))
            subLayers.Add(Name, new SubLayer(gameobject, Name));

        return subLayers[Name];
    }

    public Dictionary<string, SubLayer>.KeyCollection subLayerNames ()
    {
        return subLayers.Keys;
    }


}
