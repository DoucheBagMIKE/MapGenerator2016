using Contracts.Interfaces;

namespace Contracts.Abstract
{
    abstract public class MapGenBase : IMapGenerator
    {
        IMapData Map { get; set; }

        public MapGenBase (IMapData mapData)
        {
            Map = mapData;
        }

        abstract public void Generate();
    }
}
