using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMapGenerator : MonoBehaviour
{
    private struct MapElement
    {
        private Vector2 _position;
        private float _radius;

        public Vector2 position => _position;
        public float radius => _radius;

        public MapElement(Vector2 position, float radius)
        {
            _position = position;
            _radius = radius;
        }
    }

    [SerializeField] float _mapSize;

    [Header("Islands")]
    [SerializeField] int _islandNoiseTexResolution = 32;
    private Texture2D _islandNoiseTexture;
    [SerializeField] float _islandNoiseSize = 5;
    [SerializeField] float _maxValueToPlaceIsland = 0.35f;
    [SerializeField] float _minIslandRadius = 2;
    [SerializeField] float _maxIslandRadius = 8;
    private List<MapElement> _islands = new List<MapElement>();
    [SerializeField] GameObject _islandPrefab;
    private List<GameObject> _instantiatedIslands = new List<GameObject>();
    [SerializeField] RawImage _debugIslands;

    [Header("Rocks")]
    [SerializeField] int _rocksNoiseTexResolution = 32;
    private Texture2D _rocksNoiseTexture;
    [SerializeField] float _rocksNoiseSize = 5;
    [SerializeField] float _maxValueToPlaceRocks = 0.35f;
    [SerializeField] float _minRockRadius = 0.5f;
    [SerializeField] float _maxRockRadius = 4;
    private List<MapElement> _rocks = new List<MapElement>();
    [SerializeField] GameObject _rocksPrefab;
    private List<GameObject> _instantiatedRocks = new List<GameObject>();
    [SerializeField] RawImage _debugRocks;

    [Header("Render")]
    //[SerializeField] int _renderTexResolution = 4096;
    //private Texture2D _renderTex;
    [SerializeField] SpriteRenderer _scenarioSpriteRenderer;
    [SerializeField] Material _mapMaterial;
    private ComputeBuffer _islandsBuffer;
    private ComputeBuffer _rocksBuffer;
    private Texture2D _islandDisplacementTex;
    [SerializeField] int _islandDisplacementTexResolution = 256;
    [SerializeField] float _islandDisplacementNoiseSize = 30;
    private Texture2D _islandGrassDetailTex;
    [SerializeField] int _islandGrassDetailTexResolution = 256;
    [SerializeField] float _islandGrassDetailNoiseSize = 30;
    private Texture2D _rockDisplacementTex;
    [SerializeField] int _rockDisplacementTexResolution = 128;
    [SerializeField] float _rockDisplacementNoiseSize = 100;

    [SerializeField] RawImage _debugRender;

    private void Start()
    {
        CreateNewMap();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            ClearMap();
            CreateNewMap();
        }
    }

    private void OnDestroy()
    {
        _islandsBuffer.Dispose();
        _rocksBuffer.Dispose();
    }

    private void CreateNewMap()
    {
        _islandNoiseTexture = CreateNoiseTexture(_islandNoiseTexResolution, _islandNoiseSize);
        _debugIslands.texture = _islandNoiseTexture;

        _rocksNoiseTexture = CreateNoiseTexture(_rocksNoiseTexResolution, _rocksNoiseSize);
        _debugRocks.texture = _rocksNoiseTexture;

        PopulateMapElementsList(ref _islands, _islandNoiseTexture, _maxValueToPlaceIsland, _minIslandRadius, _maxIslandRadius);

        PopulateMapElementsList(ref _rocks, _rocksNoiseTexture, _maxValueToPlaceRocks, _minRockRadius, _maxRockRadius);

        InstantiateMapElements(ref _islands, _islandPrefab, ref _instantiatedIslands);
        InstantiateMapElements(ref _rocks, _rocksPrefab, ref _instantiatedRocks);

        UpdateMapMaterial();
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
                pixels[y * rez + x] = new Color(value, value, value, 1);
            }
        }

        Texture2D noise = new Texture2D(rez, rez);
        noise.filterMode = FilterMode.Point;
        noise.SetPixels(pixels);
        noise.Apply();
        return noise;
    }

    private void PopulateMapElementsList(ref List<MapElement> mapElementList, Texture2D noiseTex, float maxValueToCreateElement, float minRadius, float maxRadius)
    {
        for (int x = 0; x < noiseTex.width; x++)
        {
            for (int y = 0; y < noiseTex.height; y++)
            {
                float pixelValue = noiseTex.GetPixel(x, y).r;
                if (pixelValue > maxValueToCreateElement)
                {
                    continue;
                }

                Vector2 position = new Vector2((float)x / noiseTex.width, (float)y / noiseTex.height);
                float radius = Mathf.Lerp(maxRadius, minRadius, pixelValue / maxValueToCreateElement);

                mapElementList.Add(new MapElement(position, radius));
            }
        }
    }

    private void InstantiateMapElements(ref List<MapElement> mapElementList, GameObject prefab, ref List<GameObject> instantiatedElementsList)
    {
        foreach (MapElement mapElement in mapElementList)
        {
            GameObject instantiatedElement = ObjectPoolManager.instance.InstantiateInPool(prefab, mapElement.position * _mapSize, Quaternion.identity);
            CircleCollider2D collider = instantiatedElement.GetComponent<CircleCollider2D>();
            collider.radius = mapElement.radius;
            instantiatedElementsList.Add(instantiatedElement);
        }
    }

    private void UpdateMapMaterial()
    {
        _islandsBuffer = new ComputeBuffer(_islands.Count, sizeof(float) * 3, ComputeBufferType.Default);
        _islandsBuffer.SetData(_islands.ToArray());
        _mapMaterial.SetBuffer("_islands", _islandsBuffer);
        _mapMaterial.SetInt("_islandCount", _islands.Count);

        _rocksBuffer = new ComputeBuffer(_rocks.Count, sizeof(float) * 3, ComputeBufferType.Default);
        _rocksBuffer.SetData(_rocks.ToArray());
        _mapMaterial.SetBuffer("_rocks", _rocksBuffer);
        _mapMaterial.SetInt("_rockCount", _rocks.Count);

        _islandDisplacementTex = CreateNoiseTexture(_islandDisplacementTexResolution, _islandDisplacementNoiseSize);
        _islandDisplacementTex.filterMode = FilterMode.Bilinear;
        _mapMaterial.SetTexture("_IslandDisplacementTex", _islandDisplacementTex);

        _islandGrassDetailTex = CreateNoiseTexture(_islandGrassDetailTexResolution, _islandGrassDetailNoiseSize);
        _islandGrassDetailTex.filterMode = FilterMode.Bilinear;
        _mapMaterial.SetTexture("_IslandGrassDetailTex", _islandGrassDetailTex);
        _debugRocks.texture = _islandGrassDetailTex;

        _rockDisplacementTex = CreateNoiseTexture(_rockDisplacementTexResolution, _rockDisplacementNoiseSize);
        _rockDisplacementTex.filterMode = FilterMode.Bilinear;
        _mapMaterial.SetTexture("_RockDisplacementTex", _rockDisplacementTex);

        _mapMaterial.SetFloat("_mapSize", _mapSize);
        _debugRender.material = _mapMaterial;
        _scenarioSpriteRenderer.material = _mapMaterial;
        _scenarioSpriteRenderer.transform.localScale = Vector3.one * _mapSize;
        _scenarioSpriteRenderer.transform.position = Vector2.one * _mapSize / 2;
    }

    //private void CreateRenderTex()
    //{
    //    Color[] pixels = new Color[_renderTexResolution * _renderTexResolution];
    //    for (int x = 0; x < _renderTexResolution; x++)
    //    {
    //        for (int y = 0; y < _renderTexResolution; y++)
    //        {
    //            float islandValue = 0;
    //            float rockValue = 0;
    //            foreach(MapElement island in _islands)
    //            {
    //                float distanceToIslandCenter = Vector2.Distance(
    //                    new Vector2((float)x / _renderTexResolution, (float)y / _renderTexResolution),
    //                    island.position);
    //                float currentValue = Mathf.InverseLerp(island.radius, 0, distanceToIslandCenter * _mapSize);
    //                if (currentValue > islandValue)
    //                {
    //                    islandValue = currentValue;
    //                }
    //            }
    //            foreach (MapElement rock in _rocks)
    //            {
    //                float distanceToRockCenter = Vector2.Distance(
    //                    new Vector2((float)x / _renderTexResolution, (float)y / _renderTexResolution),
    //                    rock.position);
    //                float currentValue = Mathf.InverseLerp(rock.radius, 0, distanceToRockCenter * _mapSize);
    //                if (currentValue > rockValue)
    //                {
    //                    rockValue = currentValue;
    //                }
    //            }

    //            pixels[y * _renderTexResolution + x] = new Color(islandValue, rockValue, 0, 1);
    //        }
    //    }

    //    _renderTex = new Texture2D(_renderTexResolution, _renderTexResolution);
    //    _renderTex.SetPixels(pixels);
    //    _renderTex.filterMode = FilterMode.Point;
    //    _renderTex.Apply();

    //    _debugRender.texture = _renderTex;
    //}

    private void ClearMap()
    {
        _islandsBuffer.Dispose();
        _rocksBuffer.Dispose();

        foreach (GameObject instantiatedElement in _instantiatedIslands)
        {
            instantiatedElement.SetActive(false);
        }
        _instantiatedIslands.Clear();

        foreach (GameObject instantiatedElement in _instantiatedRocks)
        {
            instantiatedElement.SetActive(false);
        }
        _instantiatedRocks.Clear();

        _islands.Clear();
        _rocks.Clear();
    }
}
