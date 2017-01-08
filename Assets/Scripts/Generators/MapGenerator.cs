using UnityEngine;
using System.Collections.Generic;
using ClipperLib;

public class MapGenerator : MonoBehaviour {

    public static MapGenerator instance;

    public enum GenerateType { Maze_Structure, CA_45Rule, RandomWalk, PerlinWorm, MaskTest, AbstractLayoutTest};

    public GenerateType GenType;
    public string seed;
    public bool randomSeed;

    public int width;
    public int height;
    public System.Random Rng;

    public MapData Map;

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
    LayoutGenerator layoutGen;

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
        layoutGen = gameObject.GetComponent<LayoutGenerator>();

        if (randomSeed)
        {
            seed = System.DateTime.Now.ToString();
        }

        Rng = new System.Random(seed.GetHashCode());

        Map = new MapData(width, height);

    }

    void Start ()
    {
        Tmx = Serialization<TmxFile>.DeserializeFromXmlFile("Test.tmx");
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
        Map = new MapData(width, height);
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
                Map.layer["Floor"] = rule45.Generate(width, height);
                break;
            case GenerateType.RandomWalk:
                randomWalk.Generate();
                Map.layer["Floor"] = randomWalk.Map;
                break;
            case GenerateType.PerlinWorm:
                perlinWorm.Generate();
                Map.layer["Floor"] = perlinWorm.Map;
                break;
            case GenerateType.MaskTest:
                Map.layer["Floor"] = Utility.floodfill(width / 2, height / 2, rule45.Generate(width, height));
                break;
            case GenerateType.AbstractLayoutTest:
                layoutGen.Generate();
                layoutGen.TestMap();
                break;
        }
        ChunkManager.SpawnChunks();
    }

    void GenMazeStructure()
    {
        structGen.Generate();
        mazeGen.Generate((width / 2) + 1, (height / 2) + 1, Map.layer[MapData.BaseLayers[0].name]);
        for (int n = 0; n < RemoveDeadEndsIterations; n++)
        {
            mazeGen.Sparsify(Map.layer[MapData.BaseLayers[0].name]);
        }
    }

}
