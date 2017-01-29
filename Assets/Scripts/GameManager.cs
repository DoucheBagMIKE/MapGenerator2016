using UnityEngine;
using System.IO;

using System.Collections.Generic;
using SimplePlugin;
using Contracts.Abstract;
using Contracts.Interfaces;
public class GameManager : MonoBehaviour {

    GameObject CameraObj;
	void Awake () {
        ChunkManager.MapChunks = new GameObject("MapChunks");
        InitFileStructure();
        ResourceManager.LoadTileset("Floor");
        ResourceManager.LoadTileset("Walls");
        ResourceManager.LoadTileset("SideWalls");
        ResourceManager.LoadTileset("WoodWalls");

        ICollection<IMapGenerator> plugins = GenericPluginLoader<IMapGenerator>.LoadPlugins(Application.dataPath + "/Plugins");
        foreach (IPlugin item in plugins)
        {
            Debug.Log(item.Name);
        }
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
        for(int i = 0; i < CameraObj.transform.childCount; i++)
        {
            CameraObj.transform.GetChild(i).rotation = Quaternion.identity;
        }
	}

    void InitFileStructure ()
    {
        string exPath = Application.persistentDataPath;
        string inPath = Application.dataPath;
        string[] folderNames = new string[2] { "Tilesets", "Textures" };

        foreach (string name in folderNames)
        {
            string exDir = string.Format("{0}/{1}", exPath, name);
            string inDir = string.Format("{0}/{1}", inPath, name);

            if (!Directory.Exists(exDir))
            {
                Directory.CreateDirectory(exDir);
            }

            foreach (string filePath in Directory.GetFiles(inDir))
            {
                string fName = Path.GetFileName(filePath);

                if (fName.Contains(".meta"))
                    continue;

                string ex = string.Format("{0}/{1}", exDir, fName);

                if (!File.Exists(ex))
                {
                    File.Copy(filePath, ex);
                }
            }
        }
    }
}
