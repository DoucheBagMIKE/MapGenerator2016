using UnityEngine;
using Contracts.Abstract;
using Contracts.Interfaces;
using System;

namespace CoreGame
{
    class MapGenerator : MapGenBase
    {
        public MapGenerator() :base (null) { }
        public MapGenerator(MapData Map) : base(Map) { }

        public string Name
        {
            get
            {
                return "MapGenerator";
            }
        }

        public override void Generate()
        {
            Debug.Log ("Generator Called.");
        }
    }
}