using UnityEngine;
using System.Collections;
using LibNoise.Generator;
using LibNoise;

public class pNoise : MonoBehaviour {

    [SerializeField]
    float _left = 2;

    [SerializeField]
    float _right = 6;

    [SerializeField]
    float _top = 1;

    [SerializeField]
    float _bottom = 5;

    public float startFreq;
    public int octaveCount;
    public float persistance;
    public int noiseWidth;
    public int noiseHeight;
    public int seed;

    Perlin perlin;
    public Noise2D heightMapBuilder;
    Renderer rend;

    public int[,] Map;

    // Use this for initialization
    void Start () {
        rend = gameObject.GetComponent<Renderer>();

        perlin = new Perlin();
        perlin.Frequency = startFreq;
        perlin.OctaveCount = octaveCount;
        perlin.Persistence = persistance;

        heightMapBuilder = new Noise2D(noiseWidth, noiseHeight, perlin);

        heightMapBuilder.GeneratePlanar(_left, _right, _top, _bottom);

        rend.material.mainTexture = heightMapBuilder.GetTexture(GradientPresets.Grayscale);
	
	}
}
