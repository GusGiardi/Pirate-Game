using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniMapManager : MonoBehaviour
{
    private static MiniMapManager _instance;
    public static MiniMapManager instance => _instance;

    [SerializeField] RawImage _mapImage;
    [SerializeField] Material _mapMaterialModel;

    private float _mapSize;
    [SerializeField] float _miniMapSize;

    [SerializeField] GameObject _playerIcon;
    [SerializeField] GameObject _enemyIcon;

    private Dictionary<Transform, GameObject> _trackedTransforms = new Dictionary<Transform, GameObject>();

    private void Awake()
    {
        _instance = this;
    }

    private void Update()
    {
        foreach (KeyValuePair<Transform, GameObject> mapIcon in _trackedTransforms)
        {
            UpdateIconPosition(mapIcon.Value.transform as RectTransform, mapIcon.Key);
        }
    }

    public void UpdateMapTexture(Vector4[] islandData, int islandCount, Vector4[] rockData, int rockCount, Texture2D islandDisplacementTex, Texture2D islandGrassDetailTex, Texture2D rockDisplacementTex, float mapSize)
    {
        Material mapMaterial = new Material(_mapMaterialModel);

        mapMaterial.SetVectorArray("_islandData", islandData);
        mapMaterial.SetInt("_islandCount", islandCount);

        mapMaterial.SetVectorArray("_rockData", rockData);
        mapMaterial.SetInt("_rockCount", rockCount);

        mapMaterial.SetTexture("_IslandDisplacementTex", islandDisplacementTex);

        mapMaterial.SetTexture("_IslandGrassDetailTex", islandGrassDetailTex);

        mapMaterial.SetTexture("_RockDisplacementTex", rockDisplacementTex);

        mapMaterial.SetFloat("_mapSize", mapSize);

        _mapImage.material = mapMaterial;
        _mapSize = mapSize;
    }

    public void TrackTransform(Transform transformToTrack, bool isPlayer)
    {
        GameObject icon = ObjectPoolManager.instance.InstantiateInPool(
            isPlayer ? _playerIcon : _enemyIcon,
            Vector3.zero,
            Quaternion.identity);

        icon.transform.SetParent(_mapImage.transform);
        UpdateIconPosition(icon.transform as RectTransform, transformToTrack);
        icon.SetActive(true);

        _trackedTransforms.Add(transformToTrack, icon);
    }

    public void RemoveTrackedTransform(Transform trackedObject)
    {
        if (!_trackedTransforms.ContainsKey(trackedObject))
            return;

        if (_trackedTransforms[trackedObject] != null)
        {
            _trackedTransforms[trackedObject].SetActive(false);
        }
        _trackedTransforms.Remove(trackedObject);
    }

    private void UpdateIconPosition(RectTransform icon, Transform trackedObject)
    {
        icon.rotation = trackedObject.rotation;
        icon.anchoredPosition = new Vector2(
            trackedObject.position.x * _miniMapSize / _mapSize,
            trackedObject.position.y * _miniMapSize / _mapSize);
    }
}
