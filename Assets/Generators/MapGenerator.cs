using UnityEngine;
using System.Collections.Generic;
using ClipperLib;

public class MapGenerator : MonoBehaviour {

    public static MapGenerator instance;

    public enum GenerateType { Maze_Structure, CA_45Rule, RandomWalk, PerlinWorm, MaskTest, MazeTest, AbstractLayoutTest, Test};

    public GenerateType GenType;
    public string seed;
    public bool randomSeed;

    public int width;
    public int height;
    public System.Random Rng;

    public bool ca_mask;
    public int[,] mask;

    [HideInInspector]
    public MazeGenerator mazeGen;
    StructureGenerator structGen;
    [HideInInspector]
    public Rule45 rule45;
    [HideInInspector]
    public Gradients gradient;
    RandomWalk randomWalk;
    PerlinWorm perlinWorm;
    PolygonGenerator polyGen;
    LayoutGenerator layoutGen;

    public int[,] Map;

    public int RemoveDeadEndsIterations;

     public TmxFile Tmx;

    void Awake()
    {
        if (instance == null)
            instance = this;

        mazeGen = gameObject.GetComponent<MazeGenerator>();
        structGen = gameObject.GetComponent<StructureGenerator>();
        rule45 = gameObject.GetComponent<Rule45>();
        gradient = gameObject.GetComponent<Gradients>();
        randomWalk = gameObject.GetComponent<RandomWalk>();
        perlinWorm = gameObject.GetComponent<PerlinWorm>();
        polyGen = gameObject.GetComponent<PolygonGenerator>();
        layoutGen = gameObject.GetComponent<LayoutGenerator>();

        if (randomSeed)
        {
            seed = System.DateTime.Now.ToString();
        }

        Rng = new System.Random(seed.GetHashCode());

        Map = new int[width, height];

    }

    void Start ()
    {
        Tmx = Serialization<TmxFile>.DeserializeFromXmlFile("Test.tmx");
        //if (Tmx != null)
        //{
        //    print(Tmx.tileset[0].tile[0].objectgroup.obj[0].polygon.points);// lol wtf tiled..
        //}
        //foreach(IntPoint p in Tmx.getTileColliderInfo(0))
        //{
        //    print(string.Format("{0},{1}", p.X, p.Y));
        //}

        Generate();
    }

    void Update()
    {
        if (Input.GetAxis("Fire1") == 1f)
        {
            Generate();
        }
    }

    void Generate ()
    {
        Map = new int[width, height];
        mazeGen.Visited = new bool[width, height];
        structGen.rooms.Clear();

        if (ca_mask)
        {
            mask = Utility.floodfill(width / 2, height / 2, rule45.Generate(width, height));
        }

        switch (GenType)
        {
            case GenerateType.Maze_Structure:
                GenMazeStructure();
                break;
            case GenerateType.CA_45Rule:
                Map = rule45.Generate(width, height);
                break;
            case GenerateType.RandomWalk:
                randomWalk.Generate();
                Map = randomWalk.Map;
                break;
            case GenerateType.PerlinWorm:
                perlinWorm.Generate();
                Map = perlinWorm.Map;
                break;
            case GenerateType.MaskTest:
                Map = Utility.floodfill(width / 2, height / 2, rule45.Generate(width, height));
                break;
            case GenerateType.MazeTest:
                int[,] test = new int[10, 10];
                mazeGen.Generate(5, 5, test);
                Map = test;
                break;
            case GenerateType.AbstractLayoutTest:
                layoutGen.Generate();
                layoutGen.TestMap();
                break;
            case GenerateType.Test:
                Map = new int[5, 5]
                {
                    {1,1,1,1,1},
                    {1,0,0,0,1},
                    {1,0,1,0,1},
                    {1,0,0,0,1},
                    {1,1,1,1,1}
                };
                width = 5;
                height = 5;
                MapChunk.chunkSize = 5;
                MapChunk testChunk = new MapChunk(0, 0);
                GameObject go = new GameObject("MapChunk");
                testChunk.gameobject = go;
                //PolyGen.Generate(testChunk);
                break;
        }
        //polyGen.Generate();
    }

    void GenMazeStructure()
    {
        structGen.Generate();
        mazeGen.Generate((width / 2) + 1, (height / 2) + 1, Map);
        for (int n = 0; n < RemoveDeadEndsIterations; n++)
        {
            mazeGen.Sparsify(Map);
        }
    }

}
