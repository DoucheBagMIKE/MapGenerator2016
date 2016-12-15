using UnityEngine;
using LibNoise;
using LibNoise.Generator;
using System.Collections;

class Worm
{
    public float seed;
    public Vector2 pos;
    public Vector2 dir;

    public Worm (float seed)
    {
        this.seed = seed;
    }

}

public class PerlinWorm : MonoBehaviour
{
    public int[,] Map;
    public System.Random Rng;
    public Vector2 noiseStart;
    public int noiseLength;
    public int stepSize;
    public int MaxSegments;

    void Start ()
    {
        Map = MapGenerator.instance.Map;
        Rng = MapGenerator.instance.Rng;
    }

    [SerializeField]
    float _left = 2;

    [SerializeField]
    float _right = 6;

    [SerializeField]
    float _top = 1;

    [SerializeField]
    float _bottom = 5;

    bool inBounds(float x, float y)
    {
        if (x < 0 || y < 0 || x > Map.GetLength(0) - 1 || y > Map.GetLength(1) - 1)
        {
            return false;
        }
        return true;
    }

    public void Brush(Vector2 pos, float radius)
    {
        Vector2 cPos = new Vector2();

        for (float x = pos.x - radius; x < pos.x + radius; x++)
        {
            for (float y = pos.y - radius; y < pos.y + radius; y++)
            {
                cPos.Set(x + 0.5f, y + 0.5f);

                //float dist = Mathf.Sqrt(Mathf.Pow((x + .5f) - pos.x, 2) + Mathf.Pow((y + .5f) - pos.y, 2));

                float dist = Vector2.Distance(cPos, pos);

                if (dist < radius && inBounds(x, y))
                {
                    Map[Mathf.FloorToInt(x), Mathf.FloorToInt(y)] = 1;
                }
            }
        }
    }

    public void Generate()
    {
        Perlin perlin = new Perlin();
        perlin.OctaveCount = 3;
        perlin.Quality = QualityMode.High;
        perlin.Frequency = .375F;
        perlin.Persistence = .5;
        perlin.Seed = Rng.Next();

        Noise2D heightMapBuilder = new Noise2D(256, 256, perlin);
        heightMapBuilder.GeneratePlanar(_left, _right, _top, _bottom);
        float[,] noise = heightMapBuilder.GetData();

        float _StepSize = noiseLength / MaxSegments;

        Worm pWorm = new Worm(0f);
        pWorm.pos.Set(51, 51);

        for (float i = 0f; i < MaxSegments; i += _StepSize)
        {
            float radians = Mathf.Lerp(-360f, 360f, noise[Mathf.FloorToInt(noiseStart.x + i), Mathf.FloorToInt(noiseStart.y)]) * Mathf.Deg2Rad;

            pWorm.dir.Set((float)Mathf.Cos(radians), (float)Mathf.Sin(radians));


            pWorm.pos += pWorm.dir * stepSize;
            Brush(pWorm.pos, Mathf.Lerp(3f, 0, i/noiseLength));
        }

    }

}

//public class PerlinWorm : MonoBehaviour
//{
//    public Vector2 pos;
//    public Vector2 dir;
//    public Vector2 targetPos;
//    public int stepSize;

//    public int[,] Map;
//    public int WormSteps;

//    [SerializeField]
//    Gradient _gradient = GradientPresets.Grayscale;

//    [SerializeField]
//    float _left = 2;

//    [SerializeField]
//    float _right = 6;

//    [SerializeField]
//    float _top = 1;

//    [SerializeField]
//    float _bottom = 5;

//    bool inBounds(float x, float y)
//    {
//        if (x < 0 || y < 0 || x > Map.GetLength(0) - 1 || y > Map.GetLength(1) - 1)
//        {
//            return false;
//        }
//        return true;
//    }

//    public void Brush(Vector2 pos, float radius)
//    {
//        Vector2 cPos = new Vector2();

//        for (float x = pos.x - radius; x < pos.x + radius; x++)
//        {
//            for (float y = pos.y - radius; y < pos.y + radius; y++)
//            {
//                cPos.Set(x + 0.5f, y + 0.5f);

//                //float dist = Mathf.Sqrt(Mathf.Pow((x + .5f) - pos.x, 2) + Mathf.Pow((y + .5f) - pos.y, 2));

//                float dist = Vector2.Distance(cPos, pos);

//                if (dist < radius && inBounds(x, y))
//                {
//                    Map[Mathf.FloorToInt(x), Mathf.FloorToInt(y)] = 1;
//                }
//            }
//        }
//    }

//    public void Generate()
//    {
//        Perlin perlin = new Perlin();
//        perlin.OctaveCount = 3;

//        Noise2D heightMapBuilder = new Noise2D(256, 256, perlin);
//        heightMapBuilder.GeneratePlanar(_left, _right, _top, _bottom);

//        float TotalDistance = Vector2.Distance(pos, targetPos);
//        float _StepSize = TotalDistance / WormSteps;

//        float[,] noise = heightMapBuilder.GetData();

//        for (int i = 0; i < WormSteps; i++)
//        {
//            float radians = Mathf.Lerp(-360f, 360f, noise[Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y)]) * Mathf.Deg2Rad;
//            dir.Set((float)Mathf.Cos(radians), (float)Mathf.Sin(radians));

//            pos = pos + (dir * stepSize);
//            Brush(pos, Mathf.Lerp(3f, 0f, 0));
//        }

//    }

//}
