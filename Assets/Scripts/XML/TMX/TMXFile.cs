using UnityEngine;
using System.Collections.Generic;
using System.Xml.Serialization;
using ClipperLib;

[XmlRoot("map")]
public class TmxFile
{
    [XmlElement]
    public List<tileset> tileset { get; set; }
    [XmlElement]
    public properties properties { get; set;}

    public string tileIdToTilesetName(int id)
    {
        for (int i = 0; i < tileset.Count; i++)
        {
            if (id <= tileset[i].tilecount + tileset[i].firstgid - 1)
                return tileset[i].name;
        }
        return null;
    }

    public int? tileIdToTilesetId(int id)
    {
        for (int i = 0; i < tileset.Count; i++)
        {
            if (id <= tileset[i].tilecount + tileset[i].firstgid - 1)
                return i;
        }
        return null;
    }

    public int? tilesetNametoID(string name)
    {
        for(int i = 0; i < tileset.Count; i++)
        {
            if (tileset[i].name == name)
            {
                return i;
            }
        }
        return null;
    }

    public int? globalIdToLocalId (int globalID)
    {
        int? tilesetID = tileIdToTilesetId(globalID);
        if (tilesetID == null)
            return null;//global id does not belong to any tileset.

        int localID = (globalID - tileset[(int)tilesetID].firstgid);
        //Debug.Log(string.Format("GID {0} belongs to tileset {1} Tileset Tile Id is {2}", globalID, tilesetID, localID));
        return localID;
    }

    public List<IntPoint> getTileColliderInfo (int id)
    {
        List<IntPoint> ret = new List<IntPoint>();
        int? tID = tileIdToTilesetId(id);
        if(tID != null)
        {
            if (tileset[(int)tID].tile == null)// if the tileset has no collision data then we just return.
            {
                return ret;
            }
            tile tile;
            int localID = (int)globalIdToLocalId(id);
            for (int i = 0; i < tileset[(int)tID].tile.Length; i++)// theres probably a better way to get the tile than a for loop.
            {
                tile = tileset[(int)tID].tile[i];
                if (tile.id != localID)
                {
                    continue;
                }

                if (tile.objectgroup.obj == null)
                {
                    break;
                }
                //Debug.Log(string.Format("Collison data found! LID : {0}", localID));
                foreach (tmxobject obj in tile.objectgroup.obj)
                {
                    if (obj.polygon != null) // if this tile has a polygon we use that as for the tile.
                    {
                        string[] s = obj.polygon.points.Split(' ');
                        for (int index = s.Length - 1; index >= 0; index--)
                        {
                            string[] pos = s[index].Split(',');
                            //Debug.Log(string.Format("{0},{1}", pos[0], pos[1]));
                            ret.Add(new IntPoint(obj.x + Mathf.RoundToInt(float.Parse(pos[0])), Mathf.RoundToInt(obj.y + (16 - (int)float.Parse(pos[1]))))); // we round the input polygon because tiled has a shit collision editor that makes it imposable to get interger points..
                        }
                    }
                    else // if theres no polygon data then it must be a rectangle so we use the position and the width and height to get our four points.
                    {
                        //ret.Add(new IntPoint(obj.x, obj.y + (int)obj.height));
                        //ret.Add(new IntPoint(obj.x + (int)obj.width, obj.y + (int)obj.height));
                        //ret.Add(new IntPoint(obj.x + (int)obj.width, obj.y));
                        //ret.Add(new IntPoint(obj.x, obj.y));

                    }
                }
                break;
            }
        }

        return ret;
    }

}
[XmlRoot("tileset")]
public class TsxFile : tileset { }

public class tileset
{
    [XmlAttribute]
    public int firstgid { get; set; }
    [XmlAttribute]
    public string name { get; set; }
    [XmlAttribute]
    public int tilewidth { get; set; }
    [XmlAttribute]
    public int tileheight { get; set; }
    [XmlAttribute]
    public int tilecount { get; set; }
    [XmlAttribute]
    public int columns { get; set; }
    [XmlElement]
    public image image { get; set; }
    [XmlElement]
    public tile[] tile { get; set; }    
    [XmlElement]
    public properties properties { get; set; }
}

public class image
{
    [XmlAttribute]
    public string source { get; set; }
    [XmlAttribute]
    public string trans { get; set; }
    [XmlAttribute]
    public int width { get; set; }
    [XmlAttribute]
    public int height { get; set; }
}

public class tile
{
    [XmlAttribute]
    public int id { get; set; }
    [XmlElement]
    public objectgroup objectgroup { get; set; }
}

public class objectgroup
{
    [XmlElement(ElementName = "object")]
    public tmxobject[] obj { get; set;}
}

public class tmxobject
{
    [XmlAttribute]
    public int id { get; set; }
    [XmlAttribute]
    public int x { get; set; }
    [XmlAttribute]
    public int y { get; set; }
    [XmlAttribute]
    public int? width { get; set; }
    [XmlAttribute]
    public int? height { get; set; }

    [XmlElement]
    public tmxPolygon polygon { get; set; }
}

public class tmxPolygon
{
    [XmlAttribute]
    public string points { get; set; }
}

public class properties
{
    [XmlElement]
    public property[] property { get; set; }
}

public class property
{
    [XmlAttribute]
    public string name { get; set; }
    [XmlAttribute]
    public string value { get; set; }
}