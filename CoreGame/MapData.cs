using System;
using System.Collections.Generic;
using Contracts.Interfaces;

namespace CoreGame
{
    class MapData : IMapData
    {
        Dictionary<string, int[,]> _LayerMaps;
        public int width;
        public int height;

        public MapData (int w, int h)
        {
            width = w;
            height = h;
            _LayerMaps = new Dictionary<string, int[,]>();
        }

        public int this[string LName, int x, int y]
        {
            get
            {
                return _LayerMaps[LName][x, y];
            }

            set
            {
                _LayerMaps[LName][x, y] = value;
            }
        }

        public bool OutOfBounds(int x, int y)
        {
            throw new NotImplementedException();
        }

        public void AddLayer(string LayerName)
        {
            _LayerMaps.Add(LayerName, new int[width, height]);
        }

        public void RemoveLayer(string LayerName)
        {
            _LayerMaps.Remove(LayerName);
        }

        public bool ContainsLayer(string LayerName)
        {
            return _LayerMaps.ContainsKey(LayerName);
        }
    }
}
