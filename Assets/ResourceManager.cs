using UnityEngine;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using ClipperLib;

public static class ResourceManager {
    static int nextStartID = 1;
    public static OrderedDictionary tilesetInfo = new OrderedDictionary();
    //public static ListDictionary tilesetInfo = new ListDictionary();
    //public static Dictionary<string, TilesetInfo> tilesetInfo = new Dictionary<string, TilesetInfo>();
    public static Dictionary<string, List<IntPoint>[]> colliderData = new Dictionary<string, List<IntPoint>[]>();

    public static void LoadTileset (string name)
    {
        if (tilesetInfo.Contains(name))
            return;
        //if (tilesetInfo.ContainsKey(name))
        //    return;

        string path = Application.persistentDataPath + "/Tilesets/" + name + ".tsx";
        TsxFile ts = Serialization<TsxFile>.DeserializeFromXmlFile(path);

        TilesetInfo info = new TilesetInfo(name, nextStartID, ts.tilecount, getProps(ts.properties), CreateTexture(name));

        switch(info.props["Collision"].ToLower())
        {
            case "self":
                colliderData.Add(name, CreateColliderInfo(ts));
                break;
            case "":
                break;
            default:
                if(!tilesetInfo.Contains(info.colliderData))//tilesetInfo.ContainsKey(info.ColliderData))
                {
                    // if the current tilesets colliderdata isnt in the colliderdata dictionary we need to create it... 
                    LoadTileset(info.colliderData);
                }
                break;
        }
        tilesetInfo.Add(name, info);
        //tilesetInfo.Add(name, info);
        nextStartID += ts.tilecount;
    }

    public static int? tileIdToTilesetId(int id)
    {
        for (int i = 0; i < tilesetInfo.Count; i++)
        {
            TilesetInfo info = tilesetInfo[i] as TilesetInfo;
            if (id <= info.tileCount + info.firstGid - 1)
                return i;
        }
        return null;
    }
    public static int? globalIdToLocalId(int globalID)
    {
        int? tilesetID = tileIdToTilesetId(globalID);
        if (tilesetID == null)
            return null;//global id does not belong to any tileset.
        TilesetInfo info = tilesetInfo[(int)tilesetID] as TilesetInfo;

        int localID = (globalID - info.firstGid);
        //Debug.Log(string.Format("GID {0} belongs to tileset {1} Tileset Tile Id is {2}", globalID, tilesetID, localID));
        return localID;
    }

    static Texture2D CreateTexture(string name)
    {
        string path = Application.persistentDataPath + "/Textures/" + name + ".png";
        Texture2D tex = new Texture2D(0, 0);
        tex.name = name;
        tex.filterMode = FilterMode.Point;
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.LoadImage(File.ReadAllBytes(path));
        return tex;
    }
    static Dictionary<string, string> getProps(properties properties)
    {
        Dictionary<string, string> ret = new Dictionary<string, string>();
        foreach (property prop in properties.property)
        {
            ret.Add(prop.name, prop.value);
        }
        return ret;
    }
    static List<IntPoint>[] CreateColliderInfo(tileset ts)
    {
        List<IntPoint>[] ret = new List<IntPoint>[ts.tilecount];
        
        foreach (tile Tile in ts.tile)
        {
            List<IntPoint> points = new List<IntPoint>();

            foreach (string pos in Tile.objectgroup.obj[0].polygon.points.Split(' '))
            {
                string[] vals = pos.Split(',');
                points.Add(new IntPoint(Mathf.RoundToInt(float.Parse(vals[0])), Mathf.RoundToInt((16 - (int)float.Parse(vals[1])))));

            }
            ret[Tile.id] = points;
        }
        return ret;

    }

    public static List<IntPoint> getTileColliderInfo (int index)
    {
        string target;

        TilesetInfo info = tilesetInfo[(int)tileIdToTilesetId(index)] as TilesetInfo;
        if (info.colliderData == "")
            return new List<IntPoint>();

        if (info.colliderData == "self")
        {
            target = info.name;
        }
        else
        {
            target = info.colliderData;
        }

        return colliderData[target][index - info.firstGid];
    }
}

public class TilesetInfo
{
    public string name;
    public int firstGid;
    public int tileCount;
    public string colliderData;
    public Dictionary<string, string> props;
    public Texture2D texture;
    public Material material;

    public TilesetInfo (string name, int firstGID, int tileCount, Dictionary<string, string> props, Texture2D texture)
    {
        this.name = name;
        this.firstGid = firstGID;
        this.props = props;
        this.texture = texture;
        this.tileCount = tileCount;
        this.colliderData = props["Collision"];

        this.material = new Material(Shader.Find("Unlit/Texture"));
        this.material.mainTexture = texture;
    }
}