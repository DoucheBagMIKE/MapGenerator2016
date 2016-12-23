using UnityEngine;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour {

    public static MapGenerator instance;

    public enum GenerateType { Maze_Structure, CA_45Rule, Gradient, RandomWalk, PerlinWorm, MaskTest, MazeTest, AbstractLayoutTest};

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

    // Use this for initialization
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
        map tmxMap = Serialization<map>.DeserializeFromXmlFile("test.tmx");
        //print(tmxMap.tilesets[0].name);
        Generate();
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
            case GenerateType.Gradient:
                gradient.Generate();
                //Map = gradient.Value;
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
                //layoutGen.Test();
                break;
        }
        polyGen.Generate();
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

    void Update()
    {
        if (Input.GetAxis("Fire1") == 1f)
        {
            Generate();
        }
    }
    //void OnDrawGizmos()
    //{
    //    if (Map != null)
    //    {
    //        switch(GenType)
    //        {
    //            case GenerateType.Gradient:
    //                DrawGradient();
    //                break;
    //            default:
    //                DrawMap();
    //                break;
    //        }
    //    }
    //}

    //void DrawMap ()
    //{
    //    for (int x = 0; x < width; x++)
    //    {
    //        for (int y = 0; y < height; y++)
    //        {
    //            switch (Map[x, y])
    //            {
    //                case 0:
    //                    Gizmos.color = Color.black;
    //                    break;
    //                case 1:
    //                    Gizmos.color = Color.white;
    //                    break;
    //                case 2:
    //                    Gizmos.color = Color.grey;
    //                    break;
    //            }
    //            Vector3 pos = new Vector3(-width / 2 + x + .5f, 0, -height / 2 + y + .5f);
    //            Gizmos.DrawCube(pos, Vector3.one);
    //        }
    //    }
    //}
    //void DrawGradient()
    //{
    //    for (int x = 0; x < width; x++)
    //    {
    //        for (int y = 0; y < height; y++)
    //        {
    //            float cv = gradient.Value[x,y];

    //            Gizmos.color = new Color(cv, cv, cv, 1f);
    //            Vector3 pos = new Vector3(-width / 2 + x + .5f, 0, -height / 2 + y + .5f);
    //            Gizmos.DrawCube(pos, Vector3.one);
    //        }
    //    }
    //}

}
