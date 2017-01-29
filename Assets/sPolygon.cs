using UnityEngine;
using System.Collections.Generic;

namespace ShadyPixel.serializable
{
    [System.Serializable]
    public class sPolygon
    {
        public List<Vector2> points;

        public sPolygon ()
        {
            points = new List<Vector2>();
        }
    }
}

