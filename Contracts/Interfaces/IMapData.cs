namespace Contracts.Interfaces
{
    public interface IMapData
    {
        int this[string LName,int x, int y] { get; set; }
        bool OutOfBounds(int x, int y);
        bool ContainsLayer(string LayerName);
    }
}
