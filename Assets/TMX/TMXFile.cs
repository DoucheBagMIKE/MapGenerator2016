using UnityEngine;
using System.Collections.Generic;
using System.Xml.Serialization;
using ClipperLib;

//    <? xml version="1.0" encoding="UTF-8"?>
//<map version = "1.0" orientation="orthogonal" renderorder="right-down" width="16" height="16" tilewidth="16" tileheight="16" nextobjectid="1">
// <tileset firstgid = "1" name="Floor" tilewidth="16" tileheight="16" tilecount="819" columns="21">
//  <image source = "Assets/DawnLike/Objects/Floor.png" trans="ffffff" width="336" height="624"/>
// </tileset>
// <tileset firstgid = "820" name="Wall" tilewidth="16" tileheight="16" tilecount="1020" columns="20">
//  <image source = "Assets/DawnLike/Objects/Wall.png" trans="ffffff" width="320" height="816"/>
// </tileset>
//</map>

[XmlRoot("map")]
public class TmxFile
{
    [XmlElement]
    public List<tileset> tileset { get; set; }

    public string tileIdToTilesetName(int id)
    {
        for (int i = 0; i < tileset.Count; i++)
        {
            if (id < tileset[i].tilecount + tileset[i].firstgid)
                return tileset[i].name;
        }
        return null;
    }

    public int? tileIdToTilesetId(int id)
    {
        for (int i = 0; i < tileset.Count; i++)
        {
            if (id < tileset[i].tilecount + tileset[i].firstgid)
                return i;
        }
        return null;
    }

    public List<IntPoint> getTileColliderInfo (int id)
    {
        List<IntPoint> ret = new List<IntPoint>();
        int? tID = tileIdToTilesetId(id);
        if(tID != null)
        {
            for (int i = 0; i < tileset[(int)tID].tile.Length; i++)
            {
                tile tile = tileset[(int)tID].tile[i];
                if (tile.id != id)
                {
                    continue;
                }
                else
                {
                    foreach (tmxobject obj in tile.objectgroup.obj)
                    {
                        if (obj.polygon != null) // if this tile has a polygon we use that as for the tile.
                        {
                            string[] s;
                            foreach(string pos in obj.polygon.points.Split(' '))
                            {
                                s = pos.Split(',');
                                ret.Add(new IntPoint(obj.x + int.Parse(s[0]), obj.y + int.Parse(s[1])));
                                
                            }
                            break;// olny looks at the first obj in the collinders.
                        }
                        else // if theres no polygon data then it must be a rectangle so we use the position and the width and height to get our four points.
                        {
                            ret.Add(new IntPoint(obj.x, obj.y + (int)obj.height));
                            ret.Add(new IntPoint(obj.x + (int)obj.width, obj.y + (int)obj.height));
                            ret.Add(new IntPoint(obj.x + (int)obj.width, obj.y));
                            ret.Add(new IntPoint(obj.x, obj.y));
                            break;
                        }
                    }
                }
            }
        }

        return ret;
    }

}

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