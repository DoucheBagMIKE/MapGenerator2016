using UnityEngine;
using System.Collections.Generic;
using ShadyPixel.serializable;

namespace ShadyPixel.serializable
{
    [System.Serializable]
    public class ColliderInfo : ScriptableObject
    {
        public List<sPolygon> Polygons;
        public List<int> keys;

        void OnEnable()
        {
            if (Polygons == null)
            {
                Polygons = new List<sPolygon>();
            }
            if (keys == null)
                keys = new List<int>();

        }


    }
}
