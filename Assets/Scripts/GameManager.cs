using UnityEngine;
using System.Collections;
using ClipperLib;
using UnitySteer2D.Behaviors;

public class GameManager : MonoBehaviour {

    GameObject CameraObj;
	void Awake () {
        ChunkManager.MapChunks = new GameObject("MapChunks");
	}
	
    void Start ()
    {
        CameraObj = GameObject.FindGameObjectWithTag("MainCamera");
        Vector3 P = GameObject.FindGameObjectWithTag("Player").transform.position;
        P.z = -10;
        CameraObj.transform.position = P;
    }


	void LateUpdate () {
        ChunkManager.Redraw();
        for(int i = 0; i < 2; i++)
        {
            CameraObj.transform.GetChild(i).rotation = Quaternion.identity;
        }
	}
}
