using UnityEngine;
using System.Collections;

public enum direction { Up, Down, left, Right };

class Walker
{
    public int x;
    public int y;
    public int stepX;
    public int stepY;
    public direction dir;


    public Walker (int startX, int startY)
    {
        x = startX;
        y = startY;
        stepX = 0;
        stepY = 0;
    }
}

public class RandomWalk : MonoBehaviour {
    

    public System.Random Rng;
    public int[,] Map;

    public int startX;
    public int startY;
    Walker walker;
    public int width;
    public int height;
    public int numberOfSteps;

    void Start ()
    {
        Map = MapGenerator.instance.Map;
        width = MapGenerator.instance.width;
        height = MapGenerator.instance.height;
        Rng = MapGenerator.instance.Rng;

    }

    void Walk(Walker walker, int numberOfSteps)
    {
        int i = 0;
        while(i < numberOfSteps)
        {
            i++;
            float rFloat = (float)Rng.NextDouble();

            if(rFloat <= .5f)
            {
                Step(walker);
            }
            else
            {
                Turn(walker);
                Step(walker);
            }
            Map[walker.x, walker.y] = 1;
        }
    }

    void Step (Walker walker)
    {
        int nx = walker.x + walker.stepX;
        int ny = walker.y + walker.stepY;

        if (nx < 0 || ny < 0 || nx > width - 1 || ny > height - 1)
        {
            Turn(walker);
            Step(walker);
        }
        else
        {
            walker.x = nx;
            walker.y = ny;
        }
    }
    void Turn (Walker walker)
    {
        float rF = (float)Rng.NextDouble();
        if (walker.stepX != 0)
        {
            if (rF <= .5f)
            {
                walker.stepY = 1;
            }
            else
            {
                walker.stepY = -1;
            }
            walker.stepX = 0;
        }
        else
        {
            if (rF <= .5f)
            {
                walker.stepX = 1;
            }
            else
            {
                walker.stepX = -1;
            }
            walker.stepY = 0;
        }

    }

    public void Generate()
    {
        walker = new Walker(startX, startY);

        if (Rng.NextDouble() <= .5f)
        {
            if (Rng.NextDouble() <= 0.5f)
            {
                walker.stepX = 1;
            }
            else
            {
                walker.stepX = -1;
            }
        }
        else
        {
            if (Rng.NextDouble() <= 0.5f)
            {
                walker.stepY = 1;
            }
            else
            {
                walker.stepY = -1;
            }
        }

        Walk(walker, numberOfSteps);
    }
}
