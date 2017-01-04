using System.Collections.Generic;

public class MapData {
    public static string[] BaseLayers = new string[2]
    {
        "Floor",
        "Wall"
    };

    public int width;
    public int height;
    public Dictionary<string, int[,]> layer;

    public MapData (int Width, int Height)
    {
        width = Width;
        height = Height;
        layer = new Dictionary<string, int[,]>();
        foreach(string layerName in BaseLayers)
        {
            layer.Add(layerName, new int[height, width]);
        }

    }
    
}
