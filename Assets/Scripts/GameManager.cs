using UnityEngine;
using System.Collections;
using ClipperLib;

public class GameManager : MonoBehaviour {

	// Use this for initialization
	void Awake () {
        ChunkManager.MapChunks = new GameObject("MapChunks");
	}
	
    void Start ()
    {
        TmxFile Tmx = Serialization<TmxFile>.DeserializeFromXmlFile("Test.Tmx");
        MapGenerator.instance.Tmx = Tmx;

        SubLayer s = new SubLayer(null, "Walls");

        //Debug.Log(PolyGen.TileMin(9+7, s));
        //foreach (IntPoint point in Tmx.getTileColliderInfo(9))
        //{
        //    Debug.Log(string.Format("{0},{1}", point.X, point.Y));
        //}

    }
	// Update is called once per frame
	void Update () {
        ChunkManager.Redraw();
	}
}
