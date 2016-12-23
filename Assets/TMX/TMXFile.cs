using UnityEngine;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization;

//    <? xml version="1.0" encoding="UTF-8"?>
//<map version = "1.0" orientation="orthogonal" renderorder="right-down" width="16" height="16" tilewidth="16" tileheight="16" nextobjectid="1">
// <tileset firstgid = "1" name="Floor" tilewidth="16" tileheight="16" tilecount="819" columns="21">
//  <image source = "Assets/DawnLike/Objects/Floor.png" trans="ffffff" width="336" height="624"/>
// </tileset>
// <tileset firstgid = "820" name="Wall" tilewidth="16" tileheight="16" tilecount="1020" columns="20">
//  <image source = "Assets/DawnLike/Objects/Wall.png" trans="ffffff" width="320" height="816"/>
// </tileset>
//</map>

[Serializable]
class map
{
    [DataMember]
    public List<tileset> tilesets{ get; set; }
}

[Serializable]
class tileset
{
    [DataMember]
    public int firstgid { get; set; }
    [DataMember]
    public string name { get; set; }
    [DataMember]
    public int tilewidth { get; set; }
    [DataMember]
    public int tileheight { get; set; }
    [DataMember]
    public int tilecount { get; set; }
    [DataMember]
    public int columns { get; set; }
    [DataMember]
    public image image { get; set; }
    
}
[Serializable]
class image
{
    [DataMember]
    public string source { get; set; }
    [DataMember]
    public string trans { get; set; }
    [DataMember]
    public int width { get; set; }
    [DataMember]
    public int height { get; set; }
}

