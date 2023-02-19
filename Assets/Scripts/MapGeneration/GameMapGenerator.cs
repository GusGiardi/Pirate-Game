using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
    public float mapSize => _mapSize;
    [SerializeField] float _elementsRadiusMultiplier = 0.9f;

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

    [Header("Render")]
    [SerializeField] SpriteRenderer _scenarioSpriteRenderer;
    [SerializeField] Material _mapMaterial;
    private Texture2D _islandDisplacementTex;
    [SerializeField] int _islandDisplacementTexResolution = 256;
    [SerializeField] float _islandDisplacementNoiseSize = 30;
    private Texture2D _islandGrassDetailTex;
    [SerializeField] int _islandGrassDetailTexResolution = 256;
    [SerializeField] float _islandGrassDetailNoiseSize = 30;
    private Texture2D _rockDisplacementTex;
    [SerializeField] int _rockDisplacementTexResolution = 128;
    [SerializeField] float _rockDisplacementNoiseSize = 100;

    public void CreateNewMap()
    {
        _islandNoiseTexture = CreateNoiseTexture(_islandNoiseTexResolution, _islandNoiseSize);

        _rocksNoiseTexture = CreateNoiseTexture(_rocksNoiseTexResolution, _rocksNoiseSize);

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
            collider.radius = mapElement.radius * _elementsRadiusMultiplier;
            instantiatedElementsList.Add(instantiatedElement);
        }
    }

    private void UpdateMapMaterial()
    {
        _mapMaterial.SetVectorArray("_islandData", _islands.Select(o => new Vector4(o.position.x, o.position.y, o.radius, 0)).ToArray());
        _mapMaterial.SetInt("_islandCount", _islands.Count);

        _mapMaterial.SetVectorArray("_rockData", _rocks.Select(o => new Vector4(o.position.x, o.position.y, o.radius, 0)).ToArray());
        _mapMaterial.SetInt("_rockCount", _rocks.Count);

        _islandDisplacementTex = CreateNoiseTexture(_islandDisplacementTexResolution, _islandDisplacementNoiseSize);
        _islandDisplacementTex.filterMode = FilterMode.Bilinear;
        _mapMaterial.SetTexture("_IslandDisplacementTex", _islandDisplacementTex);

        _islandGrassDetailTex = CreateNoiseTexture(_islandGrassDetailTexResolution, _islandGrassDetailNoiseSize);
        _islandGrassDetailTex.filterMode = FilterMode.Bilinear;
        _mapMaterial.SetTexture("_IslandGrassDetailTex", _islandGrassDetailTex);

        _rockDisplacementTex = CreateNoiseTexture(_rockDisplacementTexResolution, _rockDisplacementNoiseSize);
        _rockDisplacementTex.filterMode = FilterMode.Bilinear;
        _mapMaterial.SetTexture("_RockDisplacementTex", _rockDisplacementTex);

        _mapMaterial.SetFloat("_mapSize", _mapSize);
        _scenarioSpriteRenderer.material = _mapMaterial;
        _scenarioSpriteRenderer.transform.localScale = Vector3.one * _mapSize;
        _scenarioSpriteRenderer.transform.position = Vector2.one * _mapSize / 2;
    }

    public void ClearMap()
    {
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
