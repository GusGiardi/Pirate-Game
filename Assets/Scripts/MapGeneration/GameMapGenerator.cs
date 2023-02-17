using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMapGenerator : MonoBehaviour
{
    [SerializeField] RawImage _debugImage;
    private Texture2D _noiseTexture;
    [SerializeField] int _noiseTexResolution = 128;
    [SerializeField] float _noiseSize = 0.01f;
    [SerializeField] float _minValueToPlaceIsland = 0.1f;

    private void Awake()
    {
        _noiseTexture = CreateNoiseTexture(_noiseTexResolution, _noiseSize);
        _debugImage.texture = _noiseTexture;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            _noiseTexture = CreateNoiseTexture(_noiseTexResolution, _noiseSize);
            _debugImage.texture = _noiseTexture;
        }
    }

    private Texture2D CreateNoiseTexture(int rez, float size)
    {
        float seedX = Random.value * size * 100;
        float seedY = Random.value * size * 100;

        Color[] pixels = new Color[rez * rez];
        for (int x = 0; x < rez; x++)
        {
            for (int y = 0; y < rez; y++)
            {
                float value = Mathf.PerlinNoise(seedX + (x * 1f / rez) * size, seedY + (y * 1f / rez) * size);
                value = value > _minValueToPlaceIsland ? 1 : 0;
                pixels[y * rez + x] = new Color(value, value, value, 1);
            }
        }

        Texture2D noise = new Texture2D(rez, rez);
        noise.filterMode = FilterMode.Point;
        noise.SetPixels(pixels);
        noise.Apply();
        return noise;
    }
}
