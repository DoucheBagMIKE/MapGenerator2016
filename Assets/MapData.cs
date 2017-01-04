using System.Collections.Generic;

public class LayerInfo
{
    public string name;
    public bool collisions;
    public bool useEmptyTileRule;

    public LayerInfo (string Name, bool Collisions= false, bool UseEmptyTileRule= false)
    {
        name = Name;
        collisions = Collisions;
        useEmptyTileRule = UseEmptyTileRule;
    }
}

public class MapData {
    //public static string[] BaseLayers = new string[1]
    //{
    //    "Floor"
    //};
    public static LayerInfo[] BaseLayers = new LayerInfo[2]
    {
        new LayerInfo("Floor", true, true),
        new LayerInfo("Wall")
    };

    public int width;
    public int height;
    public Dictionary<string, int[,]> layer;

    public MapData (int Width, int Height)
    {
        width = Width;
        height = Height;
        layer = new Dictionary<string, int[,]>();
        foreach(LayerInfo layerInfo in BaseLayers)
        {
            layer.Add(layerInfo.name, new int[height, width]);
        }

    }
    
}
