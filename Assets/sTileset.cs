using UnityEngine;
using System.Collections;

namespace ShadyPixel.serializable
{
    [System.Serializable]
    public class sTileset : ScriptableObject
    {
        public int fID;
        public Material material;
        public int tileCount;
        public int tileWidth;
        public int tileHeight;
        public ColliderInfo colliderInfo;
        public sProperties properties;

        void OnEnable ()
        {
            if(properties == null)
            {
                properties = new sProperties();
            }
        }
    }
}

