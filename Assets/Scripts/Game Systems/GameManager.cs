using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager instance => _instance;

    private static float _gameTime = 180;
    private static float _enemySpawnRate = 5;
    public static float gameTime { get => _gameTime; set => _gameTime = value; }
    public static float enemySpawnRate { get => _enemySpawnRate; set => _enemySpawnRate = value; }

    [SerializeField] Transform _playerTransform;
    [SerializeField] GameMapGenerator _mapGenerator;
    [SerializeField] float _mapLimitsBorderSize;
    [SerializeField] LoadingFade _loadingFade;

    public Transform playerTransform => _playerTransform;
    public GameMapGenerator mapGenerator => _mapGenerator;
    public float mapLimitsBorderSize => _mapLimitsBorderSize;

    private float _gameTimeCounter;
    public float gameTimeCounter => _gameTimeCounter;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        StartCoroutine(StartNewGame());
    }

    private void Update()
    {
        if (_gameTimeCounter > 0)
        {
            _gameTimeCounter -= Time.deltaTime;
            if (_gameTimeCounter <= 0)
            {
                _gameTimeCounter = 0;
                EndGame();
            }
        }
    }

    private IEnumerator StartNewGame() 
    {
        _mapGenerator.CreateNewMap();

        float playerMinPosition = _mapGenerator.mapSize / 4f;
        float playerMaxPosition = _mapGenerator.mapSize * 3f / 4f;
        _playerTransform.position = new Vector2(
            Mathf.Lerp(playerMinPosition, playerMaxPosition, Random.value),
            Mathf.Lerp(playerMinPosition, playerMaxPosition, Random.value));
        _playerTransform.gameObject.SetActive(true);

        yield return new WaitForSeconds(2);

        _gameTimeCounter = _gameTime;
        _loadingFade.Open();
    }

    private void EndGame() 
    {

    }

}
