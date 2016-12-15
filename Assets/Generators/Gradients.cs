using UnityEngine;
using System.Collections;

public class Gradients : MonoBehaviour {
    public enum types {X,Y,Radial};
    public types type;
    types lastType;
    public int width;
    public int height;
    public float cx;
    public float cy;
    public float[,] Value;
    [Range(0,5)]
    public float weightX;
    public float weightY;
    public System.Random Rng;

    void Start ()
    {
        width = MapGenerator.instance.width;
        height = MapGenerator.instance.height;
        Rng = MapGenerator.instance.Rng;

    }

    void Update()
    {
        if (type != lastType)
        {
            lastType = type;
            Generate();
        }
    }

    public void Generate()
    {
        Value = new float[width, height];
        float dist;
        
        //cx = width / 2f;
        //cy = height / 2f;

        for (int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++) { 
                switch (type)
                {
                    case Gradients.types.Radial:
                        float r = Mathf.Min(width, height);
                        dist = Mathf.Sqrt(Mathf.Pow((x + .5f) - cx, 2) + Mathf.Pow((y + .5f) - cy, 2));
                        float cv = dist * weightX;
                        Value[x,y] = cv / r;
                        break;
                    case Gradients.types.X:
                        dist = Mathf.Abs((x + .5f) - cx);
                        Value[x,y] = (dist * weightX) / width;
                        break;
                    case Gradients.types.Y:
                        dist = Mathf.Abs((y + .5f) - cy);
                        Value[x, y] = (dist * weightY) / height;
                        break;
                }
            }
        }
    }

    public float GradientPos(int cx, int cy, int x, int y, float weightX, float weightY, int width, int height, types genType)
    {
        float dist;
        switch (genType)
        {
            case Gradients.types.Radial:
                float r = Mathf.Min(width, height);
                dist = Mathf.Sqrt(Mathf.Pow((x + .5f) - cx, 2) + Mathf.Pow((y + .5f) - cy, 2));
                float cv = dist * weightX;
                return cv / r;
            case Gradients.types.X:
                dist = Mathf.Abs((x + .5f) - cx);
                return (dist * weightX) / width;
            case Gradients.types.Y:
                dist = Mathf.Abs((y + .5f) - cy);
                return (dist * weightY) / height;
        }
        return 0f;
    }
}
