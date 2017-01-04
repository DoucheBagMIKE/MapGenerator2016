using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	// Use this for initialization
	void Awake () {
        ChunkManager.MapChunks = new GameObject("MapChunks");
	}
	
	// Update is called once per frame
	void Update () {
        ChunkManager.Redraw();
	}
}
