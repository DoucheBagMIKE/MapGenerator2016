using UnityEngine;
using System.Collections;

public class Rule45 : MonoBehaviour {
    public enum InitTypes {Random, Gradient};
    public InitTypes Init;
    [HideInInspector]
    public int width;
    [HideInInspector]
    public int height;
    public int[,] Map;
    [Range(0, 100)]
    public int numberOfPasses;
    public System.Random Rng;
    [Range(0,8)]
    public int DeathLimit;
    [Range(0,8)]
    public int BirthLimit;
    [Range(0,1)]
    public int outOfBoundsRule;

    [HideInInspector]
    public Gradients gradient;

    void Start ()
    {
        Map = MapGenerator.instance.Map.layer[MapData.BaseLayers[0].name];
        Rng = MapGenerator.instance.Rng;
        gradient = MapGenerator.instance.gradient;
    }
    public int[,] Generate (int w, int h, int iterations, int deathLimit, int birthLimit, int OBRule)
    {
        width = w;
        height = h;
        numberOfPasses = iterations;
        DeathLimit = deathLimit;
        BirthLimit = birthLimit;
        outOfBoundsRule = OBRule;
        return Generate();
    }
    public int[,] Generate (int w, int h)
    {
        width = w;
        height = h;
        return Generate();
    }
	
    public int[,] Generate()
    {      
        int[,] nMap = InitMap();
        for (int i = 0; i < numberOfPasses; i++)
        {

            nMap = Step(nMap);
        }
        return nMap;
    }

    int AdjWallCount(int x, int y, int[,] Map)
    {
        int nCount = 0;

        for (int xi = x - 1; xi <= x + 1; xi++)
        {
            for (int yi = y - 1; yi <= y + 1; yi++)
            {
                if (xi == x && yi == y)
                {
                    continue;
                }
                else
                {
                    bool isValid = inBounds(xi, yi);

                    if (isValid)
                    {
                        if (Map[xi, yi] == 0)
                        {
                            nCount++;
                        }
                    }
                    else
                    {
                        if (outOfBoundsRule == 1)
                        {
                            nCount++;
                        }
                    }
                }
            }
        }
        return nCount;
    }

    bool inBounds(int x, int y)
    {
        if (x < 0 || y < 0 || x > width - 1 || y > height - 1)
        {
            return false;
        }
        return true;
    }

    int[,] InitMap()
    {
        int[,] nMap = new int[width, height];
        switch (Init)
        {
            case InitTypes.Random:
                for (int x = 0; x < width - 1; x++)
                {
                    for (int y = 0; y < height - 1; y++)
                    {
                        nMap[x, y] = Rng.Next(0, 2);
                    }
                }
                break;
            case InitTypes.Gradient:
                //gradient.Generate();
                for (int x = 0; x < width - 1; x++)
                {
                    for (int y = 0; y < height - 1; y++)
                    {
                        double rD = Rng.NextDouble();
                        float gV = gradient.GradientPos(width / 2, height / 2, x, y, gradient.weightX, gradient.weightY, width, height, gradient.type);
                        if (rD <= gV)
                        {
                            nMap[x, y] = 0;
                        }
                        else
                        {
                            nMap[x, y] = 1;

                        }

                    }
                }
                break;
        }
        return nMap;

    }

    int[,] Step(int[,] Map)
    {
        int[,] nMap = new int[width, height];

        for (int x = 0; x < width - 1; x++)
        {
            for (int y = 0; y < height - 1; y++)
            {
                int adjWalls = AdjWallCount(x, y, Map);

                if (Map[x, y] == 0)
                {
                    if (adjWalls < DeathLimit)
                    {
                        nMap[x, y] = 1;
                    }
                    else
                    {
                        nMap[x, y] = 0;
                    }
                }
                else
                {
                    if (adjWalls > BirthLimit)
                    {
                        nMap[x, y] = 0;
                    }
                    else
                    {
                        nMap[x, y] = 1;
                    }
                }
            }
        }
        return nMap;
    }
}
