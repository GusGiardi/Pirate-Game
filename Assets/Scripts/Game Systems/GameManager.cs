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

    [SerializeField] PirateShip _playerShip;
    [SerializeField] PlayerCamera _playerCamera;
    [SerializeField] GameMapGenerator _mapGenerator;
    [SerializeField] float _mapLimitsBorderSize;
    [SerializeField] float _enemySpawnBorderSize = 10f;
    [SerializeField] EnemySpawner _enemySpawner;
    [SerializeField] LoadingFade _loadingFade;

    public Transform playerTransform => _playerShip.transform;
    public Vector2 cameraPosition => _playerCamera.transform.position;
    public Camera gameCamera => _playerCamera.camera;
    public GameMapGenerator mapGenerator => _mapGenerator;
    public float mapLimitsBorderSize => _mapLimitsBorderSize;
    public float enemySpawnBorderSize => _enemySpawnBorderSize;

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
        playerTransform.position = new Vector2(
            Mathf.Lerp(playerMinPosition, playerMaxPosition, Random.value),
            Mathf.Lerp(playerMinPosition, playerMaxPosition, Random.value));
        playerTransform.gameObject.SetActive(true);

        yield return new WaitForSeconds(2);

        _gameTimeCounter = _gameTime;
        _loadingFade.Open();
        _enemySpawner.enabled = true;
    }

    private void EndGame() 
    {
        _enemySpawner.enabled = false;
    }

    private IEnumerator DiscardLastGame() 
    {
        _loadingFade.Close();

        yield return new WaitForSeconds(2);

        _enemySpawner.RemoveAllEnemies();

        _mapGenerator.ClearMap();

        playerTransform.gameObject.SetActive(false);
    }

}
