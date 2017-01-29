using UnityEngine;
using System.Collections;

namespace ShadyPixel.serializable
{
    [System.Serializable]
    public class Tileset : ScriptableObject
    {
        int fID;
        Material material;
        int tileCount;
        int tileWidth;
        int tileHeight;
        ColliderInfo colliderInfo;

    }
}
