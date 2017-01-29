using UnityEngine;
using System.Collections.Generic;

namespace ShadyPixel.serializable
{
    [System.Serializable]
    public class sProperties
    {
        public List<string> keys;
        public List<string> values;
        public sProperties()
        {
            keys = new List<string>();
            values = new List<string>();          
        }
    }
}
