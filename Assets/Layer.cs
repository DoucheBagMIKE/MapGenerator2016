using UnityEngine;
using System.Collections.Generic;

public class Layer {

    Dictionary<string, SubLayer> subLayers;

    public GameObject gameobject;

    public Layer (GameObject Parent, string Name)
    {
        gameobject = new GameObject(Name);
        gameobject.transform.parent = Parent.transform;
        subLayers = new Dictionary<string, SubLayer>();
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
