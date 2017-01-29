using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using ShadyPixel.serializable;

[InitializeOnLoad]
public class AssetWatcher : EditorWindow
{
    private static TmxFile tmx;
    private static WatchedFileEvent Ev;

    private static FileSystemWatcher[] watchers;
    private static FileSystemWatcher assetFileWatcher;
    private static Queue<WatchedFileEvent> q;

    private static string[] ext = new string[2] { ".tmx", ".tsx"};

    [MenuItem("Window/Asset Watcher")]
    static void Init()
    {
        AssetWatcher window = (AssetWatcher)EditorWindow.GetWindow(typeof(AssetWatcher));
        
    }

    void OnGUI()
    {
        OnCreateWatchers();

        if (q == null)
        {
            q = new Queue<WatchedFileEvent>();
        }

        while(q.Count != 0)
        {
            Ev = q.Dequeue();

            switch (Ev.Watcher.Filter)
            {
                case "*.tmx":
                    //ProcessTmx();
                    break;
                case "*.tsx":
                    Debug.Log("Processing Tsx.");
                    ProcessTsx();
                    break;
            }
        }
    }

    static void OnCreateWatchers ()
    {
        string currentPath = Path.GetFullPath("Assets");

        if (watchers == null)
        {
            watchers = new FileSystemWatcher[ext.Length];
            for(int i =0; i < ext.Length;i++)
            {
                
                watchers[i] = new FileSystemWatcher(currentPath);

                watchers[i].Changed += OnAssetFileWatcherChanged;
                watchers[i].Created += OnAssetFileWatcherChanged;
                watchers[i].Deleted += OnAssetFileWatcherChanged;
                watchers[i].Renamed += OnAssetFileWatcherChanged;

                watchers[i].Filter = "*"+ext[i];
                watchers[i].IncludeSubdirectories = true;
                watchers[i].EnableRaisingEvents = true;
            }
        }
    }

    static void OnAssetFileWatcherChanged(object sender, FileSystemEventArgs e)
    {
        q.Enqueue(new WatchedFileEvent((FileSystemWatcher)sender, e));
    }

    public static Dictionary<string, string> getProps(properties properties, ref sTileset ts)
    {
        Dictionary<string, string> ret = new Dictionary<string, string>();
        foreach (property prop in properties.property)
        {
            ts.properties.keys.Add(prop.name);
            ts.properties.values.Add(prop.value);
            ret.Add(prop.name, prop.value);
        }
        return ret;
    }

    static void ProcessTsx()
    {
        Create_sTileset(Serialization<TsxFile>.DeserializeFromXmlFile(Ev.Data.Name));
    }

    static void ProcessTmx()
    {

    }

    static void Create_sTileset (tileset ts)
    {
        sTileset tileset = CreateInstance<sTileset>();
        tileset.fID = ts.firstgid;
        tileset.tileCount = ts.tilecount;
        tileset.tileWidth = ts.tilewidth;
        tileset.tileHeight = ts.tileheight;
        tileset.name = ts.name;

        Dictionary<string, string> props = getProps(ts.properties, ref tileset);

        //Create/Set Material.
        Material mat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/" + tileset.name);
        if (mat == null)
        {
            mat = new Material(Shader.Find("Unlit/Texture"));
            AssetDatabase.CreateAsset(mat, string.Format("Assets/Materials/{0}.mat", tileset.name));

        }
        mat.mainTexture = AssetDatabase.LoadAssetAtPath<Texture>("Assets/" + ts.image.source) as Texture;
        tileset.material = mat;

        // Create/Set ColliderData.
        switch (props["Collision"])
        {
            case "self":
                tileset.colliderInfo = CreateColliderInfo(ts);
                break;
            default:
                tileset.colliderInfo = Resources.Load<ColliderInfo>("ColliderData/" + props["Collision"]) as ColliderInfo;
                if (tileset.colliderInfo == null)
                    Debug.Log("No ColliderInfo named " + props["Collision"]);
                break;
        }
        AssetDatabase.CreateAsset(tileset, string.Format("Assets/Tilesets/{0}.asset", tileset.name));
        AssetDatabase.SaveAssets();
    }

    static ColliderInfo CreateColliderInfo(tileset ts)
    {
        ColliderInfo info = CreateInstance<ColliderInfo>();

        info.name = Ev.Data.Name;

        foreach (tile Tile in ts.tile)
        {
            sPolygon poly = new sPolygon();
            foreach (string pos in Tile.objectgroup.obj[0].polygon.points.Split(' '))
            {
                string[] vals = pos.Split(',');
                poly.points.Add(new Vector2(float.Parse(vals[0]), float.Parse(vals[1])));

            }
            info.Polygons.Add(poly);
            info.keys.Add(Tile.id);
        }
        string path = "Assets/Resources/ColliderData/";
        string[] parts = Ev.Data.Name.Split('\\');
        string name = parts[parts.Length - 1].Split('.')[0];
        AssetDatabase.CreateAsset(info, string.Format("{0}{1}{2}", path, name, ".asset"));
        AssetDatabase.SaveAssets();
        return info;

    }
}

public class WatchedFileEvent
{
    public FileSystemWatcher Watcher;
    public FileSystemEventArgs Data;

    public WatchedFileEvent (FileSystemWatcher fsw, FileSystemEventArgs e)
    {
        Watcher = fsw;
        Data = e;
    }
}